# Contributing

No `CONTRIBUTING.md`, issue/PR templates, or code-of-conduct file currently exist in this repository. This page documents the conventions **observable directly from the existing codebase** to guide contributors until a formal contributing guide is authored.

## Code Style and Conventions (Observed)

- **Braces**: Allman style (opening brace on its own line) throughout the codebase — see any file, e.g. [`DIContainer.cs`](../SimplEnteiner/Core/DIContainer.cs).
- **Naming**:
  - Interfaces prefixed with `I` (`IScope`, `IBinder`, `IRegistry`, `IResolver`, ...).
  - Private fields prefixed with `_` (`_pendingBindings`, `_registry`, `_singletons`).
  - Private **static** fields prefixed with `s_` (`s_injectableConstructorsCache`, `s_lock`, `s_assembliesCache`).
  - `Internal`/implementation classes typically match their public interface name without the `I` prefix (`IBinder` → no direct class; `IRegistry` → `Registry`; `IResolver` → `Resolver`).
- **Visibility discipline**: Types not meant for direct consumer use are consistently marked `internal` (all of `Binder.Implementations`, `Binder.BuilderStages`, `Registry`, `Resolver`, `RepositoryService`, `CleanupService`, `InterfaceInvoker`, `RegistryFactory`, `DefaultScopeFactory`). See [Architecture → Namespace Structure → Visibility Conventions](./architecture/namespaces.md#visibility-conventions) — follow this pattern for new code: default to `internal`, and only make something `public` when it's part of the intended extension surface.
- **Argument validation**: Guard clauses are expressed via the small extension-method helpers in [`Utilities/ThrowExtensions.cs`](../SimplEnteiner/Utilities/ThrowExtensions.cs) (`ThrowIfArgumentNull`, `ThrowInvalidIfNull`, `ThrowIfInvalidOperation`) rather than repeated inline `if (x == null) throw ...` blocks. Prefer these helpers for consistency in new code.
- **XML doc comments**: Public API surfaces in [`TypeAnalyzes.cs`](../SimplEnteiner/TypeAnalyzes.cs) are thoroughly documented with `<summary>`, `<param>`, `<returns>`, and `<exception>` tags. This standard is **not** uniformly applied across `Core/*` (many public interfaces/classes there currently have no XML doc comments) — new public API additions should follow the `TypeAnalyzes.cs` standard of complete XML documentation.
- **Nullable reference types**: Enabled in the test project (`<Nullable>enable</Nullable>` in [`SimplEnteinerTests.csproj`](../SimplEnteinerTests/SimplEnteinerTests.csproj)) but **not** enabled in the main library project ([`SimplEnteiner.csproj`](../SimplEnteiner/SimplEnteiner.csproj) has no `<Nullable>` setting, defaulting to disabled for its `netstandard2.1` target). `TypeAnalyzes.cs` nonetheless uses nullable annotations in a few return types (`ConstructorInfo?`) despite the project-level setting being off — be consistent with the surrounding file's existing style when editing.
- **File-scoped organization**: One primary type per file, file name matching the type name (e.g. `IBindingTo.cs` contains both `IBindingTo<TInterface>` and `IBindingTo`, `IBindingLifetime.cs` contains both generic and non-generic variants) — generic/non-generic interface pairs are consistently co-located in a single file.
- **Partial classes** are used to split a single logical type across files by concern (`TypeAnalyzes.cs` + `TypeAnalyzes.TypeCondition.cs`).

## File Size Convention

This repository's local rules mandate that **no code file exceed 200 lines**. Most existing files comply, but a few exceed this (notably `TypeAnalyzes.cs` at over 1000 lines and `Scope.cs`/`Resolver.cs` at 400-600+ lines) — these predate the convention or represent an accepted exception for the reflection toolkit and core resolution engine, given their inherently large, closely-related method sets. New contributions should still aim to keep individual files under 200 lines where reasonably possible, favoring further decomposition (e.g., more `partial class` splits, or extracting cohesive private helper methods into separate internal utility classes) over growing existing large files further.

## Setting Up the Development Environment

```bash
git clone <repository-url>
cd SimplEnteiner
dotnet restore SimplEnteiner.sln
dotnet build SimplEnteiner.sln
dotnet test SimplEnteinerTests/SimplEnteinerTests.csproj
```

No additional environment variables, local secrets, or external services are required to build or test this repository — it is a fully self-contained, dependency-light .NET solution. See [Building and Deployment](./building-deployment.md) for full prerequisites.

## Adding Tests for New Features

Given the current [test coverage gap](./testing.md) around the `Core.*` namespaces (container/binder/resolver/scope behavior is not yet directly unit-tested — only `TypeAnalyzes` is), contributions that add or modify `Core.*` behavior are strongly encouraged to include corresponding tests, following the existing `SimplEnteinerTests` project structure:

- Add fixture types under `SimplEnteinerTests/TestTypes/` rather than inline in test files, matching the existing pattern.
- Reset any relevant static caches at the start of tests (see [`TypeExtensionsTestBase`](../SimplEnteinerTests/TestInfrastructure/TypeExtensionsTestBase.cs)) if your test touches `TypeAnalyzes`'s cached state.
- Prefer integration-style tests (like [`IntegrationTests.cs`](../SimplEnteinerTests/TypeAnalyzesTests/IntegrationTests.cs)) when validating end-to-end container behavior (bind → build → resolve → dispose), in addition to focused unit tests for individual methods.

## Pull Request Process

No formal PR template or review process is documented in the repository. Until one is established, contributors should follow standard good practice:

1. Open an issue or discussion describing the change before large refactors.
2. Keep pull requests focused on a single logical change.
3. Ensure `dotnet build` and `dotnet test` both succeed locally before submitting.
4. Update this documentation (`src/`) alongside any change to public API surface, behavior, or configuration — per this project's own repository rule that documentation in `docs`/`src` must be kept in sync with code changes.
5. Match the existing code style conventions documented above.

Continue to [Conclusion](../conclusion.md).
