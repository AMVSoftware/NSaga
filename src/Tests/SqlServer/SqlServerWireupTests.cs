//using System;
//using FluentAssertions;
//using NSaga;
//using NSaga.SqlServer;
//using PetaPoco;
//using Tests.Stubs;
//using Xunit;


//namespace Tests.SqlServer
//{
//    public class SqlServerWireupTests
//    {
//        private readonly Database database;

//        public SqlServerWireupTests()
//        {
//            database = new Database("TestingConnectionString");
//        }


//        [Fact]
//        public void UseSqlServer_Registers_SqlServerRepository()
//        {
//            //Arrange
//            var mediatorBuilder = Wireup.UseInternalContainer()
//                .UseSqlServerStorage("TestingConnectionString");

//            // Act
//            var repository = mediatorBuilder.Container.Resolve<ISagaRepository>();


//            // Assert
//            repository.Should().BeOfType<SqlSagaRepository>();
//        }

//        [Fact]
//        public void UseSqlServer_Database_IsUsed()
//        {
//            //Arrange
//            var mediator = Wireup.UseInternalContainer()
//                                 .UseSqlServerStorage("TestingConnectionString")
//                                 .BuildMediator();
//            var correlationId = Guid.NewGuid();

//            // Act
//            var result = mediator.Consume(new MySagaInitiatingMessage(correlationId));

//            // Assert
//            result.IsSuccessful.Should().BeTrue();
//            var sagaData = DatabaseHelpers.GetSagaData(database, correlationId);
//            sagaData.Should().NotBeNull();
//        }


//        [Fact]
//        public void UseSqlServerWithProviderName_Database_IsUsed()
//        {
//            //Arrange
//            var mediator = Wireup.UseInternalContainer()
//                                 .UseSqlServerStorage(@"Server=(localdb)\v12.0;Database=NSaga-Testing", "System.Data.SqlClient")
//                                 .BuildMediator();
//            var correlationId = Guid.NewGuid();

//            // Act
//            var result = mediator.Consume(new MySagaInitiatingMessage(correlationId));

//            // Assert
//            result.IsSuccessful.Should().BeTrue();
//            var sagaData = DatabaseHelpers.GetSagaData(database, correlationId);
//            sagaData.Should().NotBeNull();
//        }
//    }
//}
