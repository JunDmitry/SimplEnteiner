# Registration and Resolution

## `IRegistry` / `Registry`

Namespace: `SimplEnteiner.Core.RegistrationService`
Source: [`Core/RegistrationService/IRegistry.cs`](../../SimplEnteiner/Core/RegistrationService/IRegistry.cs), [`Core/RegistrationService/Registry.cs`](../../SimplEnteiner/Core/RegistrationService/Registry.cs)

```csharp
public interface IRegistry
{
    IReadOnlyDictionary<ConditionalKey, Registration> ConditionalBindings { get; }
    IReadOnlyDictionary<Type, List<DecoratorRegistration>> DecoratorBindings { get; }
    IReadOnlyDictionary<Type, Registration> ExactBindings { get; }
    IReadOnlyDictionary<Type, Registration> OpenGenericBindings { get; }

    void Add(BindingBuilder bindingBuilder);
    void AddConditionalRegistration(Type key, object id, Registration value);
    void AddDecorator(DecoratorRegistration registration);
    void AddExactRegistration(Type key, Registration value);
    void AddOpenGenericRegistration(Type key, Registration value);
    void AnalyzeReachability(IEnumerable<Type> roots, Type injectAttribute);
    bool CanResolveAllDependencies(Type type, Type injectAttribute);
    bool CanResolveGeneric(Type interfaceType);
    void ValidateAll();
}
```

Every `Scope` owns exactly one `IRegistry` (created via `IRegistryFactory`). It is a per-scope store split into four buckets:

| Bucket | Key | Populated when |
|---|---|---|
| `ExactBindings` | `Type` (interface/service type, non-open-generic, no condition/id) | `Bind<T>()...Apply()` for a closed type without `WithId`/`WhenInjectedInto` |
| `OpenGenericBindings` | `Type` (open generic definition, e.g. `typeof(IRepository<>)`) | `Bind(typeof(IRepository<>))...Apply()` |
| `ConditionalBindings` | `ConditionalKey` (`(Type interfaceType, object id)`) | `WithId(id)` or `WhenInjectedInto<T>()` (using the consumer `Type` as the id) |
| `DecoratorBindings` | `Type` (interface being decorated) → `List<DecoratorRegistration>` | `Decorate<T>().With<TDecorator>().AsX()` |

### `Registry.Add(BindingBuilder)`

Called by `Scope`/`DIContainer` when a binding's staged pipeline completes. It:

1. Validates the builder (`Validate`): the implementation must be a concrete, injectable class (`IsConcreteClass(isIgnoreGeneratedType: true)`), assignable/compatible with the interface (handling exact, open-generic-definition, and closed-generic-with-constraints cases), and must expose a resolvable constructor (`GetInjectableConstructor`).
2. Builds a `Registration` (compiling a factory delegate via `GetFactoryMethod`, unless a custom `FactoryMethod` was supplied).
3. Routes the registration into the correct bucket based on whether `ConditionType`/`Id` is set, or whether `InterfaceType.IsGenericTypeDefinition`.

### `Registry.ValidateAll()`

Called from `Scope.Build()` → `Scope.ValidateAll()`. Iterates all `ExactBindings` and throws `InvalidOperationException` if any registered type's dependency graph cannot be fully resolved against the currently-known exact bindings (via `TypeAnalyzes.CanResolveAllDependencies`, which also detects cycles and throws `TypeAnalyzes.CircularDependencyException` if one is found). Also validates that every open-generic implementation is concrete and has an injectable constructor.

### `Registry.AnalyzeReachability(roots, injectAttribute)`

