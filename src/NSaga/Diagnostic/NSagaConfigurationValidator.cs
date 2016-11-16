using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    /// <summary>
    /// Class that should be used in your unit-tests: checks if all your sagas and messages are used correctly
    /// </summary>
    public sealed class NSagaConfigurationValidator
    {
        private readonly IEnumerable<Assembly> assemblies;
        private IEnumerable<Type> sagaTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NSagaConfigurationValidator"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public NSagaConfigurationValidator(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }


        /// <summary>
        /// Asserts that your sagas and messages are written correctly:
        /// <list type="bullet">
        /// <item><description>All classes impmenenting <see cref="ISaga{TSagaData}"/> should also have <see cref="InitiatedBy{TMsg}"/> applied.</description></item>
        /// <item><description>Any saga message can only be used by a single Saga. No message sharing between different sagas are allowed.</description></item>
        /// <item><description>All saga messages are used by sagas. No loose classes with <see cref="ISagaMessage"/> or <see cref="IInitiatingSagaMessage"/> without sagas consuming them are allowed.</description></item>
        /// </list>
        /// </summary>
        /// <exception cref="AggregateException">Aggregation of all the exceptions for each rule violation</exception>
        public void AssertConfigurationIsValid()
        {
            var allExceptions = new List<Exception>();

            sagaTypes = NSagaReflection.GetAllSagaTypes(assemblies);

            allExceptions.AddRange(AllSagasHaveInitialiser());

            allExceptions.AddRange(InitiatorMessagesAreNotShared());

            allExceptions.AddRange(ConsumerMessagesAreNotShared());
            

            if (allExceptions.Any())
            {
                throw new AggregateException(allExceptions);
            }
        }



        /// <summary>
        /// All sagas must have an Initialiser
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Exception> AllSagasHaveInitialiser()
        {
            foreach (var sagaType in sagaTypes)
            {
                var initInterfaces = sagaType.GetInterfaces()
                                             .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                                             .ToList();
                if (!initInterfaces.Any())
                {
                    yield return 
                        new Exception(
                            $"Saga of type {sagaType.FullName} does not have an initialiser interface applied. Add 'InitiatedBy<>' to the Saga class");
                }
            }
        }


        private IEnumerable<Exception> InitiatorMessagesAreNotShared()
        {
            var initiatingMessageTypes = assemblies.SelectMany(a => a.GetTypes())
                                                   .Where(t => t.IsClass)
                                                   .Where(t => typeof(IInitiatingSagaMessage).IsAssignableFrom(t))
                                                   .ToList();

            foreach (var messageType in initiatingMessageTypes)
            {
                var initiatingInterfaceType = typeof(InitiatedBy<>).MakeGenericType(messageType);

                var initiatedSagaTypes = sagaTypes.Where(t => initiatingInterfaceType.IsAssignableFrom(t)).ToList();

                if (initiatedSagaTypes.Count() > 1)
                {
                    var sagaTypesCsv = String.Join(", ", initiatedSagaTypes.Select(t => t.Name));
                    yield return
                        new Exception(
                            $"Message of type {messageType.Name} initiates more than one saga: {sagaTypesCsv}. Any message can be consumed only by a single Saga");
                }
                else if (initiatedSagaTypes.Count() == 0)
                {
                    yield return
                        new Exception(
                            $"Message of type {messageType.Name} does not initiate any sagas. Have you forgotten to apply 'InitiatedBy<{messageType.Name}> to your Saga?'");
                }
            }
        }


        private IEnumerable<Exception> ConsumerMessagesAreNotShared()
        {
            var consumerMessageTypes = assemblies.SelectMany(a => a.GetTypes())
                                                   .Where(t => t.IsClass)
                                                   .Where(t => typeof(ISagaMessage).IsAssignableFrom(t))
                                                   .Where(t => !typeof(IInitiatingSagaMessage).IsAssignableFrom(t))
                                                   .ToList();

            foreach (var messageType in consumerMessageTypes)
            {
                var consumerInterfaceType = typeof(ConsumerOf<>).MakeGenericType(messageType);

                var consumerSagaTypes = sagaTypes.Where(t => consumerInterfaceType.IsAssignableFrom(t)).ToList();

                if (consumerSagaTypes.Count() > 1)
                {
                    var sagaTypesCsv = String.Join(", ", consumerSagaTypes.Select(t => t.Name));
                    yield return
                        new Exception(
                            $"Message of type {messageType.Name} is consumed by more than one saga: {sagaTypesCsv}. Any message can be consumed only by a single Saga");
                }
                else if (consumerSagaTypes.Count() == 0)
                {
                    yield return
                        new Exception(
                            $"Message of type {messageType.Name} is not consumed by any sagas. Have you forgotten to apply 'ConsumerOf<{messageType.Name}> to your Saga?'");
                }
            }
        }
    }
}
