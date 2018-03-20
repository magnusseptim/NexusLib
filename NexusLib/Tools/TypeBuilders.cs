using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace NexusLib.Tools
{
    /// <summary>
    /// All class need to be refactored in future
    /// 
    /// Class response for build dynamic types with custom properties
    /// </summary>
    public class TypeBuilders
    {
        FieldBuilder fieldBuilder;
        PropertyBuilder propertyBuilder;
        MethodBuilder methodGetBuilder, methodSetBuilder;
        ILGenerator getGenerator, setGenerator;
        AppDomain Domain { get; set; }
        AssemblyName AssemblyName { get; set; }

        public Type BuildType(string typeName, IEnumerable<PropertyModel> propertyModel)
        {
            // generate name of assembly
            Guid guid = Guid.NewGuid();
            // return domain from thread
            Domain = Thread.GetDomain();
            // ugly as hell
            AssemblyName = new AssemblyName();

            AssemblyName.Name = guid.ToString();

            // define dynamic assembly with default Run builder access
            AssemblyBuilder myAsBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(guid.ToString()),AssemblyBuilderAccess.Run);

            // define module builder, same name like assembly
            ModuleBuilder moduleBuilder = myAsBuilder.DefineDynamicModule(AssemblyName.Name);
            // define type builder, TODO :  need to be added optional TypeAttribute
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            // Another attributes that should be optional in future
            MethodAttributes getSetAttr =
            MethodAttributes.Public | MethodAttributes.SpecialName |
            MethodAttributes.HideBySig;

            // Build properties from IEnumerable<PropertyModel> propertyModel
            // By default we build Get/Set property
            foreach (var property in propertyModel)
            {
                // Firstly, define underlying field
                fieldBuilder = typeBuilder.DefineField
                    (
                        // field name should allways be lowercase
                        Char.ToLowerInvariant(property.Name[0]).ToString() + property.Name.Substring(1, property.Name.Length - 1),
                        property.PropertyType,
                        property.UnderlyingFieldAttribute
                    );

                // Follow MSDN : 
                // The last argument of DefineProperty is null, because the
                // property has no parameters. (If you don't specify null, you must
                // specify an array of Type objects. For a parameterless property,
                // use an array with no elements: new Type[] {})
                propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attribute, property.PropertyType, null);

                // define get attribute
                methodGetBuilder = typeBuilder.DefineMethod("get_" + property.Name, getSetAttr, property.PropertyType, Type.EmptyTypes);

                // build "get" il generator
                getGenerator = methodGetBuilder.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                getGenerator.Emit(OpCodes.Ret);

                // define set attribute
                methodSetBuilder = typeBuilder.DefineMethod("set_" + property.Name,
                                       getSetAttr,
                                       null,
                                       new Type[] { property.PropertyType });

                // build "set" il generator
                setGenerator = methodSetBuilder.GetILGenerator();
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                setGenerator.Emit(OpCodes.Ret);

                // Mapping created method to "get" and "set" behavior
                propertyBuilder.SetGetMethod(methodGetBuilder);
                propertyBuilder.SetSetMethod(methodSetBuilder);

            }

            // return generated type
            return typeBuilder.CreateType();
        }
    }
}
