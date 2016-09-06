using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSaga;
using NSaga.SqlServer;
using Tests.Stubs;
using Xunit;

namespace Tests.SqlServer
{
    public class SqlSagaRepositorySpecificTests
    {
        private readonly SqlSagaRepository sut;

        public SqlSagaRepositorySpecificTests()
        {
            sut = new SqlSagaRepository("TestingConnectionString", new DumbServiceLocator(), new JsonNetSerialiser());
        }


        [Fact]
        public void Find_NoSaga_ReturnsNull()
        {
            // Act
            var result = sut.Find<MySaga>(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
    }
}
