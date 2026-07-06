# Dependencies and NuGet Packages

## SimplEnteiner (Main Library)

Source: [`SimplEnteiner/SimplEnteiner.csproj`](../SimplEnteiner/SimplEnteiner.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.7" />
    <PackageReference Include="System.Text.Json" Version="10.0.7" />
  </ItemGroup>

</Project>
```

| Package | Version | Why it's used |
|---|---|---|
| `Microsoft.Extensions.DependencyInjection` | `10.0.7` | Provides the `IServiceProvider`, `IServiceScope`, `IServiceScopeFactory`, and `ISupportRequiredService` abstractions implemented by [`SimplEnteinerServiceProvider`](./api/ms-di-integration.md) / [`SimplEnteinerServiceScope`](./api/ms-di-integration.md), plus the `IServiceCollection` extension surface (`AddSimplEnteiner`). SimplEnteiner's own container logic (binding, resolution, scopes) does **not** depend on this package at all — it is used purely for the optional [MS.DI interoperability layer](./architecture/di-integration.md). |
| `System.Text.Json` | `10.0.7` | Powers the [configuration export/import feature](./core/serialization.md) (`ExportConfiguration`/`ImportConfiguration`), serializing/deserializing `ScopeConfig`/`BindingConfig`/`DecoratorConfig` DTOs and arbitrary bound `Instance`/`Arguments`/`Id` values to/from JSON. |

Notably, **no third-party IoC/DI framework** (Autofac, Ninject, Lamar, etc.) is used anywhere — the entire binding/resolution/lifetime engine is implemented from scratch using only `System.Reflection` and `System.Linq.Expressions` from the BCL.

## SimplEnteinerTests (Test Project)

Source: [`SimplEnteinerTests/SimplEnteinerTests.csproj`](../SimplEnteinerTests/SimplEnteinerTests.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.5.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SimplEnteiner\SimplEnteiner.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
```

| Package | Version | Purpose |
|---|---|---|
| `coverlet.collector` | `6.0.0` | Code coverage collection when running `dotnet test` (integrates with the MSTest/VSTest data collector pipeline). |
| `Microsoft.NET.Test.Sdk` | `17.8.0` | Standard .NET test SDK required to run any test project via `dotnet test`. |
| `xunit` | `2.5.3` | The test framework used for every test in the solution (see [Testing and Quality Assurance](./testing.md)). |
| `xunit.runner.visualstudio` | `2.5.3` | Test adapter enabling Visual Studio / `dotnet test` discovery and execution of xUnit tests. |

The test project targets `net8.0` (not `netstandard2.1`) since test *execution* requires a concrete runtime, while the library itself targets `netstandard2.1` for maximum consumer compatibility (see [Introduction → Technology Stack](./introduction.md#technology-stack)).

## Is SimplEnteiner Published as a NuGet Package?

No `.nuspec` file, no `PackageId`/`Authors`/`Description`/`Version` MSBuild properties, and no packaging/publishing pipeline (`.github/workflows`) were found anywhere in the repository. The `SimplEnteiner.csproj` does not set `<IsPackable>` (defaults to packable for a class library, but no package metadata is defined, and no publish/pack workflow currently exists), and no license or NuGet-specific metadata files (`LICENSE`, `.nuspec`) are present in the repository root.

**Conclusion:** As of this documentation, SimplEnteiner is **not currently published or configured as a versioned NuGet package**; it is consumed as a source/project reference (as the test project does via `<ProjectReference>`). See [Building and Deployment](./building-deployment.md) for how you could add packaging support, and [Building and Deployment → Publishing to NuGet](./building-deployment.md#publishing-to-nuget-not-yet-configured) for the gaps that would need to be filled in to publish an official package.

Continue to [Testing and Quality Assurance](./testing.md).
