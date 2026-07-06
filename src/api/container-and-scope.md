# Container and Scope

## `DIContainer`

Namespace: `SimplEnteiner.Core`
Source: [`Core/DIContainer.cs`](../../SimplEnteiner/Core/DIContainer.cs)

```csharp
public class DIContainer : IScope, IBindingTarget
```

`DIContainer` is the top-level, public entry point of the library. It owns a root [`Scope`](#iscope--scope) and delegates virtually every member to it. Create one instance per application (or per logical composition root).

### Construction

```csharp
public DIContainer();
```

Creates an empty root container, wiring up default implementations for `IResolver`, `IScopeFactory`, `IRegistryFactory`, and `IRepositoryService` (see `ConfigureConfig` in the source).

### Binding Members

| Member | Signature | Description |
|---|---|---|
| `Bind<TService>()` | `IBindingTo<TService> Bind<TService>()` | Starts a fluent binding for `TService`. See [Binder API](./binder.md). |
| `Bind(Type)` | `IBindingTo Bind(Type serviceType)` | Non-generic overload. Throws `ArgumentNullException` if `serviceType` is `null`. |
| `Decorate<TService>()` | `IBindingDecorate<TService> Decorate<TService>()` | Starts a fluent decorator registration. See [Decorators](../core/decorators.md). |
| `Decorate(Type)` | `IBindingDecorate Decorate(Type interfaceType)` | Non-generic overload. |
| `BindConvention(Action<IConventionBuilder>)` | `void BindConvention(Action<IConventionBuilder> configure)` | Configures and immediately executes assembly-scanning based registration. See [Convention-Based Binding](./convention-binding.md). |
| `Install(IInstaller)` | `virtual void Install(IInstaller installer)` | Invokes `installer.Install(this)` — a simple modularization hook (see `IInstaller`). |

Bindings created via `Bind<T>()`/`Bind(Type)` are added to an internal pending list (`_pendingBindings`) and are **not** immediately visible to the registry — they are executed (staged pipeline run to completion) and registered either:

- Eagerly, when `.Apply()` is called on the resulting `IBindingOptions`, or
- In batch, when `Build()` is called (which flushes any bindings that were never explicitly `Apply()`-ed).

### Resolution Members

| Member | Signature |
|---|---|
| `Resolve<TService>()` | `TService Resolve<TService>()` |
| `Resolve(Type)` | `object Resolve(Type type)` |
| `Resolve<T>(object id)` | `T Resolve<T>(object id)` |
| `Resolve(Type, object)` | `object Resolve(Type interfaceType, object id)` |
| `ResolveAsync<TService>()` | `Task<TService> ResolveAsync<TService>()` |
| `ResolveAsync(Type)` | `Task<object> ResolveAsync(Type type)` |
| `ResolveAsync<T>(object id)` | `Task<T> ResolveAsync<T>(object id)` |
| `ResolveAsync(Type, object)` | `Task<object> ResolveAsync(Type interfaceType, object id)` |

The `id`-overloads resolve a **conditional** binding registered with `WithId(id)` (see [Attributes and Delegates](./attributes-delegates.md)). The `Async` overloads resolve synchronously and then, if the resulting instance implements `IAsyncInitializable`, `await` its `InitializeAsync()` method before returning. See [Resolution Workflow](../core/resolution-workflow.md).

### Lifecycle / Scope Members

| Member | Signature | Description |
|---|---|---|
| `Build()` | `void Build()` | Flushes pending bindings, validates the whole registry (`Registry.ValidateAll`), and invokes `IStartable.Start()` on all eagerly-instantiated singletons (and recursively on all child scopes). **Must** be called before resolving from a freshly configured container in most real usage (though `Resolve` can technically work on partially-built containers for ad-hoc/test scenarios). |
| `CreateScope()` | `IScope CreateScope()` | Creates and returns a new child `Scope`. |
| `GetChildrens()` / `GetChildrens(List<IScope>)` | — | Enumerates direct child scopes. |
| `RemoveChildren(IScope child)` | `void RemoveChildren(IScope child)` | Detaches a child scope. |
| `Parent` | `IScope Parent { get; }` | Always `null` for the root `DIContainer`. |
| `IsRoot` | `bool IsRoot { get; }` | Always `true` for `DIContainer`. |
| `Registry` | `IRegistry Registry { get; }` | Exposes the root scope's registry (read access to `ExactBindings`, `OpenGenericBindings`, `ConditionalBindings`, `DecoratorBindings`). |
| `Dispose()` / `DisposeAsync()` | — | Disposes the root scope: clears scoped instances, disposes all tracked singletons, disposes the cleanup service. |

### Configuration Import/Export

```csharp
public string ExportConfiguration();
public void ImportConfiguration(string jsonConfiguration);
```

Serializes/deserializes the entire registration graph (exact, open-generic, conditional, decorator bindings, and all child scopes recursively) to/from JSON. `ImportConfiguration` disposes the current root scope, replaces it with a freshly configured one populated from the DTO graph, and calls `Build()`. See [Serialization](../core/serialization.md) for the DTO shapes and caveats (e.g., factory delegates and live instances are **not** round-tripped faithfully for non-trivial types).

### Reachability Analysis

```csharp
public void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute);
```

Delegates to the root scope's `Registry.AnalyzeReachability`, throwing `InvalidOperationException` if there are unreachable registrations or missing bindings for non-concrete reachable types. See [Reachability Analysis and Validation](../core/reachability-analysis.md).

## `IScope` / `Scope`

Namespace: `SimplEnteiner.Core.ScopeFeature`
Source: [`Core/ScopeFeature/IScope.cs`](../../SimplEnteiner/Core/ScopeFeature/IScope.cs), [`Core/ScopeFeature/Scope.cs`](../../SimplEnteiner/Core/ScopeFeature/Scope.cs)

```csharp
public interface IScope : IDisposable, IBinder, IAsyncDisposable
{
    IScope Parent { get; }
    IRegistry Registry { get; }
    bool IsRoot { get; }

    IScope[] GetChildrens();
    void GetChildrens(List<IScope> results);
    IScope CreateScope();
    void RemoveChildren(IScope child);

    object Resolve(Type type);
    T Resolve<T>();
    Task<object> ResolveAsync(Type type);
    Task<T> ResolveAsync<T>();
    object Resolve(Type type, object id);
    T Resolve<T>(object id);
    Task<object> ResolveAsync(Type type, object id);
    Task<T> ResolveAsync<T>(object id);

    void Install(IInstaller installer);
    void Build();
}
```

`IScope` extends `IBinder` (see [Binder API](./binder.md)), so every scope — root or child — exposes the same `Bind`/`Decorate`/`BindConvention` fluent surface as `DIContainer`.

`Scope` is the concrete, publicly-visible implementation class (used directly for child scopes; `DIContainer` wraps a root `Scope` internally). Key internal state:

- `IRegistry _registry` — this scope's own bindings (parent registrations are consulted separately by walking `Parent` chains).
- `IRepositoryService _singletons` — **shared with the root scope** across the whole tree (singletons live process/container-wide, not per-scope).
- `Dictionary<Type, object> _scopedInstances` — **local to this scope**; `Scoped`-lifetime instances are stored here and never shared with parent/children.
- `List<IScope> _childrens` — direct children, used for cascading `Build()`, `AnalyzeReachability`, and disposal notification.
- `ICleanupService _cleanupService` — tracks `Transient`/`Cached` disposables created within this scope for disposal when the scope itself is disposed.

### Creating and Disposing Child Scopes

```csharp
using IScope child = container.CreateScope();
var service = child.Resolve<IMyScopedService>();
// child.Dispose() at end of `using` releases Scoped + Transient/Cached instances created in this scope
```

Disposing a non-root scope calls `Parent.RemoveChildren(this)` to detach itself from the tree; disposing the root scope instead clears and disposes the (tree-wide) singleton repository.

### Internal Members (not part of the public contract)

`Scope` exposes several `internal` members used by `Resolver`/`Registry`/`DIContainer` but not intended for direct external use: `AddRegister`, `GetAllExactRegistration`, `GetAllOpenGenericRegistration`, `FindExactRegistration`, `FindOpenGenericRegistration`, `FindConditionalRegistration`, `GetSingleton`, `GetScoped`, `GetDecoratorRegistrations`, `StoreSingleton`, `StoreScoped`, `TrackDisposable`, `InitializeFromDto`, `ValidateAll`, `Start`. These are documented here for maintainers; see the [source file](../../SimplEnteiner/Core/ScopeFeature/Scope.cs) for full signatures.

## `ScopeCreationConfig`

Namespace: `SimplEnteiner.Core.ScopeFeature`
Source: [`Core/ScopeFeature/ScopeCreationConfig.cs`](../../SimplEnteiner/Core/ScopeFeature/ScopeCreationConfig.cs)

A small mutable configuration object shared by every scope in a tree, holding the pluggable strategy instances:

```csharp
public sealed class ScopeCreationConfig
{
    public IResolver Resolver { get; set; }
    public IRegistryFactory RegistryFactory { get; set; }
    public IScopeFactory ScopeFactory { get; set; }
    public IRepositoryService SingletonRepository { get; set; }
}
```

All setters throw `ArgumentNullException` (via `ThrowIfArgumentNull`) if assigned `null`. `DIContainer` populates this once (in `ConfigureConfig`) with the built-in default implementations and passes the **same instance** down to every child scope, which is how singletons remain shared tree-wide while registries stay independent per-scope.

Continue to [Binder API](./binder.md).
