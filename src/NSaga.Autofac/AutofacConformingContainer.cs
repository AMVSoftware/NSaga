using System;
using System.Collections.Generic;
using Autofac;

namespace NSaga.Autofac
{
    public class AutofacConformingContainer : IConformingContainer
    {
        private readonly ContainerBuilder builder;
        private IContainer container;


        public AutofacConformingContainer(ContainerBuilder builder)
        {
            this.builder = builder;
        }

        public AutofacConformingContainer(IContainer container)
        {
            this.container = container;
            this.builder = new ContainerBuilder();
        }


        public T Resolve<T>() where T : class
        {
            if (container == null)
            {
                container = builder.Build();
            }
            else
            {
                builder.Update(container);
            }
            return container.Resolve<T>();
        }

        public void Register(Type registerType, object instance)
        {
            builder.RegisterInstance(instance).As(registerType);
        }

        public void Register(Type registerType, Type implementation)
        {
            builder.RegisterType(implementation).As(registerType);
        }

        public void RegisterMultiple(Type registerType, IEnumerable<Type> implementations)
        {
            foreach (var implementation in implementations)
            {
                builder.RegisterType(implementation).As(registerType);
            }
        }
    }
}
