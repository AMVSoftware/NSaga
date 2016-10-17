using System;

namespace NSaga
{
    public interface IConformingContainer
    {
        T Resolve<T>() where T : class;
        void Register(Type registerType, object instance);
        void Register(Type registerType, Type implementation);
    }
}