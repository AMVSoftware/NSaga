using Autofac;
using NSaga.Autofac;

namespace Tests.Composition
{
    public class AutofacConformingContainerTests : ConformingContainerTestsBaseClass
    {
        public AutofacConformingContainerTests()
        {
            Sut = new AutofacConformingContainer(new ContainerBuilder());
        }
    }


    public class AutofacNonBuilderConformingContainerTests : ConformingContainerTestsBaseClass
    {
        public AutofacNonBuilderConformingContainerTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SuperService>();
            var container = builder.Build();

            Sut = new AutofacConformingContainer(container);
        }


        public class SuperService { }
    }
}
