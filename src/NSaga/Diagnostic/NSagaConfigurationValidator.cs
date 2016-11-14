using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public sealed class NSagaConfigurationValidator
    {
        private readonly IEnumerable<Assembly> assemblies;

        public NSagaConfigurationValidator(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }


        public void AssertConfigurationIsValid()
        {
            var allExceptions = new List<NSagaDiagnosticException>();

            var sagaTypes = NSagaReflection.GetAllSagaTypes(assemblies);


            foreach (var sagaType in sagaTypes)
            {
                //all sagas have Initialiser
                var initInterface = sagaType.GetInterfaces().Where(i => i.IsGenericType && i == typeof(InitiatedBy<>));
                if (!initInterface.Any())
                {
                    allExceptions.Add(new NSagaDiagnosticException($"Saga of type {sagaType.FullName} does not have an initialiser interface applied. Add 'InitiatedBy<>' to the Saga class"));
                    continue;
                }
                //all messages that initialise should be IInitiliseMessage
                //var initInterfaceGenericType = initInterface.
            }

            foreach (var sagaType in sagaTypes)
            {
                
            }


            //TODO no message sharing between different sagas
            //TODO all saga messages should be applied
        }
    }

    public sealed class NSagaDiagnosticException : Exception
    {
        public NSagaDiagnosticException(String message) : base(message)
        {
            // nothing
        }
        // nothing here so far
    }

    //internal interface IAssemblyValidator
    //{
    //    void ValidateConfiguration(IEnumerable<Assembly> assemblies);
    //}
}
