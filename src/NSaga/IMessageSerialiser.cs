using System;

namespace NSaga
{
    public interface IMessageSerialiser
    {
        string Serialise(object message);
        object Deserialise(string stream, Type objectType);
        T Deserialise<T>(String stream);
    }
}
