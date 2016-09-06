using System;
using Newtonsoft.Json;

namespace NSaga
{
    public class JsonNetSerialiser : IMessageSerialiser
    {
        public string Serialise(object message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public object Deserialise(string stream, Type objectType)
        {
            return JsonConvert.DeserializeObject(stream, objectType);
        }

        public T Deserialise<T>(string stream)
        {
            return JsonConvert.DeserializeObject<T>(stream);
        }
    }
}