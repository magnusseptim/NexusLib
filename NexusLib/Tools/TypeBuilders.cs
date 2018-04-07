using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TypeBuilders : ITypeBuilders
    {
        FieldBuilder fieldBuilder;
        PropertyBuilder propertyBuilder;
        MethodBuilder methodGetBuilder, methodSetBuilder;
        ILGenerator getGenerator, setGenerator;
        AppDomain Domain { get; set; }
        AssemblyName AssemblyName { get; set; }
        Guid guid;
        IList<Type> CreatedTypes { get; set; }
        MethodAttributes getSetAttr;

        public TypeBuilders(MethodAttributes? getSetAttr = null)
        {
            CreatedTypes = new List<Type>();
            // Another attributes that should be optional in future
            this.getSetAttr = getSetAttr.HasValue   ? 
                              getSetAttr.Value      : MethodAttributes.Public | MethodAttributes.SpecialName |
                                                      MethodAttributes.HideBySig;
            // return domain from thread
            Domain = Thread.GetDomain();
            // ugly as hell
            AssemblyName = new AssemblyName();
        }


        public Type BuildType(string typeName, IEnumerable<PropertyModel> propertyModel)
        {
            // generate name of assembly
            guid = Guid.NewGuid();
            AssemblyName.Name = guid.ToString();

            // define dynamic assembly with default Run builder access
            AssemblyBuilder myAsBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(guid.ToString()),AssemblyBuilderAccess.Run);

            // define module builder, same name like assembly
            ModuleBuilder moduleBuilder = myAsBuilder.DefineDynamicModule(AssemblyName.Name);
            // define type builder, TODO :  need to be added optional TypeAttribute
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            // Build properties from IEnumerable<PropertyModel> propertyModel
            // By default we build Get/Set property
            foreach (var property in propertyModel)
            {
                // Firstly, define underlying field
                fieldBuilder = BuildUnderlyingField(typeBuilder, property);

                // Follow MSDN : 
                // The last argument of DefineProperty is null, because the
                // property has no parameters. (If you don't specify null, you must
                // specify an array of Type objects. For a parameterless property,
                // use an array with no elements: new Type[] {})
                propertyBuilder = typeBuilder.DefineProperty(char.ToUpper(property.Name[0]) + property.Name.Substring(1), property.Attribute, property.PropertyType, null);

                // define get attribute
                // define set attribute
                // Mapping created method to "get" and "set" behavior
                propertyBuilder.SetGetMethod(BuildGetProperty(typeBuilder, property));
                propertyBuilder.SetSetMethod(BuildSetProperty(typeBuilder, property));

            }

            var type = typeBuilder.CreateType();

            if(!CreatedTypes.Contains(type))
            {
                CreatedTypes.Add(type);
            }

            // return generated type
            return type;
        }

        public IList<Type> GetCreatedTypes()
        {
            return CreatedTypes;
        }

        private MethodBuilder BuildGetProperty(TypeBuilder typeBuilder, PropertyModel property)
        {
            methodGetBuilder = typeBuilder.DefineMethod("get_" + char.ToUpper(property.Name[0]) + property.Name.Substring(1), getSetAttr, property.PropertyType, Type.EmptyTypes);

            // build "get" il generator
            getGenerator = methodGetBuilder.GetILGenerator();
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getGenerator.Emit(OpCodes.Ret);

            return methodGetBuilder;
        }

        private MethodBuilder BuildSetProperty(TypeBuilder typeBuilder, PropertyModel property)
        {
            methodSetBuilder = typeBuilder.DefineMethod("set_" + char.ToUpper(property.Name[0]) + property.Name.Substring(1),
                                        getSetAttr,
                                        null,
                                        new Type[] { property.PropertyType });

            // build "set" il generator
            setGenerator = methodSetBuilder.GetILGenerator();
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setGenerator.Emit(OpCodes.Ret);

            return methodSetBuilder;
        }

        private FieldBuilder BuildUnderlyingField(TypeBuilder typeBuilder, PropertyModel property)
        {
            return typeBuilder.DefineField
                 (
                     // field name should allways be lowercase
                     Char.ToLowerInvariant(property.Name[0]).ToString() + property.Name.Substring(1, property.Name.Length - 1),
                     property.PropertyType,
                     property.UnderlyingFieldAttribute
                 );
        }

        public override bool Equals(object obj)
        {
            var builders = obj as TypeBuilders;
            return builders != null &&
                   EqualityComparer<FieldBuilder>.Default.Equals(fieldBuilder, builders.fieldBuilder) &&
                   EqualityComparer<PropertyBuilder>.Default.Equals(propertyBuilder, builders.propertyBuilder) &&
                   EqualityComparer<MethodBuilder>.Default.Equals(methodGetBuilder, builders.methodGetBuilder) &&
                   EqualityComparer<MethodBuilder>.Default.Equals(methodSetBuilder, builders.methodSetBuilder) &&
                   EqualityComparer<ILGenerator>.Default.Equals(getGenerator, builders.getGenerator) &&
                   EqualityComparer<ILGenerator>.Default.Equals(setGenerator, builders.setGenerator) &&
                   EqualityComparer<AppDomain>.Default.Equals(Domain, builders.Domain) &&
                   EqualityComparer<AssemblyName>.Default.Equals(AssemblyName, builders.AssemblyName) &&
                   guid.Equals(builders.guid) &&
                   EqualityComparer<IList<Type>>.Default.Equals(CreatedTypes, builders.CreatedTypes) &&
                   getSetAttr == builders.getSetAttr;
        }

        public override int GetHashCode()
        {
            var hashCode = 1568179448;
            hashCode = hashCode * -1521134295 + EqualityComparer<FieldBuilder>.Default.GetHashCode(fieldBuilder);
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyBuilder>.Default.GetHashCode(propertyBuilder);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodBuilder>.Default.GetHashCode(methodGetBuilder);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodBuilder>.Default.GetHashCode(methodSetBuilder);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILGenerator>.Default.GetHashCode(getGenerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILGenerator>.Default.GetHashCode(setGenerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<AppDomain>.Default.GetHashCode(Domain);
            hashCode = hashCode * -1521134295 + EqualityComparer<AssemblyName>.Default.GetHashCode(AssemblyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(guid);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Type>>.Default.GetHashCode(CreatedTypes);
            hashCode = hashCode * -1521134295 + getSetAttr.GetHashCode();
            return hashCode;
        }
    }
}
