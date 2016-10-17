using System;

namespace NSaga
{
    //TODO rename this into ISagaFactory
    //TODO remove generic option
    //TODO use IAccessibleSaga more here
    public interface IServiceLocator
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
    }
}
