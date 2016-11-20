using System;
using NSaga;
using Samples.BasicSaga;

namespace Samples
{
    public class StaticBuilderReference
    {
        public void Run()
        {
            SagaMediatorBuilder.Current
                               .UseSqlServer()
                               .WithConnectionStringName("NSagaDatabase");

            var sagaMediator = SagaMediatorBuilder.Current.ResolveMediator();

            var correlationId = Guid.NewGuid();

            sagaMediator.Consume(new BasicSagaStart(correlationId, "some name"));
            sagaMediator.Consume(new BasicSagaConsumeMessage(correlationId, "some value"));
        }
    }
}
