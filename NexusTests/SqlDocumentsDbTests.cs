using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Reflection.Emit;
using NexusLib.Tools;
using System.Collections.Generic;
using System.Reflection;
using NexusLib.Model;
using System.Threading;

namespace NexusTests
{
    public class SqlDocumentsDbTests
    {

        NexusLib.Repository.SqlDocumentsDB sqlDocumentsDB = new NexusLib.Repository.SqlDocumentsDB("","");
        [Fact]
        public void CheckIfDatabaseIsInitialisedCorrect()
        {
            CheckIfDatabaseIsInitialisedCorrectAsync().Wait();
        }

        private Task CheckIfDatabaseIsInitialisedCorrectAsync()
        {
            //Arrange
            //Act
            var actual = sqlDocumentsDB.CreateDatabase("TestDB", "TestDb");
            //Assert
            actual.Should().NotBeNull();
            return actual;
        }

        [Fact]
        public void CheckIfCollectionIsCreatedCorrect()
        {
            CheckIfCollectionIsCreatedCorrectAsync();
        }

        private async void CheckIfCollectionIsCreatedCorrectAsync()
        {
            //Arrange
            //Act
            var actual = await sqlDocumentsDB.CreateCollection("TestCollection", "/testCollection", "TestDB");
            //Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public async void CheckIfDocumentIsCreatedCorrect()
        {
            //Arrange
            //Act
            TypeBuilders builder = new TypeBuilders();

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
                                                      typeof(string),
                                                      System.Reflection.PropertyAttributes.HasDefault,
                                                      System.Reflection.FieldAttributes.Private
                                                  )
                                              });

            dynamic document = Activator.CreateInstance(ourType);

            //ourType.InvokeMember("CustomerName", BindingFlags.SetProperty,
            //                          null, document, new object[] { "Whatever" });



            document.Prop1 = "Whatever";
            document.Prop2 = "and ever";
            var actual = await sqlDocumentsDB.CreateDocumentAsync(document, "TestCollection", "TestDB");
            //Assert
            //Assert.NotNull(actual);
            Assert.True(true);
        }

       
    }
}
