using NSaga;

namespace Tests.Composition
{
    public class TinyIocConformingContainerTests : ConformingContainerTestsBaseClass
    {
        public TinyIocConformingContainerTests()
        {
            Sut = new TinyIocConformingContainer();
        }
    }
}
