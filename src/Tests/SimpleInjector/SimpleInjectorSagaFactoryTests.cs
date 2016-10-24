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
            container.Register<IMyService, MyService>();
            sut = new SimpleInjectorSagaFactory(container);
        }

        [Fact]
        public void Resolve_Resolves_Instance()
        {
            var result = sut.Resolve<IMyService>();

            result.Should().BeOfType<MyService>();
        }


        [Fact]
        public void ResolveByType_Resolves_Instance()
        {
            var result = sut.Resolve(typeof(IMyService));

            result.Should().BeOfType<MyService>();
        }


        public interface IMyService
        {
            void DoSomething();
        }

        public class MyService : IMyService
        {
            public void DoSomething()
            {
                // nothing
            }
        }
    }
}
