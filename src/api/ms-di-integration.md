# MS.DI Integration API

Namespace: `SimplEnteiner.Integrations.MS_DI`
Source: [`Integrations/MS_DI/`](../../SimplEnteiner/Integrations/MS_DI)

This page is the API-reference companion to the architectural discussion in [Architecture → Dependency Injection Integration](../architecture/di-integration.md). See that page for behavior/sequence diagrams; this page focuses on exact members and signatures.

## `Extensions.AddSimplEnteiner`

```csharp
public static class Extensions
{
    public static IServiceCollection AddSimplEnteiner(this IServiceCollection services, Action<DIContainer> configure);
}
```

| Parameter | Description |
|---|---|
| `services` | The `IServiceCollection` to augment with a `SimplEnteiner`-backed `IServiceProvider`/`IServiceScopeFactory`. |
| `configure` | Callback invoked synchronously with a brand-new `DIContainer` before it is built. Throws `ArgumentNullException` if `null` (via `ThrowIfArgumentNull`). |

Returns the same `services` instance (fluent chaining). See [source](../../SimplEnteiner/Integrations/MS_DI/Extensions.cs).

## `SimplEnteinerServiceProvider`

```csharp
public class SimplEnteinerServiceProvider : IServiceProvider, ISupportRequiredService, IServiceScopeFactory
{
    public SimplEnteinerServiceProvider(IScope container);

    public object GetService(Type serviceType);
    public object GetRequiredService(Type serviceType);
    public IServiceScope CreateScope();
}
```

| Member | Behavior |
|---|---|
| `GetService(Type)` | `_container.Resolve(serviceType)` — propagates any exception thrown by SimplEnteiner's resolution pipeline (does not swallow to `null`). |
| `GetRequiredService(Type)` | Same as `GetService`, but throws `InvalidOperationException` if the result is `null` (note: SimplEnteiner's own resolver already throws for unregistered non-concrete types, so this null-check mainly guards against services intentionally registered/resolved as `null` instance, an edge case). |
| `CreateScope()` | `_container.CreateScope()` wrapped in a new `SimplEnteinerServiceScope`. |

Constructed directly around **any** `IScope` (not just the root `DIContainer`), so it can also be used to expose an arbitrary child scope as a standalone `IServiceProvider`.

## `SimplEnteinerServiceScope`

```csharp
public class SimplEnteinerServiceScope : IServiceScope
{
    public SimplEnteinerServiceScope(IScope scope);

    public IServiceProvider ServiceProvider { get; }
    public void Dispose();
}
```

Wraps a child `IScope` in a fresh `SimplEnteinerServiceProvider` exposed via `ServiceProvider`. `Dispose()` disposes the `ServiceProvider` if it implements `IDisposable` (which `SimplEnteinerServiceProvider` currently does **not** implement explicitly — disposal of the underlying scope must currently be managed by disposing the `IScope` passed to the constructor directly, or by relying on parent-scope disposal cascading). Consumers who need deterministic disposal of the underlying `IScope` when the MS.DI `IServiceScope` is disposed should keep a reference to the `IScope` and dispose it explicitly, or track this as a potential enhancement (see [Conclusion → Roadmap](../conclusion.md)).

## Full Example: ASP.NET Core Minimal API

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSimplEnteiner(container =>
{
    container.Bind<IGreetingService>().To<GreetingService>().AsSingle().Apply();
});

var app = builder.Build();

app.MapGet("/hello", (IServiceProvider sp) =>
{
    var greeter = (IGreetingService)sp.GetRequiredService(typeof(IGreetingService));
    return greeter.Greet();
});

app.Run();
```

> Note: Whether ASP.NET Core's own request pipeline actually resolves controllers/minimal-API handlers through the `IServiceProvider` registered by `AddSimplEnteiner` depends on how/whether the host's default service provider factory is overridden (e.g. via a custom `IServiceProviderFactory<TContainerBuilder>`), which SimplEnteiner does not currently supply out of the box. `AddSimplEnteiner` only adds the provider/factory as services *within* the default container — see the caveats in [Architecture → DI Integration](../architecture/di-integration.md#limitations-of-the-current-integration).

Continue to [Core Functionality → Binding Workflow](../core/binding-workflow.md).
