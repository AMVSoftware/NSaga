using System;
using System.Collections.Generic;

namespace NSaga
{
    public interface IConformingContainer
    {
        T Resolve<T>() where T : class;
        void Register(Type registerType, object instance);
        void Register(Type registerType, Type implementation);
        void RegisterMultiple(Type registerType, IEnumerable<Type> implementations);
    }
}