using System;
using FluentAssertions;
using NSaga;
using NSaga.SqlServer;
using PetaPoco;
using Xunit;


namespace Tests.SqlServer
{
    [AutoRollback]
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
            var mediatorBuilder = Wireup.UseInternalContainer()
                .UseSqlServerStorage("TestingConnectionString");

            // Act
            var repository = mediatorBuilder.Container.Resolve<ISagaRepository>();


            // Assert
            repository.Should().BeOfType<SqlSagaRepository>();
        }

        [Fact]
        public void UseSqlServer_Database_IsUsed()
        {
            //Arrange
            var mediator = Wireup.UseInternalContainer()
                                 .UseSqlServerStorage("TestingConnectionString")
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
            var mediator = Wireup.UseInternalContainer()
                                 .UseSqlServerStorage(@"Server=(localdb)\v12.0;Database=NSaga-Testing", "System.Data.SqlClient")
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
            var mediator = Wireup.UseInternalContainer()
                                 .UseSqlServerStorage("TestingConnectionString")
                                 .ResolveMediator();
            // Act
            var repository = Reflection.GetPrivate(mediator, "sagaRepository");

            // Assert
            repository.Should().BeOfType<SqlSagaRepository>();
        }

        public void Dispose()
        {
            DatabaseHelpers.CleanUpData(database);
        }
    }
}
