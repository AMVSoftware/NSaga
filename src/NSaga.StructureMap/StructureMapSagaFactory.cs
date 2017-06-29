using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace NSaga.StructureMap
{
    public class StructureMapSagaFactory : ISagaFactory
    {
        private readonly IContainer _container;

        public StructureMapSagaFactory(IContainer container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            _container = container;
        }
        
        public T ResolveSaga<T>() where T : class, IAccessibleSaga
        {
            return _container.GetInstance<T>();
        }

        public IAccessibleSaga ResolveSaga(Type type)
        {
            return (IAccessibleSaga)_container.GetInstance(type);
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga) _container.GetInstance(interfaceType);
        }

        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)_container.GetInstance(interfaceType);
        }


    }
}