Computes, via [`ReachabilityAnalyzer`](#reachabilityanalyzer), the set of types reachable (via constructor/member dependencies) starting from `roots`. Then:

- **Unreachable services** — registered exact bindings that are never referenced from any root — are reported.
- **Missing bindings** — reachable, non-concrete types that have no exact binding — are reported.

If either list is non-empty, throws `InvalidOperationException` with a combined message. See [Reachability Analysis and Validation](../core/reachability-analysis.md).

## `Registration`

Namespace: `SimplEnteiner.Core.RegistrationService`
Source: [`Core/RegistrationService/Registration.cs`](../../SimplEnteiner/Core/RegistrationService/Registration.cs)

```csharp
public class Registration
{
    public Registration(Type implementation, LifeTime lifetime, Func<object[], object> factory, object instance, object[] arguments = null);

    public Type Implementation { get; }
    public LifeTime Lifetime { get; }
    public Func<object[], object> Factory { get; }
    public object Instance { get; }
    public object[] Arguments { get; }
    public Action<object> OnActivation { get; set; }
    public Action<object> OnRelease { get; set; }
}
```

An immutable (except for the two `Action` callbacks) descriptor of "how to make an instance": either a pre-existing `Instance`, or a `Factory` delegate to invoke with resolved constructor arguments.

## `DecoratorRegistration`

Namespace: `SimplEnteiner.Core.RegistrationService`
Source: [`Core/RegistrationService/DecoratorRegistration.cs`](../../SimplEnteiner/Core/RegistrationService/DecoratorRegistration.cs)

```csharp
public class DecoratorRegistration
{
    public DecoratorRegistration(Type interfaceType, Type decoratorType, int? order, LifeTime lifetime, ConstructorInfo constructor, Func<object[], object> factory);

    public Type InterfaceType { get; }
    public Type DecoratorType { get; }
    public int? Order { get; set; }
    public LifeTime Lifetime { get; }
    public ConstructorInfo Constructor { get; }
    public Func<object[], object> Factory { get; }
}
```

See [Decorators](../core/decorators.md) for how these are resolved and ordered.

## `IRegistryFactory` / `RegistryFactory`

Source: [`Core/RegistrationService/Factory/IRegistryFactory.cs`](../../SimplEnteiner/Core/RegistrationService/Factory/IRegistryFactory.cs)

```csharp
public interface IRegistryFactory
{
    IRegistry CreateRegistry();
}
```

Pluggable strategy for creating a fresh `IRegistry` per scope; `RegistryFactory.CreateRegistry()` simply returns `new Registry()`.

## `IResolver` / `Resolver`

Namespace: `SimplEnteiner.Core.ResolverService`
Source: [`Core/ResolverService/IResolver.cs`](../../SimplEnteiner/Core/ResolverService/IResolver.cs), [`Core/ResolverService/Resolver.cs`](../../SimplEnteiner/Core/ResolverService/Resolver.cs)

```csharp
public interface IResolver
{
    object Resolve(Type interfaceType, Scope scope, object id = null);
    T Resolve<T>(Scope scope, object id = null);
}
```

The `Resolver` is the algorithmic heart of the library. See [Core Functionality → Resolution Workflow](../core/resolution-workflow.md) for a complete, step-by-step description of how a single `Resolve` call is handled, including:

- Generic wrapper resolution (`IEnumerable<T>`, `Lazy<T>`, `Func<T>`).
- Registration lookup order (conditional by id → conditional by consumer type → exact → closed-generic-from-open-generic → self-registration-if-concrete-class).
- Constructor/member injection, including `[Id]`-attribute-based per-parameter conditional resolution.
- Lifetime-specific instance storage/retrieval (`GetExistingInstance`/`StoreInstance`).
- Decorator wrapping (`ResolveDecorators`).

## `ConditionalKey`

Namespace: `SimplEnteiner.Core.ScopeFeature`
Source: [`Core/ScopeFeature/ConditionalKey.cs`](../../SimplEnteiner/Core/ScopeFeature/ConditionalKey.cs)

```csharp
public struct ConditionalKey
{
    public Type interfaceType;
    public object id;
    // value-equality, deconstruction, and implicit conversions to/from (Type, object) tuples
}
```

A lightweight value-type key used by `ConditionalBindings`. Supports implicit conversion to/from `(Type interfaceType, object id)` tuples for ergonomic dictionary indexing (`_conditionalBindings[(interfaceType, id)] = registration;`).

## `ResolutionContext` (internal)

Namespace: `SimplEnteiner.Core.ScopeFeature`
Source: [`Core/ScopeFeature/ResolutionContext.cs`](../../SimplEnteiner/Core/ScopeFeature/ResolutionContext.cs)

Not part of the public API, but important to understand for maintainers: a short-lived, `IDisposable`, per-`Resolve()`-call object carrying:

- `CurrentScope` — the `Scope` the resolution started from.
- `Id` — the currently active conditional id (temporarily swapped while resolving nested `[Id]`-attributed parameters/members).
- `RequestType` — the "who is asking" type, used for `WhenInjectedInto<T>()` conditional matching; temporarily swapped to the type currently being constructed while resolving its dependencies.
- `CachedInstances` — a `ConcurrentDictionary<Type, object>` used to implement the `Cached` lifetime **within a single top-level `Resolve()` call** (see [Scopes, Lifetimes and Disposal](../core/scopes-and-lifetimes.md)).

Continue to [Lifecycle Interfaces and Enums](./lifecycle.md).
