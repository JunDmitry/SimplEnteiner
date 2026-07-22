# SimplEnteiner
A small, self-contained Dependency Injection (DI) framework for .NET, designed with simplicity and minimal dependencies in mind. Built from scratch using plain reflection and `System.Linq.Expressions`, it has no dependency on any third‑party IoC container.

**Key features:**
- Fluent binding API (similar in spirit to Ninject/Autofac)
- Scoped and hierarchical containers
- Decorator support
- Convention-based auto-registration
- Reachability validation

**Designed for:**
- .NET Standard 2.0+ (works in Unity, legacy projects, and modern .NET)
- Libraries and plugins where keeping external dependencies low is important

## Quick start

```csharp
DIContainer container = new DIContainer();

// ---Bindings---
container.Bind<IService>().To<Service>().AsSingle();
container.Bind<IScopedService>().To<ScopedService>().AsScoped();
// --------------

container.Build();

IService service = container.ResolverProvider.Resolve<IService>();
```

## Documentation and resources

📖 [Full documentation, examples, and API reference](https://jundmitry.github.io/SimplEnteiner/)\
📄 [Licence](https://github.com/JunDmitry/SimplEnteiner/blob/main/LICENSE)

## Status

⚠️ Actively developed (Beta).
- AOT support (in development)
