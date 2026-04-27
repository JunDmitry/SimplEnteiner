using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimplEnteiner
{
    /// <summary>
    /// Provides convenient methods for scanning the <see cref="System.Type">types</see> Types<br/>
    /// Has heavy lazy initializable static cache about <see cref="System.Type">types</see>
    /// </summary>
    public static partial class TypeAnalyzes
    {
        private static readonly Type s_generatedTypeAttribute = typeof(CompilerGeneratedAttribute);
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConstructorInfo>> s_injectableConstructorsCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConstructorInfo>>();
        private static readonly object s_lock = new object();

        private static HashSet<Assembly> s_assembliesCache = null;
        private static List<Type> s_cachedDomainTypes = null;
        private static bool s_initialized = false;

        /// <summary>
        /// Find all non-abstract class types assignable from type with option generic search flag. 
        /// Analysis all assemblies in CurrentDomain
        /// </summary>
        /// <param name="type">Assignable from</param>
        /// <param name="isGenericType">If true, returns only generic types, otherwise returns only non-generic types.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        /// <returns>All non-abstract class types assignable from <paramref name="type"/></returns>
        public static IEnumerable<Type> FindAllNonAbstractClassAssignableFrom(this Type type, bool isGenericType = false)
        {
            return FindAllAssignableFrom(type,
                t => (t.IsAbstract == false) && t.IsClass && (t.IsGenericType == isGenericType));
        }

        /// <summary>
        /// Find all non-abstract class types assignable from type with all possible search configure option Type.Is* flags. 
        /// Analysis all assemblies in CurrentDomain
        /// </summary>
        /// <param name="type">Assignable from</param>
        /// <param name="condition">Type enum flags from <see cref="System.Type">Type.Is*</see></param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        /// <returns>All types assignable from <paramref name="type"/> and satisfied <paramref name="condition"/> flag</returns>
        public static IEnumerable<Type> FindAllAssignableFrom(this Type type, TypeCondition condition)
        {
            if (condition == TypeCondition.None)
                return FindAllAssignableFrom(type);

            return FindAllAssignableFrom(type, t => (GetFlag(t) & condition) == condition);
        }

        /// <summary>
        /// Find all types assignable from <paramref name="type"/>. Analysis all assemblies in CurrentDomain
        /// </summary>
        /// <param name="type">Assignable from</param>
        /// <param name="additionAndPredicates">Additional predicates that applies to founded candidate type</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        /// <returns>All assignable from <paramref name="type"/> types</returns>
        public static IEnumerable<Type> FindAllAssignableFrom(this Type type, params Func<Type, bool>[] additionAndPredicates)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (s_lock)
            {
                if (s_initialized == false)
                    LoadAllDomainTypes();
            }

            additionAndPredicates ??= Array.Empty<Func<Type, bool>>();

            return FindAllAssignableFromInternal(type, additionAndPredicates);
        }

        /// <summary>
        /// Get all loadable types from Assembly <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">Assembly from where the types will be loaded</param>
        /// <returns>All loadable types from <paramref name="assembly"/></returns>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().SelectMany(GetAllNestedTypes);
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).SelectMany(GetAllNestedTypes);
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }


        /// <summary>
        /// Analyzes the type
        /// </summary>
        /// <param name="type">Type for analyzes</param>
        /// <param name="isIgnoreGeneratedType"></param>
        /// <returns>
        /// true if the type is not abstract, not an interface, not open generic, not static,<br/>
        /// not compiler—generated (<paramref name="isIgnoreGeneratedType"/>) and has at least one public constructor.
        /// </returns>
        public static bool IsConcreteClass(this Type type, bool isIgnoreGeneratedType = false)
        {
            if (isIgnoreGeneratedType && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
                return false;

            return type.IsClass
                && (type.IsAbstract == false)
                && (type.IsInterface == false)
                && (type.IsGenericTypeDefinition == false)
                && type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any();
        }

        /// <summary>
        /// Search for generic type definition
        /// </summary>
        /// <param name="type">Type for analyzes</param>
        /// <param name="genericTypeDefinition">Generic interface definition</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        /// <returns>true if <paramref name="type"/> implements the generic interface <paramref name="genericTypeDefinition"/></returns>
        public static bool IsAssignableToGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return IsAssignableGenericFrom(type, genericTypeDefinition, optionalPredicate: (t, op) => IsEqualOpenDefinition(op, t));
        }

        /// <summary>
        /// Search for generic type definition arguments
        /// </summary>
        /// <param name="type">Type for analyzes</param>
        /// <param name="genericTypeDefinition">Generic interface definition</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        /// <returns>Non-empty collection arguments if implements the generic interface <paramref name="genericTypeDefinition"/></returns>
        public static Type[] GetAssignableToGenericArguments(this Type type, Type genericTypeDefinition)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (IsEqualOpenDefinition(genericTypeDefinition, interfaceType))
                    return interfaceType.GetGenericArguments();
            }

            Type baseType = type.BaseType;

            while (baseType != null)
            {
                if (IsEqualOpenDefinition(genericTypeDefinition, baseType))
                    return baseType.GetGenericArguments();

                baseType = baseType.BaseType;
            }

            return Type.EmptyTypes;
        }

        /// <summary>
        /// Search all types with <typeparamref name="TAttribute"/> in <paramref name="assembly"/>
        /// </summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="assembly">Assembly for analyzes</param>
        /// <param name="isInherit">Search inherit defined attribute</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="assembly"/> is null</exception>
        /// <returns>All founded types in <paramref name="assembly"/> with <typeparamref name="TAttribute"/> defined</returns>
        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly, bool isInherit = false)
            where TAttribute : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            Type attributeType = typeof(TAttribute);

            return assembly.GetLoadableTypes().Where(t => t.IsDefined(attributeType, isInherit));
        }

        /// <summary>
        /// Search public <paramref name="type"/> constructor with <paramref name="injectAttributeType"/> attribute<br/>
        /// or first public constructor with the largest possible count of parameters
        /// </summary>
        /// <param name="type">Type for search</param>
        /// <param name="injectAttributeType">Inject attribute type</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> or <paramref name="injectAttributeType"/> is null</exception>
        /// <exception cref="System.Exception">If has multiple constructors with <paramref name="injectAttributeType"/> attribute in <paramref name="type"/></exception>
        /// <returns>Null if public constructor does not found</returns>
        public static ConstructorInfo? GetInjectableConstructor(this Type type, Type injectAttributeType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (injectAttributeType == null)
                throw new ArgumentNullException(nameof(injectAttributeType));

            ConcurrentDictionary<Type, ConstructorInfo> attributesCache = 
                s_injectableConstructorsCache.GetOrAdd(type, _ => new ConcurrentDictionary<Type, ConstructorInfo>());

            if (attributesCache.TryGetValue(injectAttributeType, out ConstructorInfo constructor))
                return constructor;

            ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (ctors.Length == 0)
            {
                attributesCache[injectAttributeType] = null;
                return null;
            }

            ConstructorInfo[] markedCtors = ctors.Where(ctor => ctor.IsDefined(injectAttributeType, true)).ToArray();
            ConstructorInfo? result;

            if (markedCtors.Length == 1)
                result = markedCtors[0];
            else if (markedCtors.Length > 1)
                throw new Exception($"Multiple constructors with {injectAttributeType.Name} attribute in {type}");
            else
                result = ctors.OrderByDescending(ctor => ctor.GetParameters().Length).First();

            attributesCache[injectAttributeType] = result;

            return result;
        }

        /// <summary>
        /// Supported generic types:
        /// <list type="bullet|number|table">
        ///     <listheader>
        ///         <term>Type</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>IEnumerable generic</term>
        ///         <description><see cref="System.Collections.Generic.IEnumerable{T}"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Lazy generic</term>
        ///         <description><see cref="System.Lazy{T}"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Func generic</term>
        ///         <description><see cref="System.Func{TResult}"/></description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="parameterInfo">Parameter</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="parameterInfo"/> is null</exception>
        /// <returns>Type to be injected for <paramref name="parameterInfo"/></returns>
        public static Type GetDependencyType(this ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
                throw new ArgumentNullException(nameof(parameterInfo));

            return parameterInfo.ParameterType.GetUnderlyingDependencyType();
        }

        /// <summary>
        /// Generates factory lambda method for quick instantiation without <see cref="Activator.CreateInstance(Type)"/><br/>
        /// For IL2CPP does not work
        /// </summary>
        /// <param name="constructor">Constructor for which create factory lambda method</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="constructor"/> is null</exception>
        /// <returns>Compiled factory lambda method</returns>
        public static Func<object[], object> GetFactoryMethod(this ConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            try
            {
                ParameterExpression parametersExpression = Expression.Parameter(typeof(object[]), "args");
                IEnumerable<UnaryExpression> parameterExpression = constructor.GetParameters().Select((p, i) =>
                {
                    return Expression.Convert(Expression.ArrayIndex(parametersExpression, Expression.Constant(i)), p.ParameterType);
                });
                NewExpression newExpression = Expression.New(constructor, parameterExpression);
                Expression<Func<object[], object>> lambda = Expression.Lambda<Func<object[], object>>(newExpression, parametersExpression);

                return lambda.Compile();
            }
            catch
            {
                return (args) => constructor.Invoke(args);
            }
        }

        /// <summary>
        /// Search members in <paramref name="type"/> for fields/properties/methods marked with <paramref name="injectAttribute"/>
        /// </summary>
        /// <param name="type">Type for members search</param>
        /// <param name="injectAttribute">Inject attribute type</param>
        /// <returns>All fields/properties/methods marked with <paramref name="injectAttribute"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="injectAttribute"/> is null</exception>
        public static IEnumerable<MemberInfo> GetInjectableMembers(this Type type, Type injectAttribute)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (injectAttribute == null)
                throw new ArgumentNullException(nameof(injectAttribute));

            BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            IEnumerable<MemberInfo>[] members = { type.GetFields(binding), type.GetProperties(binding), type.GetMethods(binding) };

            foreach (IEnumerable<MemberInfo> memberInfos in members)
            {
                foreach (MemberInfo member in memberInfos)
                {
                    if (member.IsDefined(injectAttribute, true))
                        yield return member;
                }
            }
        }

        /// <summary>
        /// Compares whether the <paramref name="args"/> are matches for <paramref name="constraints"/>
        /// </summary>
        /// <param name="args">Generic argument types</param>
        /// <param name="constraints">Generic type constraints</param>
        /// <returns><value>true</value> if <paramref name="args"/> matches <paramref name="constraints"/> otherwise false</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> or <paramref name="constraints"/> is null</exception>
        public static bool MatchesGenericParameters(this Type[] args, Type[] constraints)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            if (args.Length != constraints.Length)
                return false;

            int length = args.Length;

            for (int i = 0; i < length; i++)
            {
                if (constraints[i].IsAssignableFrom(args[i]) == false)
                    return false;
            }
            
            return true;
        }

        /// <summary>
        /// Collection dependency graph of <paramref name="type"/><br/> 
        /// with <paramref name="injectAttribute"/> attribute and optional <paramref name="resolver"/>
        /// </summary>
        /// <param name="type">Type for dependencies search</param>
        /// <param name="injectAttribute">Inject attribute type</param>
        /// <param name="resolver">Optional dependency resolver</param>
        /// <returns>Dependency types collection</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="injectAttribute"/> is null</exception>
        public static HashSet<Type> GetAllDependencies(this Type type, Type injectAttribute, Func<Type, Type> resolver = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (injectAttribute == null)
                throw new ArgumentNullException(nameof(injectAttribute));

            HashSet<Type> visited = new HashSet<Type>();
            Stack<Type> stack = new Stack<Type>();
            ConstructorInfo? ctor;

            stack.Push(type);

            while (stack.Count > 0)
            {
                Type current = stack.Pop();

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                try
                {
                    ctor = current.GetInjectableConstructor(injectAttribute);
                }
                catch
                {
                    ctor = null;
                }

                if (ctor != null)
                {
                    foreach (ParameterInfo parameter in ctor.GetParameters())
                    {
                        AddDependency(resolver, visited, stack, parameter.ParameterType.GetUnderlyingDependencyType());
                    }
                }

                AddAllMemberDependencies(injectAttribute, resolver, visited, stack, current);
            }

            visited.Remove(type);

            return visited;
        }

        /// <summary>
        /// Checks if the type has a cyclic dependency in its injection graph 
        /// (via constructor or injectable members).
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="injectAttribute">Inject attribute type.</param>
        /// <param name="cyclePath">If a cycle is found, contains the cycle path; otherwise null.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> or <paramref name="injectAttribute"/> is null</exception>
        /// <returns>True if a cyclic dependency exists.</returns>
        public static bool HasCyclicDependencies(this Type type, Type injectAttribute, out List<Type> cyclePath)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (injectAttribute == null)
                throw new ArgumentNullException(nameof(injectAttribute));

            HashSet<Type> visited = new HashSet<Type>();
            Stack<Type> path = new Stack<Type>();
            cyclePath = null;

            return HasCyclicDependenciesRecursive(type, injectAttribute, visited, path, out cyclePath);
        }

        /// <summary>
        /// Add assemblies to cache for analysis and immediately cache loadable types for each <see cref="Assembly"/>
        /// </summary>
        /// <param name="assemblies">Assemblies to cache</param>
        /// <exception cref="ArgumentNullException">If <paramref name="assemblies"/> is null</exception>
        public static void AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            lock (s_lock)
            {
                s_assembliesCache ??= new HashSet<Assembly>();
                List<Type> newTypes = new List<Type>(s_cachedDomainTypes ?? Enumerable.Empty<Type>());
                bool wasAdded = false;

                foreach (Assembly assembly in assemblies)
                {
                    if (s_assembliesCache.Contains(assembly))
                        continue;
                    
                    newTypes.AddRange(assembly.GetLoadableTypes());
                    s_assembliesCache.Add(assembly);
                    wasAdded = true;
                }

                if (wasAdded || s_cachedDomainTypes == null)
                    s_cachedDomainTypes = newTypes.Distinct().ToList();
            }
        }

        /// <summary>
        /// Returns the underlying dependency type, unwrapping T[], IEnumerable&lt;T&gt;, Lazy&lt;T&gt;, Func&lt;T&gt;.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is null</exception>
        public static Type GetUnderlyingDependencyType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                Type def = type.GetGenericTypeDefinition();

                if (def == typeof(IEnumerable<>) ||
                    def == typeof(Lazy<>) ||
                    def == typeof(Func<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return type;
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> arguments satisfy the restrictions of open <paramref name="openDefinition"/> interface
        /// </summary>
        /// <param name="type">Type for satisfy check</param>
        /// <param name="openDefinition">Opened Generic Interface definition</param>
        /// <returns>
        /// true if whether the <paramref name="type"/> arguments satisfy the restrictions 
        /// of opened(Will be first founded interface) <paramref name="openDefinition"/> interface
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="openDefinition"/> is null</exception>
        public static bool SatisfiesOpenedGenericConstraints(this Type type, Type openDefinition)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (openDefinition == null)
                throw new ArgumentNullException(nameof(openDefinition));

            return IsAssignableGenericFrom(type, openDefinition, 
                (t, open) => IsEqualOpenDefinition(open, t) == false, 
                (t, open) => SatisfiesGenericConstraints(open, t.GetGenericArguments()));
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> arguments satisfy the restrictions of <paramref name="closedGenericDefinition"/> interface
        /// </summary>
        /// <param name="type">Type for satisfy check</param>
        /// <param name="closedGenericDefinition">Generic Interface definition</param>
        /// <returns>true if whether the <paramref name="type"/> arguments satisfy the restrictions of <paramref name="closedGenericDefinition"/> interface</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="closedGenericDefinition"/> is null</exception>
        /// <exception cref="ArgumentException">If <paramref name="closedGenericDefinition"/> is not closed generic type</exception>
        public static bool SatisfiesClosedGenericConstraints(this Type type, Type closedGenericDefinition)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (closedGenericDefinition == null)
                throw new ArgumentNullException(nameof(closedGenericDefinition));

            if (type == closedGenericDefinition)
                return true;
            
            if ((closedGenericDefinition.IsGenericType == false) || closedGenericDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"{nameof(closedGenericDefinition)} must be a closed generic type.");

            Type openDefinition = closedGenericDefinition.GetGenericTypeDefinition();

            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (IsEqualOpenDefinition(openDefinition, interfaceType) == false)
                    continue;

                if (TrySatisfiesConstraints(interfaceType.GetGenericArguments(), closedGenericDefinition, openDefinition, out bool isSatisfies))
                    return isSatisfies;
            }

            Type baseType = type.BaseType;
            bool isContinue;

            while (baseType != null)
            {
                isContinue = IsEqualOpenDefinition(openDefinition, baseType) == false;

                if ((isContinue == false)
                    && TrySatisfiesConstraints(baseType.GetGenericArguments(), closedGenericDefinition, openDefinition, out bool isSatisfies))
                    return isSatisfies;

                baseType = baseType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Get all marked constructors with <paramref name="injectAttribute"/> attribute in <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type from which found marked constructors</param>
        /// <param name="injectAttribute">Type mark attribute</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="injectAttribute"/> is null</exception>
        /// <returns>All marked with <paramref name="injectAttribute"/> constructors in <paramref name="type"/></returns>
        public static ConstructorInfo[] GetMarkedConstructors(this Type type, Type injectAttribute)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (injectAttribute == null)
                throw new ArgumentNullException(nameof(injectAttribute));

            return type.GetConstructors()
                .Where(c => c.IsDefined(injectAttribute, true))
                .ToArray();
        }

        /// <summary>
        /// Check that <paramref name="parameter"/> is optional or has default value
        /// </summary>
        /// <param name="parameter">Parameter for check</param>
        /// <returns>true if optional otherwise false</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="parameter"/> is null</exception>
        public static bool IsOptionalParameter(this ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            return parameter.IsDefined(typeof(OptionalAttribute), false) || parameter.HasDefaultValue;
        }

        /// <summary>
        /// For <paramref name="member"/> dependency resolution
        /// </summary>
        /// <param name="member">Member for dependency resolution</param>
        /// <returns>
        /// Types collection if <paramref name="member"/> is <see cref="MethodInfo"/><br/>
        /// otherwise(<see cref="FieldInfo"/>, <see cref="PropertyInfo"/>) collection with one element<br/>
        /// Empty collection if not implemented <paramref name="member"/> type case
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="member"/> is null</exception>
        public static Type[] GetMemberDependencyType(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member switch
            {
                FieldInfo fieldInfo => new[] { fieldInfo.FieldType },
                PropertyInfo propertyInfo => new[] { propertyInfo.PropertyType },
                MethodInfo methodInfo => methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
                _ => Array.Empty<Type>()
            };
        }

        /// <summary>
        /// Checks that each type is either a concrete class or registered in the passed <paramref name="dependencyRegistryMap"/>
        /// </summary>
        /// <param name="type">Type for dependency resolve check</param>
        /// <param name="injectAttribute">Inject attribute type</param>
        /// <param name="dependencyRegistryMap">Dependency registry map</param>
        /// <param name="resolver">Optional dependency resolver</param>
        /// <returns>true if can resolve <paramref name="type"/> dependencies with <paramref name="dependencyRegistryMap"/> otherwise false</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/>, <paramref name="injectAttribute"/> or <paramref name="dependencyRegistryMap"/> is null</exception>
        /// <exception cref="CircularDependencyException">If cyclical dependency has been detected</exception>
        public static bool CanResolveAllDependencies(this Type type, 
            Type injectAttribute, 
            IReadOnlyDictionary<Type, Type> dependencyRegistryMap, 
            Func<Type, Type> resolver = null)
        {
            if (dependencyRegistryMap == null)
                throw new ArgumentNullException(nameof(dependencyRegistryMap));

            if (HasCyclicDependencies(type, injectAttribute, out List<Type> cyclePath))
                throw new CircularDependencyException(cyclePath);

            HashSet<Type> dependencies = type.GetAllDependencies(injectAttribute, resolver);

            foreach (Type dependency in dependencies)
            {
                if (dependency.IsConcreteClass() == false
                    && dependencyRegistryMap.ContainsKey(dependency) == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Clear static cache about types
        /// </summary>
        public static void ClearCache()
        {
            lock (s_lock)
            {
                s_injectableConstructorsCache.Clear();

                s_cachedDomainTypes = null;
                s_initialized = false;
            }
        }

        private static void LoadAllDomainTypes()
        {
            if (s_initialized == false)
            {
                s_assembliesCache ??= new HashSet<Assembly>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (s_assembliesCache.Contains(assembly) == false)
                        s_assembliesCache.Add(assembly);
                }

                // If .NetCore 1.0 and high or .NetStandart 5.0 and high
                //foreach (Assembly assembly in AssemblyLoadContext.All)
                //    s_assembliesCache.Add(assembly);

                s_initialized = true;
            }

            s_cachedDomainTypes = s_assembliesCache.SelectMany(GetLoadableTypes).Distinct().ToList();
        }

        private static IEnumerable<Type> GetAllNestedTypes(this Type type)
        {
            if (type == null)
                yield break;

            Queue<Type> types = new Queue<Type>();
            types.Enqueue(type);

            while (types.Count > 0)
            {
                Type nestedType = types.Dequeue();

                yield return nestedType;

                foreach (Type nested in nestedType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (nested == null)
                        continue;

                    types.Enqueue(nested);
                }
            }
        }

        private static bool SatisfiesGenericConstraints(Type genericDefinition, Type[] typeArgs)
        {
            Type[] genericArgs = genericDefinition.GetGenericArguments();

            for (int i = 0; i < typeArgs.Length; i++)
            {
                Type[] constraints = genericArgs[i].GetGenericParameterConstraints();
                Type typeArgument = typeArgs[i];

                foreach (Type constraint in constraints)
                {
                    if (constraint.IsAssignableFrom(typeArgument) == false)
                        return false;
                }

                GenericParameterAttributes attributes = genericArgs[i].GenericParameterAttributes;

                if ((attributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0
                    && typeArgument.IsValueType)
                    return false;

                if ((attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0
                    && (typeArgument.IsValueType == false))
                    return false;

                if ((attributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0
                    && typeArgument.GetConstructor(Type.EmptyTypes) == null
                    && (typeArgument.IsValueType == false))
                    return false;
            }
            
            return true;
        }

        private static bool IsEqualOpenDefinition(Type openDefinition, Type interfaceType)
        {
            return interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == openDefinition);
        }

        private static bool TrySatisfiesConstraints(Type[] args, Type closedGenericDefinition, Type openGenericDefinition, out bool isSatisfies)
        {
            isSatisfies = default;

            if (args.SequenceEqual(closedGenericDefinition.GetGenericArguments()))
            {
                isSatisfies = SatisfiesGenericConstraints(openGenericDefinition, args);
                return true;
            }

            return false;
        }

        private static bool IsAssignableGenericFrom(
            Type type,
            Type openGeneric,
            Func<Type, Type, bool>? continuePredicate = null,
            Func<Type, Type, bool>? optionalPredicate = null)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (continuePredicate?.Invoke(interfaceType, openGeneric) ?? false)
                    continue;

                if (optionalPredicate?.Invoke(interfaceType, openGeneric) ?? false)
                    return true;
            }

            Type baseType = type.BaseType;
            bool isContinue;

            while (baseType != null)
            {
                isContinue = continuePredicate?.Invoke(baseType, openGeneric) ?? false;

                if ((isContinue == false) && (optionalPredicate?.Invoke(baseType, openGeneric) ?? false))
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }

        private static bool HasCyclicDependenciesRecursive(Type type, Type injectAttribute, HashSet<Type> visited, Stack<Type> path, out List<Type> cyclePath)
        {
            if (path.Contains(type))
            {
                cyclePath = path.Reverse().ToList();
                int index = cyclePath.IndexOf(type);
                cyclePath.RemoveRange(0, index);
                cyclePath.Add(type);

                return true;
            }

            if (visited.Contains(type))
            {
                cyclePath = null;
                
                return false;
            }

            path.Push(type);
            ConstructorInfo? constructor = type.GetInjectableConstructor(injectAttribute);

            if (constructor != null)
            {
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (HasCyclicDependenciesRecursive(parameter.ParameterType.GetUnderlyingDependencyType(), injectAttribute, visited, path, out cyclePath))
                        return true;
                }
            }

            foreach (MemberInfo member in type.GetInjectableMembers(injectAttribute))
            {
                Type[] dependencies = GetMemberDependencyType(member);

                foreach (Type dependency in dependencies)
                {
                    if (HasCyclicDependenciesRecursive(dependency.GetUnderlyingDependencyType(), injectAttribute, visited, path, out cyclePath))
                        return true;
                }
            }

            path.Pop();
            visited.Add(type);
            cyclePath = null;

            return false;
        }

        private static IEnumerable<Type> FindAllAssignableFromInternal(Type type, Func<Type, bool>[] additionAndPredicates)
        {
            foreach (Type candidateType in s_cachedDomainTypes)
            {
                if (type.IsGenericTypeDefinition)
                {
                    if (IsAssignableGenericFrom(candidateType, type, optionalPredicate: (t, op) => IsEqualOpenDefinition(op, t)) == false)
                        continue;
                }
                else if (type.IsAssignableFrom(candidateType) == false)
                {
                    continue;
                }

                bool isAssignable = additionAndPredicates.All(p => p == null || p(candidateType));

                if (isAssignable)
                    yield return candidateType;
            }
        }

        private static void AddAllMemberDependencies(Type injectAttribute, Func<Type, Type> resolver, HashSet<Type> visited, Stack<Type> stack, Type current)
        {
            foreach (MemberInfo member in current.GetInjectableMembers(injectAttribute))
            {
                Type[] dependencies = GetMemberDependencyType(member);

                foreach (Type dependency in dependencies)
                {
                    AddDependency(resolver, visited, stack, dependency.GetUnderlyingDependencyType());
                }
            }
        }

        private static void AddDependency(Func<Type, Type>? resolver, HashSet<Type> visited, Stack<Type> stack, Type memberType)
        {
            Type dependencyType = resolver == null ? memberType : resolver(memberType);

            if (visited.Contains(dependencyType) == false)
                stack.Push(dependencyType);
        }

        private static void Clear()
        {
            lock (s_lock)
            {
                s_assembliesCache = null;
                s_cachedDomainTypes = null;
                s_initialized = false;
            }
        }

        private static TypeCondition GetFlag(Type type)
        {
            TypeCondition condition = TypeCondition.None;

            if (type.IsAbstract)
                condition |= TypeCondition.Abstract;

            if (type.IsAnsiClass)
                condition |= TypeCondition.AnsiClass;

            if (type.IsArray)
                condition |= TypeCondition.Array;

            if (type.IsAutoClass)
                condition |= TypeCondition.AutoClass;

            if (type.IsAutoLayout)
                condition |= TypeCondition.AutoLayout;

            if (type.IsByRef)
                condition |= TypeCondition.ByRef;

            if (type.IsByRefLike)
                condition |= TypeCondition.ByRefLike;

            if (type.IsClass)
                condition |= TypeCondition.Class;

            if (type.IsCOMObject)
                condition |= TypeCondition.COMObject;

            if (type.IsConstructedGenericType)
                condition |= TypeCondition.ConstructedGenericType;

            if (type.IsContextful)
                condition |= TypeCondition.Contextful;

            if (type.IsEnum)
                condition |= TypeCondition.Enum;

            if (type.IsExplicitLayout)
                condition |= TypeCondition.ExplicitLayout;

            if (type.IsGenericMethodParameter)
                condition |= TypeCondition.GenericMethodParameter;

            if (type.IsGenericParameter)
                condition |= TypeCondition.GenericParameter;

            if (type.IsGenericType)
                condition |= TypeCondition.GenericType;

            if (type.IsGenericTypeDefinition)
                condition |= TypeCondition.GenericTypeDefinition;

            if (type.IsGenericTypeParameter)
                condition |= TypeCondition.GenericTypeParameter;

            if (type.IsImport)
                condition |= TypeCondition.Import;

            if (type.IsInterface)
                condition |= TypeCondition.Interface;

            if (type.IsLayoutSequential)
                condition |= TypeCondition.LayoutSequential;

            if (type.IsMarshalByRef)
                condition |= TypeCondition.MarshalByRef;

            if (type.IsNested)
                condition |= TypeCondition.Nested;

            if (type.IsNestedAssembly)
                condition |= TypeCondition.NestedAssembly;

            if (type.IsNestedFamANDAssem)
                condition |= TypeCondition.NestedFamAndAssem;

            if (type.IsNestedFamily)
                condition |= TypeCondition.NestedFamily;

            if (type.IsNestedFamORAssem)
                condition |= TypeCondition.NestedFamOrAssem;

            if (type.IsNestedPublic)
                condition |= TypeCondition.NestedPublic;

            if (type.IsNestedPrivate)
                condition |= TypeCondition.NestedPrivate;

            if (type.IsNotPublic)
                condition |= TypeCondition.NotPublic;

            if (type.IsPointer)
                condition |= TypeCondition.Pointer;

            if (type.IsPrimitive)
                condition |= TypeCondition.Primitive;

            if (type.IsPublic)
                condition |= TypeCondition.Public;

            if (type.IsSealed)
                condition |= TypeCondition.Sealed;

            if (type.IsSecurityCritical)
                condition |= TypeCondition.SecurityCritical;

            if (type.IsSecuritySafeCritical)
                condition |= TypeCondition.SecuritySafeCritical;

            if (type.IsSecurityTransparent)
                condition |= TypeCondition.SecurityTransparent;

            if (type.IsSerializable)
                condition |= TypeCondition.Serializable;

            if (type.IsSignatureType)
                condition |= TypeCondition.SignatureType;

            if (type.IsSpecialName)
                condition |= TypeCondition.SpecialName;

            if (type.IsSZArray)
                condition |= TypeCondition.SZArray;

            if (type.IsTypeDefinition)
                condition |= TypeCondition.TypeDefinition;

            if (type.IsUnicodeClass)
                condition |= TypeCondition.UnicodeClass;

            if (type.IsValueType)
                condition |= TypeCondition.ValueType;

            if (type.IsVariableBoundArray)
                condition |= TypeCondition.VariableBoundArray;

            if (type.IsVisible)
                condition |= TypeCondition.Visible;

            return condition;
        }

        public sealed class CircularDependencyException : Exception
        {
            public CircularDependencyException(IEnumerable<Type> circularPath) 
                : base($"A cyclical dependency has been detected. Path: [ {string.Join(" -> ", circularPath.Select(t => t.FullName))} ]")
            {
                CircularPath = circularPath.ToList().AsReadOnly();
            }

            public IReadOnlyList<Type> CircularPath { get; }
        }
    }
}
