# Performance and Optimization

SimplEnteiner does not currently ship a `BenchmarkDotNet` project or any published throughput/latency/memory benchmarks (no benchmark project exists in the solution — only [`SimplEnteiner`](../SimplEnteiner) and [`SimplEnteinerTests`](../SimplEnteinerTests) are present in [`SimplEnteiner.sln`](../SimplEnteiner.sln)). This page instead documents the performance-relevant design decisions actually present in the code, and gives practical recommendations derived from the implementation.

## Performance-Relevant Design Decisions

### Compiled Factory Delegates instead of `Activator.CreateInstance`

Source: `TypeAnalyzes.GetFactoryMethod`

```csharp
public static Func<object[], object> GetFactoryMethod(this ConstructorInfo constructor)
{
    try
    {
        ParameterExpression parametersExpression = Expression.Parameter(typeof(object[]), "args");
        IEnumerable<UnaryExpression> parameterExpression = constructor.GetParameters().Select((p, i) =>
            Expression.Convert(Expression.ArrayIndex(parametersExpression, Expression.Constant(i)), p.ParameterType));
        NewExpression newExpression = Expression.New(constructor, parameterExpression);
        Expression<Func<object[], object>> lambda = Expression.Lambda<Func<object[], object>>(newExpression, parametersExpression);

        return lambda.Compile();
    }
    catch
    {
        return (args) => constructor.Invoke(args);
    }
}
```

Each `Registration` compiles its constructor invocation **once**, at registration time (`Registry.CreateRegistration`), rather than reflectively invoking the constructor on every resolution. This is significantly faster than repeated `ConstructorInfo.Invoke`/`Activator.CreateInstance` calls for hot paths with many resolutions, at the (one-time, per-binding) cost of expression-tree compilation. The `try/catch` fallback to `constructor.Invoke(args)` protects environments where dynamic method compilation is unavailable or restricted (AOT/IL2CPP), trading throughput for portability in those environments only.

### Reflection Metadata Caching (`TypeAnalyzes`)

Source: [`TypeAnalyzes.cs`](../SimplEnteiner/TypeAnalyzes.cs)

- **`s_injectableConstructorsCache`** — a `ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConstructorInfo>>` memoizing `GetInjectableConstructor(type, injectAttributeType)` results, avoiding repeated `GetConstructors()` + attribute-scanning + LINQ ordering on every call for the same `(type, attribute)` pair. This directly benefits the hot resolution path, since `Resolver` calls `GetInjectableConstructor` on every non-cached-instance resolution.
- **`s_cachedDomainTypes`** — a process-wide, lazily-populated `List<Type>` avoiding repeated `AppDomain.CurrentDomain.GetAssemblies()` + `GetTypes()` scans for every call to `FindAllAssignableFrom`/`FindAllNonAbstractClassAssignableFrom`. This is primarily relevant to [convention-based binding](./api/convention-binding.md) and reflection-heavy startup code, not to steady-state resolution.
- Both caches are guarded for thread safety (`ConcurrentDictionary` for the constructor cache; a single `lock (s_lock)` for the domain-types cache during initial population), so concurrent startup-time registration/scanning from multiple threads is safe, though the domain-types cache population itself is a serialized (locked) operation the first time it runs.

### Iterative (not Recursive) Dependency Graph Walking for `GetAllDependencies`

`TypeAnalyzes.GetAllDependencies` uses an explicit `Stack<Type>` rather than recursive calls, avoiding call-stack growth (and potential `StackOverflowException`) for deep or wide dependency graphs. `HasCyclicDependencies`, by contrast, **is** implemented recursively (`HasCyclicDependenciesRecursive`) — very deep dependency chains (unusual in practice for typical DI graphs) could theoretically exhaust the call stack during cycle detection; this asymmetry is worth being aware of for pathological inputs.

### Per-Resolution-Call Context, Not Global State

`ResolutionContext` is allocated fresh (and disposed via `using`) for every top-level `Resolve()` call, keeping the `Cached` lifetime's dictionary short-lived and avoiding any global mutable resolution state that would need locking on every resolve. Regular scope-level state (`_scopedInstances`, singleton repository) uses targeted `lock` statements scoped tightly around dictionary mutations (see `Scope.StoreSingleton`, `StoreScoped`) rather than broad, contention-prone locks.

## Known Bottlenecks / Trade-offs (Inferred from the Implementation)

- **`Registry.ValidateAll()` and `AnalyzeReachability()` are O(bindings × dependency-graph size)** — they re-walk each registered type's full dependency graph independently. For very large registration graphs (hundreds+ of services with deep dependency chains), calling these at every `Build()` could add measurable startup latency; this is an intentional trade-off (fail-fast correctness over startup speed) and is only paid once at composition-root build time, not per-request.
- **`IEnumerable<T>` resolution is O(all exact bindings in scope)** — `Resolver.ResolveAllEnumerable` linearly scans `context.CurrentScope.GetAllExactRegistration()` for every assignable type, on every resolution of an `IEnumerable<T>` dependency. For containers with very large numbers of registrations, resolving many-implementations collections repeatedly could be a hot spot; consider caching the resolved collection yourself (e.g., behind a `Singleton` wrapper service) if this becomes measurable in your application.
- **`Func<T>` wrapper compilation happens on every resolution** (`Resolver.CreateFunc` builds and compiles a new `Expression.Lambda` each time a `Func<T>` dependency is resolved) — unlike constructor factories (compiled once and cached on the `Registration`), this compilation is **not** cached/memoized anywhere. If `Func<T>` dependencies are resolved frequently in a hot path, this repeated expression-tree compilation cost should be measured; a possible optimization opportunity (see [Conclusion → Roadmap](./conclusion.md)) is caching the compiled `Func<T>` factory per `(argumentType, scope)` pair.
- **Decorator resolution walks the entire scope chain** (`Scope.GetDecoratorRegistrations` collects every ancestor scope into a `List<IScope>` on every call) for every decorated resolution — for deep scope trees, this adds a per-resolution cost proportional to scope depth, though scope trees are typically shallow (root + 1–2 levels) in practice.

## Recommendations for Consumers

- **Prefer `Singleton`/`Scoped` over `Transient`/`Cached` for expensive-to-construct services** that don't need per-call freshness — this avoids repeated constructor/member-injection work and repeated `IInitializable.Initialize()` invocations.
- **Call `Build()` once at startup**, not repeatedly — it triggers full-graph validation (`ValidateAll`) and singleton eager-instantiation (`IStartable.Start()`), which are one-time, not per-resolution costs. Do not call `Build()` inside a request/resolution hot path.
- **Reuse `IScope` instances appropriately** (e.g., one child scope per logical unit of work/request) rather than creating and disposing scopes excessively — scope creation allocates a new `Registry`, `_scopedInstances` dictionary, and `CleanupService` each time.
- **Avoid resolving `Func<T>`/`IEnumerable<T>` dependencies in extremely hot loops** without caching the resolved delegate/collection yourself, given the per-resolution compilation/scan costs described above.
- **Use `TypeAnalyzes.AddAssemblies(...)`/`ClearCache()` deliberately** — clearing the domain-type cache forces a full re-scan of all tracked assemblies on next use, which is comparatively expensive; only clear it when you know new types genuinely need to be picked up (e.g., after dynamically loading a plugin assembly).
- If you need concrete performance numbers for your scenario, the recommended approach is to add a `BenchmarkDotNet`-based micro-benchmark project (none currently exists) exercising your specific binding/resolution patterns — see [Conclusion → Roadmap](./conclusion.md).

Continue to [Error Handling and Logging](./error-handling.md).
