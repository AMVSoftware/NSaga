using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSaga;
using NSaga.StructureMap;
using StructureMap;
using Tests.Implementations;
using System.Reflection;

namespace Tests.StructureMap
{
    public class StructureMapFactoryTests : SagaFactoryTestsTemplate
    {
        public StructureMapFactoryTests()
        {
            var builder = new Container();
            builder.RegisterNSagaComponents(Assembly.GetExecutingAssembly());

            Sut = builder.GetInstance<ISagaFactory>();
        }
    }
}
