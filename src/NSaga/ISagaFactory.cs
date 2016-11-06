using System;

namespace NSaga
{
    //TODO remove generic option
    //TODO use IAccessibleSaga more here
    public interface ISagaFactory
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
    }
}
