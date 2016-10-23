using NSaga.SimpleInjector;
using SimpleInjector;

namespace Tests.Composition
{
    public class SimpleInjectorConformingContainerTests : ConformingContainerTestsBaseClass
    {
        public SimpleInjectorConformingContainerTests()
        {
            Sut = new SimpleConformingContainer(new Container());
        }
    }
}
