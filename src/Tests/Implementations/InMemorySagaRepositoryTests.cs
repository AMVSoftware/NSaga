using NSaga;
using Tests.Stubs;

namespace Tests
{
    public class InMemorySagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public InMemorySagaRepositoryTests()
        {
            this.Sut = new InMemorySagaRepository(new JsonNetSerialiser(), new DumbSagaFactory());
        }
    }
}