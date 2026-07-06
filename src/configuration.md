# Configuration and Settings

SimplEnteiner is a **code-first, no-external-configuration-file** library. There is no `appsettings.json`-style configuration surface, no environment-variable binding, and no `IOptions<T>`-style configuration section support built into the core container itself. All configuration of the container's behavior happens **in code**, through:

1. The fluent binder API (`Bind`, `Decorate`, `BindConvention`) — see [Binder API](./api/binder.md) and [Convention-Based Binding](./api/convention-binding.md).
2. The `ScopeCreationConfig` object, which lets you override the pluggable strategy implementations (`IResolver`, `IRegistryFactory`, `IScopeFactory`, `IRepositoryService`) — see below.
3. The JSON-based export/import feature (`ExportConfiguration`/`ImportConfiguration`), which is the closest thing to a "configuration file" the library supports, but it operates on a **serialized snapshot of an already-built binding graph**, not a hand-authored settings format — see [Configuration Import/Export (Serialization)](./core/serialization.md).

## Overriding Default Strategies via `ScopeCreationConfig`

Source: [`Core/ScopeFeature/ScopeCreationConfig.cs`](../SimplEnteiner/Core/ScopeFeature/ScopeCreationConfig.cs)

```csharp
public sealed class ScopeCreationConfig
{
    public IResolver Resolver { get; set; }
    public IRegistryFactory RegistryFactory { get; set; }
    public IScopeFactory ScopeFactory { get; set; }
    public IRepositoryService SingletonRepository { get; set; }
}
```

`DIContainer` wires up the built-in defaults internally (`ConfigureConfig`):

```csharp
private void ConfigureConfig(ScopeCreationConfig config)
{
    config.Resolver = _resolver;                                     // new Resolver()
    config.ScopeFactory = new ScopeFactory.DefaultScopeFactory();
    config.RegistryFactory = new RegistryFactory();
    config.SingletonRepository = new RepositoryService.RepositoryService(new CleanupService());
}
```

There is currently **no public constructor overload on `DIContainer` that accepts a custom `ScopeCreationConfig`** — the `internal DIContainer(ScopeConfig rootScopeConfig)` constructor exists only for the deserialization path (`Scope.Serializer.Deserialize`). Consumers who need to substitute a custom `IResolver`/`IRegistryFactory`/`IScopeFactory`/`IRepositoryService` implementation currently must either:

- Construct a `Scope` directly (`new Scope(Action<ScopeCreationConfig> configure)`), bypassing `DIContainer`, or
- Extend/modify the library source, since `DIContainer`'s `ConfigureConfig` method is `private`.

This is a known extensibility gap — see [Conclusion → Roadmap](./conclusion.md).

## "Settings" Expressed as Bindings

Because there's no dedicated settings/options subsystem, application configuration values are typically injected the same way as any other dependency — bind a configuration object as a singleton instance:

```csharp
var appConfig = new AppConfig
{
    ConnectionString = Environment.GetEnvironmentVariable("APP_CONNECTION_STRING"),
    MaxRetries = 3,
};

container.Bind<AppConfig>().ToInstance(appConfig).AsSingle().Apply();
```

Anything implementing `IInitializable`/`IAsyncInitializable` (see [Lifecycle Interfaces and Enums](./api/lifecycle.md)) can be used to perform post-construction configuration loading/validation logic (e.g., reading from environment variables or a file at first resolution), keeping such logic decoupled from the container itself.

## Overriding Defaults Summary

| What | How |
|---|---|
| Which implementation is used for an interface | `Bind<TInterface>().To<TImplementation>()` |
| Lifetime of a binding | `.AsSingle()` / `.AsTransient()` / `.AsScoped()` / `.AsCached()` |
| Multiple bindings for the same interface | `.WithId(id)` + `[Id(id)]`, or `.WhenInjectedInto<T>()` |
| Constructor selection when multiple public constructors exist | `[Inject]` on exactly one constructor |
| Extra constructor arguments not resolved from the container | `.WithArguments(...)` |
| Post-construction hooks | `IInitializable`, `IAsyncInitializable`, `IStartable`, `.OnActivation(...)`, `.OnRelease(...)` |
| Bulk registration by convention | `BindConvention(...)` — namespace/attribute/predicate filters |
| Cross-cutting wrapping | `Decorate<T>().With<TDecorator>(order).AsX()` |

## Logging Configuration

SimplEnteiner does **not** integrate with `Microsoft.Extensions.Logging` or `ILogger` in any way — there is no built-in logging output for binding/resolution activity anywhere in the codebase (confirmed: no references to `ILogger`, `Microsoft.Extensions.Logging`, or `Console.Write*` exist in the `SimplEnteiner` project). See [Error Handling and Logging](./error-handling.md) for how to add your own diagnostics using the available extension points (`OnActivation`/`OnRelease`, `IInitializable`).

Continue to [Dependencies and NuGet Packages](./dependencies.md).
