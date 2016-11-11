using System;

namespace NSaga
{
    public interface ISagaFactory
    {
        T ResolveSaga<T>() where T : class, IAccessibleSaga;
        IAccessibleSaga ResolveSaga(Type type);
        IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message);
        IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message);
    }
}
