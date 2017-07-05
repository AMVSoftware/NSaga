using NSaga;
using Xunit;


namespace Tests
{
    [Collection("InMemorySagaRepository")]
    public class InMemorySagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public InMemorySagaRepositoryTests()
        {
            this.Sut = new InMemorySagaRepository(new JsonNetSerialiser(), new DumbSagaFactory());
        }
    }
}