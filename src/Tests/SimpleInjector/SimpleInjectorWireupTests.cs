//using System;
//using FluentAssertions;
//using NSaga;
//using SimpleInjector;
//using Xunit;
//using NSaga.SimpleInjector;
//using Tests.Stubs;


//namespace Tests.SimpleInjector
//{
//    public class SimpleInjectorWireupTests
//    {
//        private readonly ISagaMediator sagaMediator;

//        public SimpleInjectorWireupTests()
//        {
//            var container = new Container();
//            var mediatorBuilder = Wireup.Init().UseSimpleInjector(container);

//            // Act
//            sagaMediator = mediatorBuilder.BuildMediator();
//        }

//        [Fact]
//        public void Mediator_Not_Null()
//        {
//            sagaMediator.Should().NotBeNull();
//        }


//        [Fact]
//        public void Default_Provides_InMemoryRepository()
//        {
//            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaRepository");

//            sagaRepository.Should().BeOfType<InMemorySagaRepository>();
//        }


//        [Fact]
//        public void Default_Provides_SimpleInjectorSagaFactory()
//        {
//            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaFactory");

//            sagaRepository.Should().BeOfType<SimpleInjectorSagaFactory>();
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
