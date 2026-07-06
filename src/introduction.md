# Introduction

## What is SimplEnteiner?

**SimplEnteiner** is a small, self-contained Dependency Injection (DI) framework for .NET. It provides a fluent binding API (similar in spirit to Ninject/Autofac), scoped/hierarchical containers, decorator support, convention-based auto-registration, reachability validation, and a JSON-based configuration import/export mechanism — all implemented from scratch on top of plain reflection and `System.Linq.Expressions`, with no dependency on any third-party IoC container.

The library also ships an adapter that plugs a `DIContainer` instance into `Microsoft.Extensions.DependencyInjection` (`IServiceCollection` / `IServiceProvider`), so it can act as a drop-in replacement for the built-in ASP.NET Core / Generic Host service provider.

Source root: [`SimplEnteiner/`](../SimplEnteiner)
Solution file: [`SimplEnteiner.sln`](../SimplEnteiner.sln)

## Problem Domain

Most .NET applications need some form of Inversion of Control container to:

- Decouple interfaces from implementations.
- Manage object lifetimes (transient / singleton / scoped / cached).
- Compose object graphs automatically via constructor/property/field injection.
- Support cross-cutting concerns via the decorator pattern.
- Validate, at startup, that the whole dependency graph can actually be resolved (fail fast instead of failing at run time, deep in a call stack).

SimplEnteiner focuses on exactly these problems, staying deliberately small and dependency-light (`netstandard2.1`, only two NuGet dependencies), so it can be embedded anywhere from ASP.NET Core services to Unity-style game engines (the code base contains explicit comments about IL2CPP compatibility, see [`TypeAnalyzes.GetFactoryMethod`](../SimplEnteiner/TypeAnalyzes.cs)).

## Target Audience

- .NET library/application developers who want a lightweight, embeddable DI container without pulling in a large third-party dependency.
- Developers who need **hierarchical scopes** (root container → child scopes → grandchild scopes) with proper singleton/scoped/cached lifetime semantics.
- Teams that want **compile-time-like safety** for their dependency graphs via `Build()`/`AnalyzeReachability` validation before the application starts serving requests.
- Consumers of `Microsoft.Extensions.DependencyInjection`-based hosting (ASP.NET Core, Generic Host, Worker Services) who want to replace the built-in container with SimplEnteiner while keeping the same `IServiceProvider`/`IServiceScope` abstractions.
- Engine/runtime authors (e.g., game engines) who need a reflection-based container that can also work without JIT compilation of expression trees (fallback path in `GetFactoryMethod`).

## Technology Stack

| Aspect | Value |
|---|---|
| Main library target framework | `netstandard2.1` (see [`SimplEnteiner.csproj`](../SimplEnteiner/SimplEnteiner.csproj)) |
| Test project target framework | `net8.0` (see [`SimplEnteinerTests.csproj`](../SimplEnteinerTests/SimplEnteinerTests.csproj)) |
| Language | C# (nullable-annotated in parts, e.g. `TypeAnalyzes.cs`) |
| Supported OS | Any OS supported by a `netstandard2.1`-compatible runtime (Windows, Linux, macOS) — no OS-specific APIs are used |
| Direct NuGet dependencies | `Microsoft.Extensions.DependencyInjection` 10.0.7, `System.Text.Json` 10.0.7 |
| Test framework | xUnit 2.5.3 |
| Documentation | mdBook (this book), Mermaid diagrams |

Because the core library targets `netstandard2.1`, it can be consumed from .NET 6/7/8/9/10 applications, as well as from any other runtime implementing `netstandard2.1` (e.g. Mono, Unity with the appropriate API compatibility level).

## Key Features and Differentiators

- **Fluent, staged binding API** — `Bind<T>().To<Impl>().AsSingle().WithId(...).WhenInjectedInto<T>().WithArguments(...).Apply()`. The staged builder (see [Architecture → Design Patterns](./architecture/design-patterns.md)) enforces a valid call order at run time via a small internal state machine.
- **Hierarchical scopes** — `IScope.CreateScope()` creates a child scope that inherits parent registrations, with correct singleton (root-only), scoped (per-scope) and cached (per-resolution-context) lifetime semantics.
- **Four lifetimes**: `Transient`, `Singleton`, `Scoped`, `Cached` (see [`LifeTime`](../SimplEnteiner/Core/Lifecycle/LifeTime.cs)).
- **Decorators** with explicit ordering, including open-generic decorators for open-generic services.
- **Conditional bindings** — `WithId(...)` and `WhenInjectedInto<T>()` allow multiple registrations of the same interface, resolved by consumer type or an explicit id (via `[Id]` attribute on constructor parameters/fields/properties).
- **Convention-based / assembly scanning binding** via `BindConvention(...)` and `IConventionBuilder` (filter by namespace, attribute, predicate; bind to interfaces and/or self).
- **Constructor, field, property and method injection**, with an explicit `[Inject]` attribute, `[Id]`-attribute-based disambiguation, and support for `IEnumerable<T>`, `Lazy<T>`, and `Func<T>` "meta" dependencies.
- **Cyclic-dependency detection** (`TypeAnalyzes.HasCyclicDependencies`) and **reachability analysis** (`Registry.AnalyzeReachability` / `ReachabilityAnalyzer`) that can be run at container `Build()` time to fail fast on invalid graphs.
- **Object lifecycle hooks**: `IInitializable`, `IAsyncInitializable`, `IStartable`, plus `OnActivation`/`OnRelease` callbacks per binding.
- **JSON export/import of the whole binding configuration** (`DIContainer.ExportConfiguration()` / `ImportConfiguration()`), enabling serialization of a container's registration graph.
- **First-class `Microsoft.Extensions.DependencyInjection` interop** via `AddSimplEnteiner(...)`, `SimplEnteinerServiceProvider`, and `SimplEnteinerServiceScope`.
- **Heavily cached reflection layer** (`TypeAnalyzes`) with a rich set of general-purpose type-introspection extension methods (open/closed generics, generic constraint satisfaction, attribute scanning, etc.) that is reusable independently of the container itself.
- **No third-party IoC dependency** — the only external packages are `Microsoft.Extensions.DependencyInjection` (for interoperability, not for actual container logic) and `System.Text.Json` (for the serialization feature).

Continue to [Architecture and Design](./architecture/overview.md) for a deep dive into how these pieces fit together, or jump straight to the [API Reference](./api/container-and-scope.md) for concrete usage.
