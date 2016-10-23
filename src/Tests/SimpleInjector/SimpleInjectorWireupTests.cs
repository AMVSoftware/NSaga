using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSaga;
using SimpleInjector;
using Xunit;
using NSaga.SimpleInjector;

namespace Tests.SimpleInjector
{
    public class SimpleInjectorWireupTests
    {
        [Fact]
        public void MethodName_StateUnderTests_ExpectedBehaviour()
        {
            //Arrange
            var container = new Container();
            var builder = Wireup.Init().UseSimpleInjector(container);
            var mediator = builder.BuildMediator();

            // Act


            // Assert
        }
    }
}
