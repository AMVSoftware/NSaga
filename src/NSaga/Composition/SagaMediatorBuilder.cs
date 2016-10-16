using System.Reflection;
using NSaga.Pipeline;

namespace NSaga
{
    //public class SagaMediatorBuilder
    //{
    //    private IMessageSerialiser messageSerialiser;
    //    private ISagaRepository sagaRepository;
    //    private IServiceLocator serviceLocator;
    //    private IPipelineHook pipelineHook;
    //    private Assembly[] assembliesToScan;


    //    public SagaMediatorBuilder()
    //    {
    //        messageSerialiser = new JsonNetSerialiser();
    //        serviceLocator = new DumbServiceLocator();
    //        pipelineHook = new CompositePipelineHook(new MetadataPipelineHook(messageSerialiser));
    //        sagaRepository = new InMemorySagaRepository(messageSerialiser, serviceLocator);
    //    }

    //    public SagaMediatorBuilder AddAssemblies(Assembly[] assemblies)
    //    {
    //        assembliesToScan = assemblies;
    //        return this;
    //    }

    //    public SagaMediatorBuilder SetMessageSerialiser(IMessageSerialiser messageSerialiser)
    //    {
    //        this.messageSerialiser = messageSerialiser;
    //        return this;
    //    }

    //    public SagaMediatorBuilder SetRepository(ISagaRepository sagaRepository)
    //    {
    //        this.sagaRepository = sagaRepository;
    //        return this;
    //    }



    //    public ISagaMediator Build()
    //    {
    //        return new SagaMediator(sagaRepository, serviceLocator, pipelineHook, assembliesToScan);
    //    }
    //}
}
