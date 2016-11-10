using System;
using NSaga;

namespace Tests
{
    public class NullSagaFactory : ISagaFactory
    {
        public T Resolve<T>() where T : class  { throw new NotImplementedException(); }
        public object Resolve(Type type) { throw new NotImplementedException(); }
    }
}