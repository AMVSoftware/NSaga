using System;
using Newtonsoft.Json;

namespace NSaga
{
    /// <summary>
    /// Message serialiser based on Json.Net. Serialises/deserialises messages into/from JSON.
    /// </summary>
    /// <seealso cref="NSaga.IMessageSerialiser" />
    public sealed class JsonNetSerialiser : IMessageSerialiser
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        /// <summary>
        /// Serialises the specified message into JSON string
        /// </summary>
        /// <param name="message">The message to be serialised</param>
        /// <returns>
        /// String representing the serialised message
        /// </returns>
        public string Serialise(object message)
        {
            return JsonConvert.SerializeObject(message, Settings);
        }

        /// <summary>
        /// Deserialises the specified JSON string into a message object of a given type
        /// </summary>
        /// <param name="stream">The JSON string to be deserialised</param>
        /// <param name="objectType">Type of the object to return</param>
        /// <returns>
        /// Deserialised message
        /// </returns>
        public object Deserialise(string stream, Type objectType)
        {
            return JsonConvert.DeserializeObject(stream, objectType, Settings);
        }

        /// <summary>
        /// Deserialises the specified JSON string into a message object of a given type
        /// </summary>
        /// <typeparam name="T">Type of object to deserialise into</typeparam>
        /// <param name="stream">The JSON string to be deserialised</param>
        /// <returns>
        /// Deserialised message
        /// </returns>
        public T Deserialise<T>(string stream)
        {
            return JsonConvert.DeserializeObject<T>(stream, Settings);
        }
    }
}