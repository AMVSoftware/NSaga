using Autofac;
using FluentAssertions;
using NSaga.Autofac;
using Xunit;

namespace Tests.Autofac
{
    public class AutofacServiceLocatorTests
    {
        private readonly AutofacServiceLocator sut;

        public AutofacServiceLocatorTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyService>().As<IMyService>();
            var container = builder.Build();
            sut = new AutofacServiceLocator(container);
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
