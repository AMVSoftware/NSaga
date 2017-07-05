using System;
using NSaga;

namespace Tests.Stubs
{
    public class NullMessageSerialiser : IMessageSerialiser
    {
        public string Serialise(object message)
        {
            throw new NotImplementedException();
        }

        public object Deserialise(string stream, Type objectType)
        {
            throw new NotImplementedException();
        }

        public T Deserialise<T>(string stream)
        {
            throw new NotImplementedException();
        }
    }
}
