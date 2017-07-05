using System;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests.Implementations
{
    public abstract class SagaFactoryTestsTemplate
    {
        protected ISagaFactory Sut;

        [Fact]
        public void Resolve_Saga_Resolved()
        {
            var result = Sut.ResolveSaga(typeof(MySaga));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }

        [Fact]
        public void Resolve_SagaGeneric_Resolved()
        {
            var result = Sut.ResolveSaga(typeof(ISaga<MySagaData>));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }


        [Fact]
        public void ResolveGeneric_Saga_Resolved()
        {
            var result = Sut.ResolveSaga<MySaga>();

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }

        [Fact]
        public void ResolveGeneric_SagaGeneric_Resolved()
        {
            var result = Sut.ResolveSaga<ISaga<MySagaData>>();

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }

        [Fact]
        public void ResolveByConsumed_Always_Resolves()
        {
            var result = Sut.ResolveSagaConsumedBy(new MySagaConsumingMessage(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }


        [Fact]
        public void ResolveByInitiated_Always_Resolves()
        {
            var result = Sut.ResolveSagaInititatedBy(new MySagaInitiatingMessage(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }


        [Fact]
        public void ResolveByInitiated_AdditinalInterface_Resolves()
        {
            var result = Sut.ResolveSagaInititatedBy(new MySagaAdditionalInitialser(Guid.NewGuid()));

            result.Should().NotBeNull().And.BeOfType<MySaga>();
        }
    }
}
