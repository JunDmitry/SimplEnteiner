using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Simple decorator for <see cref="ISimpleService"/>.
    /// </summary>
    internal class SimpleServiceDecorator : ISimpleService
    {
        public SimpleServiceDecorator(ISimpleService inner)
        {
            Inner = inner;
        }

        public ISimpleService Inner { get; }
    }

    /// <summary>
    /// Second decorator for <see cref="ISimpleService"/>.
    /// </summary>
    internal class SecondSimpleServiceDecorator : ISimpleService
    {
        public SecondSimpleServiceDecorator(ISimpleService inner)
        {
            Inner = inner;
        }

        public ISimpleService Inner { get; }
    }

    /// <summary>
    /// Generic decorator for <see cref="IGenericService{T}"/>.
    /// </summary>
    /// <typeparam name="T">Generic argument.</typeparam>
    internal class GenericServiceDecorator<T> : IGenericService<T>
    {
        public GenericServiceDecorator(IGenericService<T> inner)
        {
            Inner = inner;
        }

        public IGenericService<T> Inner { get; }
    }

    /// <summary>
    /// Generic decorator for <see cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal class RepositoryDecorator<T> : IRepository<T>
    {
        public RepositoryDecorator(IRepository<T> inner)
        {
            Inner = inner;
        }

        public IRepository<T> Inner { get; }
    }
}
