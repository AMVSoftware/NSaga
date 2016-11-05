//using System;
//using System.Collections.Generic;
//using System.Linq;
//using FluentAssertions;
//using NSaga;
//using Xunit;


//namespace Tests.Composition
//{
//    public abstract class ConformingContainerTestsBaseClass
//    {
//        internal virtual IConformingContainer Sut { get; set; }


//        [Fact]
//        public void Register_Instance_ResolvesCorrectly()
//        {
//            //Arrange
//            var expectedId = Guid.NewGuid();
//            Sut.Register(typeof(IMyService), new MyService(expectedId));

//            // Act
//            var result = Sut.Resolve<IMyService>();

//            // Assert
//            result.Id.Should().Be(expectedId);
//        }

//        [Fact]
//        public void Registration_ByType_Resolves()
//        {
//            //Arrange
//            Sut.Register(typeof(IMyService), typeof(MySimpleImplementation));

//            // Act
//            var result = Sut.Resolve<IMyService>();

//            // Assert
//            result.Should().BeOfType(typeof(MySimpleImplementation));
//        }

//        [Fact]
//        public void MultipleRegistrations_DoesNotThrow()
//        {
//            //Arrange
//            Action act = () => DoComplexRegistration(Guid.NewGuid());

//            // Assert
//            act.ShouldNotThrow();
//        }


//        [Fact]
//        public void MultipleRegistration_Returns_NonNull()
//        {
//            var expectedId = Guid.NewGuid();
//            var result = DoComplexRegistration(expectedId);

//            // Assert
//            result.Should().NotBeNull();
//        }


//        [Fact]
//        public void MultipleRegistrations_Have_CorrectNumberOfFilters()
//        {
//            var expectedId = Guid.NewGuid();
//            var result = DoComplexRegistration(expectedId);

//            result.Filters.Should().HaveCount(3);
//        }

//        [Fact]
//        public void MultipleRegistrations_Returns_ExpectedId()
//        {
//            var expectedId = Guid.NewGuid();
//            var result = DoComplexRegistration(expectedId);

//            // Assert
//            result.Filters.First(f => f.GetType() == typeof(FilterThree)).GetId.Should().Be(expectedId);
//        }

//        private MyFilterCollection DoComplexRegistration(Guid expectedId)
//        {
//            Sut.Register(typeof(IMyService), new MyService(expectedId));
//            var filters = new List<Type>()
//            {
//                typeof(FilterOne),
//                typeof(FilterTwo),
//                typeof(FilterThree),
//            };
//            Sut.RegisterMultiple(typeof(IFilter), filters);
//            Sut.Register(typeof(MyFilterCollection), typeof(MyFilterCollection));

//            var result = Sut.Resolve<MyFilterCollection>();

//            return result;
//        }


//        interface IMyService
//        {
//            Guid Id { get; set; }
//        }

//        class MyService : IMyService
//        {
//            public Guid Id { get; set; }

//            public MyService(Guid id)
//            {
//                this.Id = id;
//            }
//        }

//        class MySimpleImplementation : IMyService
//        {
//            public Guid Id { get; set; }
//        }


//        interface IFilter
//        {
//            Guid GetId { get; }
//        }

//        class FilterOne : IFilter
//        {
//            public Guid GetId => new Guid("9D8E7D10-CEEB-4017-B8C8-F1F71CCEB223");
//        }

//        class FilterTwo : IFilter 
//        {
//            public Guid GetId => new Guid("F59781A8-E488-4FB8-9389-A790BDDEB84F");
//        }

//        class FilterThree : IFilter
//        {
//            public IMyService MyService { get; set; }

//            public Guid GetId => MyService.Id;

//            public FilterThree(IMyService myService)
//            {
//                MyService = myService;
//            }
//        }


//        class MyFilterCollection
//        {
//            public IEnumerable<IFilter> Filters { get; set; }

//            public MyFilterCollection(IEnumerable<IFilter> filters )
//            {
//                Filters = filters;
//            }
//        }
//    }
//}
