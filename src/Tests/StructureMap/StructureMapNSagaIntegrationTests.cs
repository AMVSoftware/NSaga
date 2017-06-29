using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSaga;
using NSaga.StructureMap;
using StructureMap;
using Xunit;
using Xunit.Abstractions;

namespace Tests.StructureMap
{
    public class StructureMapNSagaIntegrationTests
    {
        private readonly ITestOutputHelper output;
        public StructureMapNSagaIntegrationTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Theory]
        [InlineData(typeof(ISagaMediator), typeof(SagaMediator))]
        [InlineData(typeof(ISagaRepository), typeof(InMemorySagaRepository))]
        [InlineData(typeof(ISagaFactory), typeof(StructureMapSagaFactory))]
        [InlineData(typeof(IMessageSerialiser), typeof(JsonNetSerialiser))]
        [InlineData(typeof(ISaga<MySagaData>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaInitiatingMessage>), typeof(MySaga))]
        [InlineData(typeof(ConsumerOf<MySagaConsumingMessage>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaAdditionalInitialser>), typeof(MySaga))]
        public void DefaultRegistration_Resolves_DefaultComponents(Type requestedType, Type expectedImplementation)
        {
            //Arrange
            var container = new Container().RegisterNSagaComponents();

            // Act
            var result = container.GetInstance(requestedType);

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType(expectedImplementation);
        }


        [Fact]
        public void DefaultRegistration_ResolvePipline_ResolvesMetadataHook()
        {
            //Arrange
            var container = new Container()
                                .RegisterNSagaComponents();

            // Act
            var result = container.GetInstance<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(1)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook));
        }

        [Fact]
        public void AddPipeline_Resolves_AdditionalHooks()
        {
            //Arrange
            var container = new Container()
                                .RegisterNSagaComponents()
                                .AddSagaPipelineHook<NullPipelineHook>();

            // Act
            var result = container.GetInstance<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(2)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook))
                       .And.Contain(h => h.GetType() == typeof(NullPipelineHook));
        }

        [Fact]
        public void OverrideRepository_Resolves_OverridenRepository()
        {
            //Arrange
            var container = new Container()
                                .RegisterNSagaComponents()
                                .UseSagaRepository<NullSagaRepository>();

            // Act
            var result = container.GetInstance<ISagaRepository>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullSagaRepository>();
        }

        [Fact]
        public void Default_Can_Initialise_Saga()
        {
            //Arrange
            var container = new Container().RegisterNSagaComponents();

            var sagaMediator = container.GetInstance<ISagaMediator>();
            
            // Act
            var result = sagaMediator.Consume(new MySagaInitiatingMessage(Guid.NewGuid()));

            // Assert
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseSqlServerRepository_Registers_AndResolves()
        {
            //Arrange
            var container = new Container().RegisterNSagaComponents()
                    .UseSqlServer()
                    .WithConnectionStringName("TestingConnectionString");

            var all = container.Model.AllInstances.OrderBy(x => x.Name);

            ////debug
            //foreach (var instance in all)
            //{
            //    output.WriteLine($"{instance.ReturnedType.ToString()}----{instance.PluginType.ToString()}");
            //}

            // Act
            var repository = container.GetInstance<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull().And.BeOfType<SqlSagaRepository>();
        }

        [Fact]
        public void UseSqlServerRepository_RegistersByConnectionString_AndResolves()
        {
            //Arrange
            var container = new Container()
                                .RegisterNSagaComponents()
                                .UseSqlServer()
                                .WithConnectionString(@"Server=(localdb)\v12.0;Database=NSaga-Testing");

            // Act
            var repository = container.GetInstance<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull().And.BeOfType<SqlSagaRepository>();
        }
    }

}
