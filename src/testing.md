# Testing and Quality Assurance

## Test Framework

The [`SimplEnteinerTests`](../SimplEnteinerTests) project uses **xUnit 2.5.3** (`xunit`, `xunit.runner.visualstudio`), targeting `net8.0`, with `coverlet.collector` wired in for code coverage collection. There is no NUnit or MSTest usage anywhere in the solution. See [Dependencies and NuGet Packages](./dependencies.md#simplenteinertests-test-project) for exact package versions.

## Test Project Structure

```
SimplEnteinerTests/
├── TypeAnalyzesTests/          Unit + integration tests for every TypeAnalyzes method
│   ├── IntegrationTests.cs     Cross-method integration scenarios (see below)
│   ├── ClearCacheTests.cs
│   ├── CanResolveAllDependenciesTests.cs
│   ├── GetMemberDependencyTypeTests.cs
│   ├── IsOptionalParameterTests.cs
│   ├── GetMarkedConstructorsTests.cs
│   ├── SatisfiesClosedGenericConstraints.cs
│   ├── SatisfiesOpenedGenericConstraintsTests.cs
│   ├── GetUnderlyingDependencyTypeTests.cs
│   ├── AddAssembliesTests.cs
│   ├── HasCyclicDependenciesTests.cs
│   ├── GetAllDependenciesTests.cs
│   ├── MatchesGenericParametersTests.cs
│   ├── GetInjectableMembersTests.cs
│   ├── GetFactoryMethodTests.cs
│   ├── GetDependencyTypeTests.cs
│   ├── GetInjectableConstructorTests.cs
│   ├── GetTypesWithAttributeTests.cs
│   ├── GetAssignableToGenericArgumentsTests.cs
│   ├── IsAssignableToGenericTypeDefinitionTests.cs
│   ├── IsConcreteClassTests.cs
│   ├── GetLoadableTypesTests.cs
│   ├── FindAllAssignableFromWithConditionTests.cs
│   ├── FindAllNonAbstractClassAssignableFrom.cs
│   └── FindAllAssignableFromTests.cs
├── TestInfrastructure/
│   └── TypeExtensionsTestBase.cs   Shared base class resetting TypeAnalyzes' static caches between tests via reflection
└── TestTypes/                      Purpose-built fixture types used across many tests
    ├── Classes/                    ~20 files: simple services, generics, decorators, cyclic dependencies,
    │                                disposal-tracking classes, compiler-generated types, statics, abstracts, etc.
    ├── Interfaces/
    ├── Structs/
    ├── AtributeTypes/               Test attributes + types decorated with them
    ├── IsAndGetAssignableTypes/      Types for generic-assignability tests
    ├── WithConstraints/              Types exercising generic constraint satisfaction
    ├── Delegates.cs
    ├── Enums.cs
    └── Attributes.cs
```

## Test Categories

| Category | Location | What it covers |
|---|---|---|
| **Unit tests** | `TypeAnalyzesTests/*Tests.cs` (one file per public `TypeAnalyzes` method, e.g. `GetInjectableConstructorTests.cs`, `HasCyclicDependenciesTests.cs`) | Each public method of the [`TypeAnalyzes`](./api/type-analyzes.md) reflection toolkit is tested in isolation against purpose-built fixture types in `TestTypes/`. |
| **Integration tests** | `TypeAnalyzesTests/IntegrationTests.cs` | Exercises **combinations** of `TypeAnalyzes` methods together (e.g., `FindAllAssignableFrom` + `IsConcreteClass`, `GetInjectableConstructor` + `GetFactoryMethod`, `AddAssemblies` + `FindAllAssignableFrom`, cyclic dependency detection combined with `GetAllDependencies`), verifying that the caching layer and cross-method contracts hold together correctly under realistic usage patterns. See [`IntegrationTests.cs`](../SimplEnteinerTests/TypeAnalyzesTests/IntegrationTests.cs). |
| **Fixture/test-data types** | `TestTypes/**` | Not tests themselves, but a curated library of classes/interfaces/structs/enums/attributes/delegates designed to exercise specific reflection edge cases: multiple constructors, optional parameters, cyclic dependencies (`ForCyclicDependicies.cs`), generic constraints (`WithConstraints/Types.cs`), disposal semantics (`FroLifetimeDisposalErrors.cs` — sync/async disposables, instance/creation counters, throwing constructors), compiler-generated types, and nested type visibility. |
| **Test infrastructure** | `TestInfrastructure/TypeExtensionsTestBase.cs` | A shared abstract base class providing reflection-based helpers (`ResetDomainTypeCache`, `ResetAssembliesCache`, `GetAssembliesCache`, `GetCachedDomainTypes`, `IsInjectableConstructorsCacheEmpty`, `IsDomainTypesCacheNull`, `IsInitializedFlagFalse`) that reach into `TypeAnalyzes`'s `private static` fields via reflection to reset/inspect its caches between test runs, since the caches are static and would otherwise leak state across tests. |

Note: the current test suite is **entirely focused on the `TypeAnalyzes` reflection toolkit** (namespace `SimplEnteiner`). There are currently no dedicated test files exercising the `Core.*` namespaces directly (e.g. no `DIContainerTests.cs`, `ResolverTests.cs`, `RegistryTests.cs`, `ScopeTests.cs`, or MS.DI integration tests) — despite fixture types like `FroLifetimeDisposalErrors.cs` (disposal counters, async disposables) clearly being *prepared* for container-level lifetime/disposal testing. This is a notable coverage gap for the DI container itself; see [Conclusion → Roadmap](./conclusion.md).

## Running Tests Locally

```bash
# From the repository root, restore and run the whole test project
dotnet test SimplEnteinerTests/SimplEnteinerTests.csproj

# Or run the whole solution's tests
dotnet test SimplEnteiner.sln

# Run a single test class
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Run with code coverage (coverlet.collector is already referenced)
dotnet test --collect:"XPlat Code Coverage"
```

Because many `TypeAnalyzes` tests depend on and mutate shared static caches, tests inheriting `TypeExtensionsTestBase` call `ResetDomainTypeCache()` at the start of each test method that relies on a clean cache — this pattern should be followed for any new tests added to the same class.

## Code Coverage

`coverlet.collector` is referenced and wired for VSTest-based collection (`dotnet test --collect:"XPlat Code Coverage"` produces a Cobertura-format report under `TestResults/`). No coverage threshold, badge, or published report was found in the repository, so there is currently **no enforced minimum coverage** and no publicly tracked coverage percentage.

## CI/CD Integration

No CI/CD configuration was found in this repository: there is no `.github/workflows/` directory (only `.github/copilot-instructions.md`, which contains unrelated Azure Copilot tooling instructions, not a build pipeline) and no `azure-pipelines.yml` or other pipeline definition. Tests currently must be run manually (or wired into your own CI system) via the `dotnet test` commands above. See [Building and Deployment](./building-deployment.md) and [Conclusion → Roadmap](./conclusion.md) for suggested next steps.

Continue to [Performance and Optimization](./performance.md).
