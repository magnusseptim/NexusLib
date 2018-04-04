using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

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
    }
}
