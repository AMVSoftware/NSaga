using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    /// <summary>
    /// Builder class that configures and creates Mediator and allows to register your services for injection into your sagas
    /// </summary>
    public sealed class SagaMediatorBuilder
    {
        private static SagaMediatorBuilder current;

        /// <summary>
        /// Global singleton reference for SagaMediatorBuilder
        /// </summary>
        public static SagaMediatorBuilder Current
        {
            get
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (current == null)
                {
                    current = new SagaMediatorBuilder();
                }
                return current;
            }
            set { current = value; }
        }

        private readonly List<Type> pipelineHooks;
        private List<Assembly> assembliesToScan;
        private bool areComponentsRegistered;
        private TinyIoCContainer Container { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SagaMediatorBuilder"/> class.
        /// Creates default registrations for NSaga components, but available for overriding later. 
        /// <para>
        /// Default registrations are:
        /// <list type="bullet">
        /// <item><description><see cref="JsonNetSerialiser"/> to serialise messages; </description></item> 
        /// <item><description><see cref="InMemorySagaRepository"/> to store saga datas; </description></item> 
        /// <item><description><see cref="TinyIocSagaFactory"/> to resolve instances of Sagas;</description></item> 
        /// <item><description><see cref="SagaMetadata"/> to work as the key component - SagaMediator;</description></item> 
        /// <item><description><see cref="MetadataPipelineHook"/> added to the pipeline to preserve metadata about incoming messages.</description></item> 
        /// <item><description>All currently loaded assemblies from AppDomain will be scanned for Saga classes</description></item>
        /// </list>
        /// </para>
        /// </summary>
        public SagaMediatorBuilder(params Assembly[] assembliesToScan)
        {
            Container = new TinyIoCContainer();
            pipelineHooks = new List<Type>();
            if (assembliesToScan == null || !assembliesToScan.Any())
            {
                this.assembliesToScan = AppDomain.CurrentDomain.GetAssemblies().ToList();
            }
            else
            {
                this.assembliesToScan = assembliesToScan.ToList();
            }
            areComponentsRegistered = false;

            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            UseSagaFactory<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddPiplineHook<MetadataPipelineHook>();
        }


        /// <summary>
        /// Replaces all previosly reigstered assemblies with the provided ones
        /// </summary>
        /// <param name="assemblies">List of assemblies to scan for sagas</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder ReplaceAssembliesToScan(IEnumerable<Assembly> assemblies)
        {
            this.assembliesToScan = assembliesToScan.ToList();
            return this;
        }

        /// <summary>
        /// Adds the assembly to the current list of assemblies to scan.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder AddAssemblyToScan(Assembly assembly)
        {
            assembliesToScan.Add(assembly);
            return this;
        }

        /// <summary>
        /// Adds the assemblies to the current list of assemblies to scan.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder AddAssembliesToScan(IEnumerable<Assembly> assemblies)
        {
            assembliesToScan.AddRange(assemblies);
            return this;
        }


        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>. By default it is <see cref="JsonNetSerialiser"/>
        /// </summary>
        /// <typeparam name="TSerialiser">The type of the serialiser.</typeparam>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            Container.Register(typeof(IMessageSerialiser), typeof(TSerialiser));

            return this;
        }

        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>. By default it is <see cref="JsonNetSerialiser"/>
        /// </summary>
        /// <param name="messageSerialiser">The message serialiser.</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            Container.Register(typeof(IMessageSerialiser), messageSerialiser);

            return this;
        }


        /// <summary>
        /// Override ISagaRepository with the provided type.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            Container.Register(typeof(ISagaRepository), typeof(TRepository));

            return this;
        }

        /// <summary>
        /// Override ISagaRepository with the provided instance
        /// </summary>
        /// <param name="sagaRepository">The saga repository instance to use</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseRepository(ISagaRepository sagaRepository)
        {
            Container.Register(typeof(ISagaRepository), sagaRepository);

            return this;
        }


        /// <summary>
        /// Override the provided implementation of ISagaFactory with given type
        /// </summary>
        /// <typeparam name="TSagaFactory">The type of the saga factory to be used</typeparam>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseSagaFactory<TSagaFactory>() where TSagaFactory : ISagaFactory
        {
            Container.Register(typeof(ISagaFactory), typeof(TSagaFactory));

            return this;
        }

        /// <summary>
        /// Override the provided implementation of ISagaFactory with given instance
        /// </summary>
        /// <param name="sagaFactory">The saga factory instance</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder UseSagaFactory(ISagaFactory sagaFactory)
        {
            Container.Register(typeof(ISagaFactory), sagaFactory);

            return this;
        }


        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <typeparam name="TPipelineHook">The type of the pipeline hook.</typeparam>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder AddPiplineHook<TPipelineHook>() where TPipelineHook : IPipelineHook
        {
            pipelineHooks.Add(typeof(TPipelineHook));

            return this;
        }


        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <param name="pipelineHookType">Type of the pipeline hook.</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        /// <exception cref="System.ArgumentException">Provided type is not a class or does not implement IPipelineHook</exception>
        public SagaMediatorBuilder AddPiplineHook(Type pipelineHookType)
        {
            if (!pipelineHookType.IsClass || !pipelineHookType.GetInterfaces().Contains(typeof(IPipelineHook)))
            {
                throw new ArgumentException("Provided type is not a class or does not implement IPipelineHook");
            }

            pipelineHooks.Add(pipelineHookType);

            return this;
        }

        /// <summary>
        /// Registers the specified type as provided @interface to be used in internal DI container
        /// </summary>
        /// <param name="interface">The interface.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>Current Builder for fluent configuration.</returns>
        public SagaMediatorBuilder Register(Type @interface, Type implementation)
        {
            Container.Register(@interface, implementation);

            return this;
        }


        /// <summary>
        /// Registers the specified instance as given registerType in the internal DI container
        /// </summary>
        /// <param name="registerType">Type to register.</param>
        /// <param name="instance">The instance to resolve</param>
        /// <returns></returns>
        public SagaMediatorBuilder Register(Type registerType, object instance)
        {
            Container.Register(registerType, instance);

            return this;
        }


        private SagaMediatorBuilder RegisterComponents()
        {
            if (areComponentsRegistered)
            {
                throw new Exception("Components have already been registered. Can't register second time");
            }

            Container.RegisterMultiple(typeof(IPipelineHook), pipelineHooks);
            Container.Register(typeof(ISagaMediator), typeof(SagaMediator));
            Container.RegisterSagas(assembliesToScan);
            areComponentsRegistered = true;

            return this;
        }

        /// <summary>
        /// Resolves an instance of <see cref="ISagaMediator"/> from internal DI container
        /// </summary>
        /// <returns>Resolved SagaMediator instance</returns>
        public ISagaMediator ResolveMediator()
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            var mediator = Container.Resolve<ISagaMediator>();
            return mediator;
        }

        /// <summary>
        /// Resolves an instance of <see cref="ISagaRepository"/> from internal DI container
        /// </summary>
        /// <returns>Resolved instance of repository</returns>
        public ISagaRepository ResolveRepository()
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve<ISagaRepository>();
        }

        /// <summary>
        /// Resolves a requested instance of an object from the internal DI container
        /// </summary>
        /// <typeparam name="T">Type of an object to return</typeparam>
        /// <returns>An instance of requested type</returns>
        public T Resolve<T>() where T : class
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve<T>();
        }

        /// <summary>
        /// Resolves a requested instance of an object from the internal DI container
        /// </summary>
        /// <param name="objectType">Type of the object to resolve</param>
        /// <returns>an instance of an object</returns>
        public object Resolve(Type objectType)
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve(objectType);
        }

    }
}