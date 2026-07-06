# Attributes and Delegates

Namespace: `SimplEnteiner.Core.Attributes`, `SimplEnteiner.Core`
Source: [`Core/Attributes/`](../../SimplEnteiner/Core/Attributes), [`Core/Delegates.cs`](../../SimplEnteiner/Core/Delegates.cs)

## `InjectAttribute`

```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Method)]
public sealed class InjectAttribute : Attribute
{
}
```

The marker attribute the container looks for (via `Constants.InjectAttributeType = typeof(InjectAttribute)`) to identify:

- **Which constructor to use**, when a class has more than one public constructor. `TypeAnalyzes.GetInjectableConstructor` looks for exactly one constructor marked `[Inject]`; if none is marked, it falls back to the public constructor with the most parameters; if **more than one** constructor is marked `[Inject]`, an `Exception` is thrown ("Multiple constructors with InjectAttribute attribute in {type}").
- **Which fields/properties/methods to inject**, via `TypeAnalyzes.GetInjectableMembers`, which scans public and non-public instance fields, properties, and methods for the `[Inject]` attribute (`AllowMultiple = false` is the default since it's not specified, but `inherit: true` is used when checking `IsDefined`).

```csharp
public class OrderService
{
    [Inject] private readonly ILogger _logger; // field injection

    [Inject] public IClock Clock { get; set; } // property injection

    public OrderService() { }               // ignored (no [Inject], but a marked ctor exists below)

    [Inject]
    public OrderService(IRepository repository) // used, because it's marked
    {
        Repository = repository;
    }

    public IRepository Repository { get; }

    [Inject]
    public void Configure(IOptions options) // method injection — invoked once, with resolved arguments
    {
        // ...
    }
}
```

Note: because `InjectAttribute` targets `Constructor` too, it can also be applied directly to a single-constructor class without effect (there's nothing to disambiguate), but it's harmless.

## `IdAttribute`

```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public class IdAttribute : Attribute
{
    public IdAttribute(object id);
    public object Id { get; }
}
```

Applied to a **constructor parameter**, field, or property to request a specific **conditional/id-based binding** (registered via `.WithId(id)`) instead of the default binding for that type. Read by `Resolver` via `GetCustomAttribute<IdAttribute>()` on the relevant `ParameterInfo`/`FieldInfo`/`PropertyInfo`.

```csharp
container.Bind<IStorage>().To<LocalStorage>().AsSingle().WithId("local").Apply();
container.Bind<IStorage>().To<CloudStorage>().AsSingle().WithId("cloud").Apply();

public class SyncJob
{
    public SyncJob([Id("local")] IStorage source, [Id("cloud")] IStorage destination)
    {
        Source = source;
        Destination = destination;
    }

    public IStorage Source { get; }
    public IStorage Destination { get; }
}
```

Resolution for an `[Id]`-attributed member/parameter temporarily overrides `ResolutionContext.Id` for the duration of that single dependency's resolution (see `Resolver.ResolveMember`/`ResolveConstructorWithArguments`), then restores the previous value — so nested dependencies are unaffected unless they too specify their own `[Id]`.

You can also resolve an id-based binding directly, without any attribute, via:

```csharp
IStorage local = container.Resolve<IStorage>("local");
```

## `ResolverFunc` Delegate

Source: [`Core/Delegates.cs`](../../SimplEnteiner/Core/Delegates.cs)

```csharp
namespace SimplEnteiner.Core
{
    public delegate object ResolverFunc(Type interfaceType, Scope scope, object id = null);
}
```

A public delegate type matching the signature of `IResolver.Resolve(Type, Scope, object)`. It exists as a reusable delegate shape for consumers who want to pass around "a resolve operation" as a first-class value (e.g., in custom `IResolver`/`IScopeFactory` implementations), though the built-in `Resolver` class does not currently expose itself as a `ResolverFunc` instance directly.

## `CircularDependencyException`

Source: [`TypeAnalyzes.cs`](../../SimplEnteiner/TypeAnalyzes.cs) (nested type)

```csharp
public sealed class CircularDependencyException : Exception
{
    public CircularDependencyException(IEnumerable<Type> circularPath);
    public IReadOnlyList<Type> CircularPath { get; }
}
```

Thrown by `TypeAnalyzes.CanResolveAllDependencies` when `HasCyclicDependencies` detects a cycle in a type's dependency graph. The message includes the full cycle path (`A -> B -> C -> A`) for diagnostics, and `CircularPath` exposes the same list programmatically. See [Error Handling and Logging](../error-handling.md) for the full exception catalogue.

Continue to [Convention-Based Binding](./convention-binding.md).
