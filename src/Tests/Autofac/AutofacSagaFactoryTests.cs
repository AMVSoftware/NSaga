//using Autofac;
//using FluentAssertions;
//using NSaga.Autofac;
//using Xunit;

//namespace Tests.Autofac
//{
//    public class AutofacSagaFactoryTests
//    {
//        private readonly AutofacSagaFactory sut;

//        public AutofacSagaFactoryTests()
//        {
//            var builder = new ContainerBuilder();
//            builder.RegisterType<MyService>().As<IMyService>();
//            var container = builder.Build();
//            sut = new AutofacSagaFactory(container);
//        }

//        [Fact]
//        public void Resolve_Resolves_Instance()
//        {
//            var result = sut.ResolveSaga<IMyService>();

//            result.Should().BeOfType<MyService>();
//        }


//        [Fact]
//        public void ResolveByType_Resolves_Instance()
//        {
//            var result = sut.ResolveSaga(typeof(IMyService));

//            result.Should().BeOfType<MyService>();
//        }



//        public interface IMyService
//        {
//            void DoSomething();
//        }

//        public class MyService : IMyService
//        {
//            public void DoSomething()
//            {
//                // nothing
//            }
//        }
//    }
//}
