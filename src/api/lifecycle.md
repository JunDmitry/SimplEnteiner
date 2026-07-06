# Lifecycle Interfaces and Enums

Namespace: `SimplEnteiner.Core.Lifecycle`
Source: [`Core/Lifecycle/`](../../SimplEnteiner/Core/Lifecycle)

## `LifeTime`

```csharp
public enum LifeTime
{
    Transient = 0,
    Singleton = 1,
    Cached = 2,
    Scoped = 3,
}
```

| Value | Storage | Sharing scope | Disposal |
|---|---|---|---|
| `Transient` | Not cached — a new instance is created on every resolution. | None. | Tracked by the **current scope's** `CleanupService` and disposed when that scope is disposed. |
| `Singleton` | Stored in the **tree-wide shared** `IRepositoryService` (owned by the root scope, shared by reference into every `ScopeCreationConfig`). | Entire scope tree (root + all descendants). | Disposed only when the **root** scope is disposed. |
| `Cached` | Stored in the current top-level `Resolve()` call's `ResolutionContext.CachedInstances`. | Only within the object graph built during a **single** `Resolve()` invocation — i.e., if two different dependencies of the same root request both need the cached service, they get the same instance; two separate `Resolve()` calls get two different instances. | Tracked by the current scope's `CleanupService`. |
| `Scoped` | Stored in the current scope's private `_scopedInstances` dictionary. | Only within the scope it was created in (not visible to sibling or child scopes; each child scope resolves its own instance). | Disposed when that specific scope is disposed. |

See [Scopes, Lifetimes and Disposal](../core/scopes-and-lifetimes.md) for a full walk-through with diagrams.

## `IInitializable`

```csharp
public interface IInitializable
{
    void Initialize();
}
```

Implement on any resolvable class to receive a synchronous callback right after construction and member injection, but **before** the instance is returned to the caller and before `OnActivation` fires. Invoked by `Resolver.ResolveRegistration` via the internal `IInterfaceInvoker`, for every newly-created instance (including decorator instances, see `Resolver.CreateDecoratorInstance`).

```csharp
public class ReportGenerator : IInitializable
{
    public void Initialize()
    {
        // Runs once, right after this instance is constructed.
    }
}
```

## `IAsyncInitializable`

```csharp
public interface IAsyncInitializable
{
    Task InitializeAsync();
}
```

Unlike `IInitializable`, this is **not** invoked automatically by the synchronous `Resolve<T>()`/`Resolve(Type)` path. It is only invoked when the caller explicitly resolves via `ResolveAsync<T>()` / `ResolveAsync(Type)` (on `IScope`/`DIContainer`/`Scope`), which resolves synchronously first and then `await`s `InitializeAsync()`:

```csharp
public class DatabaseConnection : IAsyncInitializable
{
    public async Task InitializeAsync()
    {
        await OpenConnectionAsync();
    }
}

var db = await container.ResolveAsync<DatabaseConnection>();
```

## `IStartable`

```csharp
public interface IStartable
{
    void Start();
}
```

Invoked once, only for **singleton-lifetime exact registrations**, during `Scope.Build()` → `Scope.Start()`. For every `ExactBindings` entry:

- If the registration already has a live `Instance` (i.e. it was bound via `ToInstance(...)`), `Start()` is invoked on it directly.
- Else if `Lifetime == LifeTime.Singleton`, the service is eagerly resolved (which triggers construction, causing it to be stored in the singleton repository) and then `Start()` is invoked.
- Non-singleton registrations (`Transient`, `Scoped`, `Cached`) are **not** eagerly instantiated and therefore never receive an automatic `Start()` call through this mechanism.

`Build()` recurses into child scopes (`_childrens[i].Build()`), so `IStartable.Start()` fires for singleton services registered at any level of the scope tree, in parent-then-children traversal order.

```csharp
public class BackgroundWorker : IStartable
{
    public void Start()
    {
        // Runs once when the container/scope tree is Build()-ed,
        // only because this service is registered AsSingle().
    }
}

container.Bind<BackgroundWorker>().ToSelf().AsSingle().Apply();
container.Build(); // BackgroundWorker.Start() is invoked here
```

## `IInterfaceInvoker` (internal)

Source: [`Core/Lifecycle/IInterfaceInvoker.cs`](../../SimplEnteiner/Core/Lifecycle/IInterfaceInvoker.cs), [`InterfaceInvoker.cs`](../../SimplEnteiner/Core/Lifecycle/InterfaceInvoker.cs)

```csharp
internal interface IInterfaceInvoker
{
    void Invoke<T>(object instance);
    Task InvokeAsync<T>(object instance);
}
```

Internal helper used by `Scope`/`Resolver` to dispatch to `IInitializable.Initialize()`, `IStartable.Start()`, and `IAsyncInitializable.InitializeAsync()` based on the generic type argument `T` — a small, closed-set visitor rather than a general-purpose reflection-based dispatcher.

## `ICleanupService` / `CleanupService` (internal)

Source: [`Core/Lifecycle/ICleanupService.cs`](../../SimplEnteiner/Core/Lifecycle/ICleanupService.cs), [`CleanupService.cs`](../../SimplEnteiner/Core/Lifecycle/CleanupService.cs)

```csharp
internal interface ICleanupService : IDisposable, IAsyncDisposable
{
    void AddIfDisposable(object instance, Action<object> onRelease = null);
}
```

Tracks every created instance that implements `IDisposable` (regardless of lifetime, except `Singleton`, which is tracked separately by `RepositoryService`) inside a scope, together with its optional `onRelease` callback. `Dispose()`/`DisposeAsync()` invokes `onRelease` for each tracked instance and then disposes it (preferring `IAsyncDisposable.DisposeAsync()` in the async path when available), clearing the tracked list afterwards. Every `Scope` owns its own `CleanupService` instance.

Continue to [Attributes and Delegates](./attributes-delegates.md).
