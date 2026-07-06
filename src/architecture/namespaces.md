# Namespace Structure and Module Organisation

All production code lives under the [`SimplEnteiner`](../../SimplEnteiner) project. The table below maps namespaces to folders and summarizes their responsibility.

| Namespace | Folder | Responsibility |
|---|---|---|
| `SimplEnteiner` | [`SimplEnteiner/`](../../SimplEnteiner) (root, `TypeAnalyzes.cs`, `TypeAnalyzes.TypeCondition.cs`) | Standalone reflection/type-introspection toolkit (`TypeAnalyzes` static partial class, `TypeCondition` flags enum, `CircularDependencyException`). Usable independently of the DI container. |
| `SimplEnteiner.Analysis` | [`Analysis/`](../../SimplEnteiner/Analysis) | `ReachabilityAnalyzer` — BFS-based reachability computation over a dependency graph. |
| `SimplEnteiner.Core` | [`Core/`](../../SimplEnteiner/Core) | `DIContainer` (public entry point), `Delegates.cs` (`ResolverFunc`), `Constants.cs`, `BindExample.cs` (internal usage examples, not part of the public API contract). |
| `SimplEnteiner.Core.Attributes` | [`Core/Attributes/`](../../SimplEnteiner/Core/Attributes) | `InjectAttribute`, `IdAttribute` — mark constructors/members for injection and disambiguate conditional/id-based bindings. |
| `SimplEnteiner.Core.Binder` | [`Core/Binder/`](../../SimplEnteiner/Core/Binder) | `BindingBuilder` — mutable, staged binding descriptor. |
| `SimplEnteiner.Core.Binder.BuilderStages` | [`Core/Binder/BuilderStages/`](../../SimplEnteiner/Core/Binder/BuilderStages) | `Stage`, `InitialStage`, `ImplementationStage`, `LifetimeStage`, `OptionsStage`, `FinalStage`, `BuilderStateMachine` — internal staged validation engine (all `internal`). |
| `SimplEnteiner.Core.Binder.Interfaces` | [`Core/Binder/Interfaces/`](../../SimplEnteiner/Core/Binder/Interfaces) | Public fluent contracts: `IBinder`, `IBindingTo(<T>)`, `IBindingLifetime(<T>)`, `IBindingOptions(<T>)`, `IBindingDecorate(<T>)`, `IBindingDecorateLifetime(<T>)`, and the internal `IBindingTarget`. |
| `SimplEnteiner.Core.Binder.Implementations` | [`Core/Binder/Implementations/`](../../SimplEnteiner/Core/Binder/Implementations) | Internal concrete implementations of the interfaces above: `BindingTo(<T>)`, `BindingLifetime(<T>)`, `BindingOptions(<T>)`, `BindingDecorate(<T>)`, `BindingDecorateLifetime(<T>)`. |
| `SimplEnteiner.Core.RegistrationService` | [`Core/RegistrationService/`](../../SimplEnteiner/Core/RegistrationService) | `IRegistry`/`Registry` (registration storage & validation), `Registration`, `DecoratorRegistration`. |
| `SimplEnteiner.Core.RegistrationService.Factory` | [`Core/RegistrationService/Factory/`](../../SimplEnteiner/Core/RegistrationService/Factory) | `IRegistryFactory`/`RegistryFactory` — pluggable `Registry` creation strategy. |
| `SimplEnteiner.Core.ResolverService` | [`Core/ResolverService/`](../../SimplEnteiner/Core/ResolverService) | `IResolver`/`Resolver` — the resolution algorithm (constructor/member injection, generic wrappers, decorators, lifetime storage). |
| `SimplEnteiner.Core.ScopeFeature` | [`Core/ScopeFeature/`](../../SimplEnteiner/Core/ScopeFeature) | `IScope`/`Scope` (hierarchical container), `ScopeCreationConfig`, `ConditionalKey`, `ResolutionContext` (internal per-resolve state). |
| `SimplEnteiner.Core.ScopeFactory` | [`Core/ScopeFactory/`](../../SimplEnteiner/Core/ScopeFactory) | `IScopeFactory`/`DefaultScopeFactory` — pluggable child-`Scope` creation strategy. |
| `SimplEnteiner.Core.Lifecycle` | [`Core/Lifecycle/`](../../SimplEnteiner/Core/Lifecycle) | `LifeTime` enum, `IInitializable`, `IAsyncInitializable`, `IStartable`, internal `ICleanupService`/`CleanupService`, internal `IInterfaceInvoker`/`InterfaceInvoker`. |
| `SimplEnteiner.Core.RepositoryService` | [`Core/RepositoryService/`](../../SimplEnteiner/Core/RepositoryService) | `IRepositoryService`/`RepositoryService` — dictionary-like store for singleton instances with disposal tracking, wrapping a `Dictionary<Type,object>` behind numerous collection interfaces. |
| `SimplEnteiner.Core.ConventionBinding.Interfaces` | [`Core/ConventionBinding/Interfaces/`](../../SimplEnteiner/Core/ConventionBinding/Interfaces) | `IConventionBuilder` — fluent contract for assembly-scanning based auto-registration. |
| `SimplEnteiner.Core.ConventionBinding.Implementations` | [`Core/ConventionBinding/Implementations/`](../../SimplEnteiner/Core/ConventionBinding/Implementations) | `ConventionBuilder`, internal `ConventionBindType` flags enum. |
| `SimplEnteiner.Core.InstallerService.Interfaces` | [`Core/InstallerService/Interfaces/`](../../SimplEnteiner/Core/InstallerService/Interfaces) | `IInstaller` — a unit of registration logic that can be discovered and invoked via `ScanAndInstall`. |
| `SimplEnteiner.Core.Configuration` | [`Core/Configuration/`](../../SimplEnteiner/Core/Configuration) | Internal serializable DTOs: `ScopeConfig`, `BindingConfig`, `DecoratorConfig`, used by the export/import feature. |
| `SimplEnteiner.Utilities` | [`Utilities/`](../../SimplEnteiner/Utilities) | Cross-cutting extension methods: `ThrowExtensions` (argument guarding), `ListExtensions` (binary-search insertion for decorator ordering), `ContainerExtensions` (`ScanAndInstall`). |
| `SimplEnteiner.Integrations.MS_DI` | [`Integrations/MS_DI/`](../../SimplEnteiner/Integrations/MS_DI) | `Extensions.AddSimplEnteiner`, `SimplEnteinerServiceProvider`, `SimplEnteinerServiceScope` — Microsoft.Extensions.DependencyInjection adapter layer. |

## Visibility Conventions

- Types that are part of the intended **public extension surface** (fluent binder interfaces, `DIContainer`, `IScope`, lifecycle interfaces, attributes, `TypeAnalyzes`) are `public`.
- Types that are **implementation details** consumers should not depend on directly (`Registry`, `Resolver`, `RepositoryService`, `CleanupService`, `InterfaceInvoker`, all `Binder.Implementations` and `Binder.BuilderStages` classes) are marked `internal`.
- A small number of types are `public` because they need to flow through public interface members even though direct construction by consumers is unusual: `Registration`, `DecoratorRegistration`, `BindingBuilder`, `ConventionBuilder`.

## Test Project Structure

The [`SimplEnteinerTests`](../../SimplEnteinerTests) project mirrors this structure with an xUnit test per feature area, plus a `TestTypes/` folder containing purpose-built fixture types (interfaces, classes, structs, enums, attributes, delegates) used across many tests without polluting production code. See [Testing and Quality Assurance](../testing.md) for details.

Continue to [Dependency Injection Integration](./di-integration.md).
