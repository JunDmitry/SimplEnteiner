using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplEnteinerTests.TestTypes.Interfaces;

namespace SimplEnteinerTests.TestTypes.Classes
{
    /// <summary>
    /// Class with a unique identifier for instance identity tests.
    /// </summary>
    internal class InstanceIdService
    {
        public InstanceIdService()
        {
            InstanceId = Guid.NewGuid();
        }

        public Guid InstanceId { get; }
    }

    /// <summary>
    /// Class that counts how many instances were created.
    /// </summary>
    internal class CreationCounterService
    {
        public CreationCounterService()
        {
            CreatedCount++;
        }

        public static int CreatedCount { get; private set; }

        public static void Reset()
        {
            CreatedCount = 0;
        }
    }

    /// <summary>
    /// Disposable class for disposal tracking tests.
    /// </summary>
    internal class DisposableService : IDisposable
    {
        public static int DisposeCount { get; private set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            DisposeCount++;
        }

        public static void Reset()
        {
            DisposeCount = 0;
        }
    }

    /// <summary>
    /// Disposable implementation of <see cref="ISimpleService"/>.
    /// </summary>
    internal class DisposableSimpleService : ISimpleService, IDisposable
    {
        public static int DisposeCount { get; private set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            DisposeCount++;
        }

        public static void Reset()
        {
            DisposeCount = 0;
        }
    }

    /// <summary>
    /// Async-disposable class for async disposal tracking tests.
    /// </summary>
    internal class AsyncDisposableService : IAsyncDisposable
    {
        public static int DisposeAsyncCount { get; private set; }

        public bool IsDisposed { get; private set; }

        public ValueTask DisposeAsync()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                DisposeAsyncCount++;
            }

            return default;
        }

        public static void Reset()
        {
            DisposeAsyncCount = 0;
        }
    }

    /// <summary>
    /// Async-disposable implementation of <see cref="ISimpleService"/>.
    /// </summary>
    internal class AsyncDisposableSimpleService : ISimpleService, IAsyncDisposable
    {
        public static int DisposeAsyncCount { get; private set; }

        public bool IsDisposed { get; private set; }

        public ValueTask DisposeAsync()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                DisposeAsyncCount++;
            }

            return default;
        }

        public static void Reset()
        {
            DisposeAsyncCount = 0;
        }
    }

    /// <summary>
    /// Class implementing both <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/>.
    /// </summary>
    internal class SyncAndAsyncDisposableService : IDisposable, IAsyncDisposable
    {
        public static int DisposeCount { get; private set; }

        public static int DisposeAsyncCount { get; private set; }

        public bool IsDisposed { get; private set; }

        public bool IsAsyncDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                DisposeCount++;
            }
        }

        public ValueTask DisposeAsync()
        {
            if (!IsAsyncDisposed)
            {
                IsAsyncDisposed = true;
                DisposeAsyncCount++;
            }

            return default;
        }

        public static void Reset()
        {
            DisposeCount = 0;
            DisposeAsyncCount = 0;
        }
    }

    /// <summary>
    /// Consumer of a disposable dependency.
    /// </summary>
    internal class DisposableDependencyConsumer
    {
        public DisposableDependencyConsumer(DisposableService service)
        {
            Service = service;
        }

        public DisposableService Service { get; }
    }

    /// <summary>
    /// Class whose constructor always throws.
    /// </summary>
    internal class ThrowingCtorService
    {
        public ThrowingCtorService()
        {
            throw new InvalidOperationException("Constructor failure.");
        }
    }
}
