using System;
using System.Collections.Generic;
using System.Reflection;
using NexusLib;
using NexusLib.Model;
using NexusLib.Repository;
using NexusLib.Tools;

namespace ExperimentalField
{
    class Program
    {
        static void Main(string[] args)
        {
            TypeBuilders builder = new TypeBuilders(MethodAttributes.Public);

            Type ourType = builder.BuildType("testtype",
                                              new List<PropertyModel>()
                                              {
                                                  new PropertyModel
                                                  (
                                                      "prop1",
                                                      typeof(string),
                                                      System.Reflection.PropertyAttributes.HasDefault,
                                                      System.Reflection.FieldAttributes.Private
                                                  ),
                                                   new PropertyModel
                                                  (
                                                      "prop2",
                                                      typeof(int),
                                                      System.Reflection.PropertyAttributes.HasDefault,
                                                      System.Reflection.FieldAttributes.Private
                                                  )
                                              });

            PropertyInfo[] custDataPropInfo = ourType.GetProperties();
            foreach (PropertyInfo pInfo in custDataPropInfo)
            {
                Console.WriteLine("Property '{0}' created!", pInfo.ToString());
            }

            Console.WriteLine("---");
            // From MSDN Note
            // Note that when invoking a property, you need to use the proper BindingFlags -
            // BindingFlags.SetProperty when you invoke the "set" behavior, and 
            // BindingFlags.GetProperty when you invoke the "get" behavior. Also note that
            // we invoke them based on the name we gave the property, as expected, and not
            // the name of the methods we bound to the specific property behaviors.

            object ourTypeData = Activator.CreateInstance(ourType);
            ourType.InvokeMember(custDataPropInfo[0].Name, BindingFlags.SetProperty,
                                          null, ourTypeData, new object[] { "testdata" });

            ourType.InvokeMember(custDataPropInfo[1].Name, BindingFlags.SetProperty,
                                          null, ourTypeData, new object[] { 12 });

            Console.WriteLine("The field of instance has been set to '{0}'.",
                               ourType.InvokeMember(custDataPropInfo[0].Name, BindingFlags.GetProperty,
                                                          null, ourTypeData, new object[] { }));

            Console.WriteLine("The field of instance has been set to '{0}'.",
                                         ourType.InvokeMember(custDataPropInfo[1].Name, BindingFlags.GetProperty,
                                                                    null, ourTypeData, new object[] { }));
            Console.ReadKey();

        }
    }
}
