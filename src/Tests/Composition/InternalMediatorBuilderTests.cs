using System;
using System.Collections.Generic;
using FluentAssertions;
using NSaga;
using Xunit;


namespace Tests.Composition
{
    public class InternalMediatorBuilderTests
    {
        [Theory]
        [InlineData(typeof(IMessageSerialiser), typeof(JsonNetSerialiser))]
        [InlineData(typeof(ISagaRepository), typeof(InMemorySagaRepository))]
        [InlineData(typeof(ISagaFactory), typeof(TinyIocSagaFactory))]
        [InlineData(typeof(ISagaMediator), typeof(SagaMediator))]
        [InlineData(typeof(IEnumerable<IPipelineHook>), typeof(IEnumerable<IPipelineHook>))]
        public void Can_Resolve_Component(Type requested, Type expected)
        {
            var builder = Wireup.UseInternalContainer().RegisterComponents();
            var container = builder.Container;

            var result = container.Resolve(requested);
            result.Should().BeAssignableTo(expected);
        }
    }
}
