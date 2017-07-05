using System;

namespace NSaga
{
    /// <summary>
    /// Abstraction to define how saga messages are serialised
    /// </summary>
    public interface IMessageSerialiser
    {
        /// <summary>
        /// Serialises the specified message into string
        /// </summary>
        /// <param name="message">The message to be serialised</param>
        /// <returns>String representing the serialised message</returns>
        string Serialise(object message);


        /// <summary>
        /// Deserialises the specified string into a message object of a given type
        /// </summary>
        /// <param name="stream">The string to be deserialised</param>
        /// <param name="objectType">Type of the object to return</param>
        /// <returns>Deserialised message</returns>
        object Deserialise(string stream, Type objectType);


        /// <summary>
        /// Deserialises the specified string into a message object of a given type
        /// </summary>
        /// <typeparam name="T">Type of object to deserialise into</typeparam>
        /// <param name="stream">The string to be deserialised</param>
        /// <returns>Deserialised message</returns>
        T Deserialise<T>(String stream);
    }
}
