using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with an explicit parameterless constructor.
    /// </summary>
    internal class ParameterlessCtorService
    {
        public ParameterlessCtorService()
        { }
    }

    /// <summary>
    /// Class with a single dependency constructor.
    /// </summary>
    internal class SingleDependencyCtorService
    {
        public SingleDependencyCtorService(ISimpleService service)
        {
            Service = service;
        }

        public ISimpleService Service { get; }
    }

    /// <summary>
    /// Class with two constructor dependencies.
    /// </summary>
    internal class TwoDependenciesCtorService
    {
        public TwoDependenciesCtorService(ISimpleService simpleService, IGenericService<int> genericService)
        {
            SimpleService = simpleService;
            GenericService = genericService;
        }

        public ISimpleService SimpleService { get; }

        public IGenericService<int> GenericService { get; }
    }

    /// <summary>
    /// Class with several public constructors for constructor selection tests.
    /// </summary>
    internal class MultipleCtorsService
    {
        public MultipleCtorsService()
        {
            UsedConstructor = "Parameterless";
        }

        public MultipleCtorsService(ISimpleService simpleService)
        {
            SimpleService = simpleService;
            UsedConstructor = "OneDependency";
        }

        public MultipleCtorsService(ISimpleService simpleService, IGenericService<int> genericService)
        {
            SimpleService = simpleService;
            GenericService = genericService;
            UsedConstructor = "Greediest";
        }

        public string UsedConstructor { get; }

        public ISimpleService SimpleService { get; }

        public IGenericService<int> GenericService { get; }
    }

    /// <summary>
    /// Class with ambiguous constructors of the same length.
    /// </summary>
    internal class AmbiguousCtorsService
    {
        public AmbiguousCtorsService(IFirstDerivedSimpleService first)
        {
            UsedConstructor = nameof(IFirstDerivedSimpleService);
        }

        public AmbiguousCtorsService(ISecondDerivedSimpleService second)
        {
            UsedConstructor = nameof(ISecondDerivedSimpleService);
        }

        public string UsedConstructor { get; }
    }

    /// <summary>
    /// Class with a private constructor only.
    /// </summary>
    internal class PrivateCtorService
    {
        private PrivateCtorService()
        { }
    }

    /// <summary>
    /// Class with an internal constructor only.
    /// </summary>
    internal class InternalCtorService
    {
        internal InternalCtorService()
        { }
    }

    /// <summary>
    /// Class with a protected constructor only.
    /// </summary>
    internal class ProtectedCtorService
    {
        protected ProtectedCtorService()
        { }
    }

    /// <summary>
    /// Class without any public constructor.
    /// </summary>
    internal class NoPublicCtorService
    {
        private NoPublicCtorService()
        { }
    }

    /// <summary>
    /// Class with primitive constructor arguments.
    /// </summary>
    internal class PrimitiveCtorService
    {
        public PrimitiveCtorService(int number, string text)
        {
            Number = number;
            Text = text;
        }

        public int Number { get; }

        public string Text { get; }
    }

    /// <summary>
    /// Class with optional constructor argument.
    /// </summary>
    internal class OptionalArgumentCtorService
    {
        public OptionalArgumentCtorService(ISimpleService service = null)
        {
            Service = service;
        }

        public ISimpleService Service { get; }
    }

    /// <summary>
    /// Class with default values for primitive constructor arguments.
    /// </summary>
    internal class DefaultValueCtorService
    {
        public DefaultValueCtorService(int number = 42, string text = "default")
        {
            Number = number;
            Text = text;
        }

        public int Number { get; }

        public string Text { get; }
    }

    /// <summary>
    /// Class with params-array constructor argument.
    /// </summary>
    internal class ParamsArrayCtorService
    {
        public ParamsArrayCtorService(params ISimpleService[] services)
        {
            Services = services;
        }

        public ISimpleService[] Services { get; }
    }

    /// <summary>
    /// Consumer of IEnumerable of services.
    /// </summary>
    internal class EnumerableDependencyConsumer
    {
        public EnumerableDependencyConsumer(IEnumerable<ISimpleService> services)
        {
            Services = services;
        }

        public IEnumerable<ISimpleService> Services { get; }
    }

    /// <summary>
    /// Consumer of array of services.
    /// </summary>
    internal class ArrayDependencyConsumer
    {
        public ArrayDependencyConsumer(ISimpleService[] services)
        {
            Services = services;
        }

        public ISimpleService[] Services { get; }
    }

    /// <summary>
    /// Consumer of closed generic service.
    /// </summary>
    internal class ClosedGenericDependencyConsumer
    {
        public ClosedGenericDependencyConsumer(IGenericService<int> service)
        {
            Service = service;
        }

        public IGenericService<int> Service { get; }
    }

    /// <summary>
    /// Consumer of open generic dependency.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class OpenGenericDependencyConsumer<T>
    {
        public OpenGenericDependencyConsumer(IGenericService<T> service)
        {
            Service = service;
        }

        public IGenericService<T> Service { get; }
    }

    /// <summary>
    /// Consumer with mixed dependency types.
    /// </summary>
    internal class MixedDependenciesConsumer
    {
        public MixedDependenciesConsumer(
            ISimpleService simpleService,
            IGenericService<string> genericService,
            IEnumerable<ISimpleService> services)
        {
            SimpleService = simpleService;
            GenericService = genericService;
            Services = services;
        }

        public ISimpleService SimpleService { get; }

        public IGenericService<string> GenericService { get; }

        public IEnumerable<ISimpleService> Services { get; }
    }

    /// <summary>
    /// Consumer of delegate factory.
    /// </summary>
    internal class FuncDependencyConsumer
    {
        public FuncDependencyConsumer(Func<ISimpleService> factory)
        {
            Factory = factory;
        }

        public Func<ISimpleService> Factory { get; }
    }

    /// <summary>
    /// Consumer of lazy dependency.
    /// </summary>
    internal class LazyDependencyConsumer
    {
        public LazyDependencyConsumer(Lazy<ISimpleService> lazyService)
        {
            LazyService = lazyService;
        }

        public Lazy<ISimpleService> LazyService { get; }
    }

    /// <summary>
    /// Generic consumer of repository dependency.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal class GenericRepositoryConsumer<T>
    {
        public GenericRepositoryConsumer(IRepository<T> repository)
        {
            Repository = repository;
        }

        public IRepository<T> Repository { get; }
    }

    /// <summary>
    /// Consumer of enumerable generic services.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class GenericEnumerableDependencyConsumer<T>
    {
        public GenericEnumerableDependencyConsumer(IEnumerable<IGenericService<T>> services)
        {
            Services = services;
        }

        public IEnumerable<IGenericService<T>> Services { get; }
    }

    /// <summary>
    /// Class with only private constructor.
    /// </summary>
    internal class ClassWithPrivateCtorOnly
    {
        private ClassWithPrivateCtorOnly() { }
    }

    /// <summary>
    /// Class with only internal constructor.
    /// </summary>
    internal class ClassWithInternalCtorOnly
    {
        internal ClassWithInternalCtorOnly() { }
    }

    /// <summary>
    /// Class with only protected constructor.
    /// </summary>
    internal class ClassWithProtectedCtorOnly
    {
        protected ClassWithProtectedCtorOnly() { }
    }

    /// <summary>
    /// Class with private and public constructors (should be concrete).
    /// </summary>
    internal class ClassWithMixedCtors
    {
        private ClassWithMixedCtors() { }

        public ClassWithMixedCtors(int x) { }
    }

    /// <summary>
    /// Class with no public constructors.
    /// </summary>
    internal class NoPublicCtorClass
    {
        private NoPublicCtorClass() { }
    }

    /// <summary>
    /// Class with only internal constructor.
    /// </summary>
    internal class OnlyInternalCtorClass
    {
        internal OnlyInternalCtorClass() { }
    }
}
