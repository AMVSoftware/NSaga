using System;
using FluentAssertions;
using NSaga.SimpleInjector;
using SimpleInjector;
using Xunit;

namespace Tests.SimpleInjector
{
    public class SimpleInjectorSagaFactoryTests
    {
        private readonly SimpleInjectorSagaFactory sut;

        public SimpleInjectorSagaFactoryTests()
        {
            var container = new Container();
            container.RegisterNSagaComponents();
            sut = new SimpleInjectorSagaFactory(container);
        }

        [Fact]
        public void Resolve_Saga_Resolved()
        {
            var result = sut.ResolveSaga(typeof(MySaga));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }

        [Fact]
        public void ResolveGeneric_Saga_Resolved()
        {
            var result = sut.ResolveSaga<MySaga>();

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }

        [Fact]
        public void ResolveByConsumed_Always_Resolves()
        {
            var result = sut.ResolveSagaConsumedBy(new MySagaConsumingMessage(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }


        [Fact]
        public void ResolveByInitiated_Always_Resolves()
        {
            var result = sut.ResolveSagaInititatedBy(new MySagaInitiatingMessage(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }


        [Fact]
        public void ResolveByInitiated_AdditinalInterface_Resolves()
        {
            var result = sut.ResolveSagaInititatedBy(new MySagaAdditionalInitialser(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }
    }
}
