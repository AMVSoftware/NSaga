//using System;
//using Autofac;
//using FluentAssertions;
//using NSaga;
//using NSaga.Autofac;
//using Tests.Stubs;
//using Xunit;

//namespace Tests.Autofac
//{
//    public class AutofacWireupTests
//    {
//        private readonly SagaMediator sagaMediator;

//        public AutofacWireupTests()
//        {
//            var container = new ContainerBuilder().Build();

//            sagaMediator = (SagaMediator) Wireup.Init().UseAutofac(container).BuildMediator();
//        }

//        [Fact]
//        public void Default_Provides_InMemoryRepository()
//        {
//            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaRepository");

//            sagaRepository.Should().BeOfType<InMemorySagaRepository>();
//        }

//        [Fact]
//        public void Default_Provides_AutofacSagaFactory()
//        {
//            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaFactory");

//            sagaRepository.Should().BeOfType<AutofacSagaFactory>();
//        }

//        [Fact]
//        public void Default_Can_Initialise_Saga()
//        {
//            //Arrange
//            var correlationId = Guid.NewGuid();

//            // Act
//            var result = sagaMediator.Consume(new MySagaInitiatingMessage(correlationId));

//            // Assert
//            result.IsSuccessful.Should().BeTrue();
//        }
//    }
//}
