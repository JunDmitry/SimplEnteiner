# Building and Deployment

## Environment Prerequisites

| Requirement | Detail |
|---|---|
| .NET SDK | An SDK capable of building `netstandard2.1` (main library) and `net8.0` (test project) — the .NET 8 SDK (or later) satisfies both, since newer SDKs can target older `TargetFramework` monikers. |
| IDE | Any IDE/editor with C#/.NET tooling (Visual Studio, Visual Studio Code with C# Dev Kit, JetBrains Rider). The solution was authored with Visual Studio 2022 (`VisualStudioVersion = 17.14...` in [`SimplEnteiner.sln`](../SimplEnteiner.sln)), but nothing in the codebase requires Visual Studio specifically. |
| Build tool | `dotnet` CLI (ships with the SDK) is sufficient for all build/test/pack operations — no additional build tooling (MSBuild-only projects, no custom `.targets`/`.props` files) is required. |
| OS | Any OS with a compatible .NET SDK — Windows, Linux, macOS all work, since no OS-specific APIs are used in the library. |

## Build Process

```bash
# Restore and build the whole solution
dotnet restore SimplEnteiner.sln
dotnet build SimplEnteiner.sln -c Release

# Build only the library
dotnet build SimplEnteiner/SimplEnteiner.csproj -c Release

# Run tests (see Testing and Quality Assurance for more detail)
dotnet test SimplEnteinerTests/SimplEnteinerTests.csproj -c Release
```

There are two configurations defined in the solution file: `Debug|Any CPU` and `Release|Any CPU` (see [`SimplEnteiner.sln`](../SimplEnteiner.sln)). No custom build configurations, `Directory.Build.props`, or MSBuild targets exist beyond the two straightforward `.csproj` files.

## Packing (`dotnet pack`)

The `SimplEnteiner.csproj` currently has **no NuGet package metadata** (`PackageId`, `Version`, `Authors`, `Description`, `PackageLicenseExpression`, etc.) — see [Dependencies and NuGet Packages → Is SimplEnteiner Published as a NuGet Package?](./dependencies.md#is-simplenteiner-published-as-a-nuget-package). Running `dotnet pack` today will still produce a `.nupkg` (since class libraries are packable by default), but with only inferred/default metadata (project name as package id, a default version like `1.0.0`, no description, no license).

```bash
# Produces a .nupkg with default/inferred metadata (not currently curated)
dotnet pack SimplEnteiner/SimplEnteiner.csproj -c Release -o ./artifacts
```

### Suggested Metadata to Add Before Publishing

To make the library production-ready for NuGet distribution, the following properties should be added to `SimplEnteiner.csproj`'s `<PropertyGroup>` (not currently present in the repository):

```xml
<PropertyGroup>
  <TargetFramework>netstandard2.1</TargetFramework>

  <PackageId>SimplEnteiner</PackageId>
  <Version>1.0.0</Version>
  <Authors>Dmitry Rysev</Authors>
  <Description>A small, embeddable Dependency Injection framework for .NET with hierarchical scopes, decorators, conditional bindings, convention-based registration, and Microsoft.Extensions.DependencyInjection interoperability.</Description>
  <PackageLicenseExpression>MIT</PackageLicenseExpression> <!-- or whichever license is chosen -->
  <RepositoryUrl>https://github.com/JunDmitry/SimplEnteiner</RepositoryUrl>
  <PackageProjectUrl>https://github.com/JunDmitry/SimplEnteiner</PackageProjectUrl>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

(The repository URL above is inferred from [`book.toml`](../book.toml)'s `git-repository-url` setting; verify against the actual canonical repository before publishing.)

## Publishing to NuGet (Not Yet Configured)

There is currently **no publish workflow** in this repository:

- No `.github/workflows/*.yml` exists (only `.github/copilot-instructions.md`, unrelated to CI/CD).
- No `dotnet nuget push` script, Azure DevOps pipeline, or any other automation for publishing to nuget.org or a private feed was found.
- No API-key management convention (secrets file, environment variable references, etc.) exists.

To set this up, a typical GitHub Actions workflow would:

```yaml
# .github/workflows/publish.yml (example — not present in the repository)
name: Publish
on:
  push:
    tags: ["v*"]
jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"
      - run: dotnet restore SimplEnteiner.sln
      - run: dotnet build SimplEnteiner.sln -c Release
      - run: dotnet test SimplEnteinerTests/SimplEnteinerTests.csproj -c Release
      - run: dotnet pack SimplEnteiner/SimplEnteiner.csproj -c Release -o ./artifacts
      - run: dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_API_KEY }}
```

### Versioning Strategy

No versioning convention (SemVer tagging, `MinVer`/`Nerdbank.GitVersioning`, changelog file) currently exists in the repository. Given the library's small, focused surface area, **Semantic Versioning (SemVer 2.0)** tied to Git tags (e.g., via `Nerdbank.GitVersioning` or `MinVer`) is recommended once packaging is set up, with breaking changes to public interfaces (`IBinder`, `IScope`, `IBindingTo<T>`, etc.) triggering a major version bump.

## Strong Naming / Assembly Signing

No `.snk` file, `<SignAssembly>`, `<AssemblyOriginatorKeyFile>`, or any other strong-naming configuration exists in `SimplEnteiner.csproj`. The assembly is **not strong-named**. If strong naming is required for your consuming scenario (e.g., GAC deployment, strict assembly-identity requirements), this would need to be added explicitly — it is not currently part of the build.

## Publishing the Documentation (mdBook / GitHub Pages)

This documentation is authored as an [mdBook](https://rust-lang.github.io/mdBook/) book, configured via [`book.toml`](../book.toml):

```toml
[book]
title = "SimplEnteiner"
authors = ["Dmitry Rysev"]
language = "en"
src = "src"

[output.html]
default-theme = "light"
preferred-dark-theme = "navy"
git-repository-url = "https://github.com/JunDmitry/SimplEnteiner"
edit-url-template = "https://github.com/JunDmitry/SimplEnteiner/edit/main/src/{path}"
additional-js = ["mermaid-init.js"]

[output.html.search]
limit-results = 30

[preprocessor.links]
```

To build and preview locally (requires the [`mdbook`](https://github.com/rust-lang/mdBook) CLI, e.g. `cargo install mdbook`):

```bash
mdbook build   # outputs to ./book by default
mdbook serve   # live-reloading local preview
```

To publish to GitHub Pages, a typical workflow (not currently present in `.github/workflows/`) would build the book and deploy the `book/` output directory via `actions/deploy-pages` or `peaceiris/actions-gh-pages`. Mermaid diagram rendering is already wired up via [`theme/head.hbs`](../theme/head.hbs) and [`theme/mermaid_init.js`](../theme/mermaid_init.js) (loaded via `additional-js` in `book.toml`), so diagrams embedded throughout this book render automatically once published.

Continue to [Contributing](./contributing.md).
