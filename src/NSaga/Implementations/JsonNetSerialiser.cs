using System;
using Newtonsoft.Json;

namespace NSaga
{
    public sealed class JsonNetSerialiser : IMessageSerialiser
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
        };

        public string Serialise(object message)
        {
            return JsonConvert.SerializeObject(message, settings);
        }

        public object Deserialise(string stream, Type objectType)
        {
            return JsonConvert.DeserializeObject(stream, objectType, settings);
        }

        public T Deserialise<T>(string stream)
        {
            return JsonConvert.DeserializeObject<T>(stream, settings);
        }
    }
}