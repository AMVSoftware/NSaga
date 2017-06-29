using System;
using FluentAssertions;
using NSaga;
using PetaPoco;
using Xunit;
using System.Reflection;

namespace Tests.SqlServer
{
    [AutoRollback]
    [Collection("Sql Tests")]
    public class SqlServerWireupTests : IDisposable
    {
        private readonly Database database;

        public SqlServerWireupTests()
        {
            database = new Database("TestingConnectionString");
        }


        [Fact]
        public void UseSqlServer_Registers_SqlServerRepository()
        {
            //Arrange
            var mediatorBuilder = Wireup.UseInternalContainer(Assembly.GetExecutingAssembly())
                                        .UseSqlServer()
                                        .WithConnectionStringName("TestingConnectionString");

            // Act
            var repository = mediatorBuilder.ResolveRepository();


            // Assert
            repository.Should().BeOfType<SqlSagaRepository>();
        }

        [Fact]
        public void UseSqlServer_Database_IsUsed()
        {
            //Arrange
            var mediator = Wireup.UseInternalContainer(Assembly.GetExecutingAssembly())
                                 .UseSqlServer()
                                 .WithConnectionStringName("TestingConnectionString")
                                 .ResolveMediator();
            var correlationId = Guid.NewGuid();

            // Act
            var result = mediator.Consume(new MySagaInitiatingMessage(correlationId));

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var sagaData = DatabaseHelpers.GetSagaData(database, correlationId);
            sagaData.Should().NotBeNull();
        }


        [Fact]
        public void UseSqlServerWithProviderName_Database_IsUsed()
        {
            //Arrange
            var mediator = Wireup.UseInternalContainer(Assembly.GetExecutingAssembly())
                                 .UseSqlServer()
                                 .WithConnectionString(@"Server=(localdb)\v12.0;Database=NSaga-Testing")
                                 .ResolveMediator();
            var correlationId = Guid.NewGuid();

            // Act
            var result = mediator.Consume(new MySagaInitiatingMessage(correlationId));

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var sagaData = DatabaseHelpers.GetSagaData(database, correlationId);
            sagaData.Should().NotBeNull();
        }


        [Fact]
        public void UseSqlServer_SetsPrivate_ToSqlRepository()
        {
            //Arrange
            var builder = Wireup.UseInternalContainer(Assembly.GetExecutingAssembly())
                                 .UseSqlServer()
                                 .WithConnectionStringName("TestingConnectionString");
            // Act
            var repository = builder.Resolve<ISagaRepository>();

            // Assert
            repository.Should().BeOfType<SqlSagaRepository>();
        }

        public void Dispose()
        {
            database.Dispose();
        }
    }
}
