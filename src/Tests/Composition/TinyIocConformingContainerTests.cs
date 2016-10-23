using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSaga;

namespace Tests.Composition
{
    public class TinyIocConformingContainerTests : ConformingContainerTestsBaseClass
    {
        public TinyIocConformingContainerTests()
        {
            Sut = new TinyIocConformingContainer();
        }
    }
}
