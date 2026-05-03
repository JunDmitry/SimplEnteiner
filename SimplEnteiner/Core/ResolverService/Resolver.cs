using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplEnteiner.Core.Lifecycle;
using SimplEnteiner.Core.RegistrationService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ResolverService
{
    internal class Resolver : IResolver
    {
        private static readonly Type s_injectAttributeType = Constants.InjectAttributeType;
        private static readonly MethodInfo s_resolveMethod = typeof(Resolver).GetMethod(nameof(Resolve), new[] { typeof(Type), typeof(Scope), typeof(object) });

        private readonly IInterfaceInvoker _invoker;

        public Resolver()
        {
            _invoker = new InterfaceInvoker();
        }

        public T Resolve<T>(Scope scope, object id = null) => (T)Resolve(typeof(T), scope, id);

        public object Resolve(Type interfaceType, Scope scope, object id = null)
        {
            using ResolutionContext context = new ResolutionContext(scope, id);
            
            return ResolveInternal(interfaceType, context);
        }

        private object ResolveInternal(Type interfaceType, ResolutionContext context)
        {
            object instance = null;

            if (interfaceType.IsGenericType)
            {
                Type genericDefinition = interfaceType.GetGenericTypeDefinition();

                if (genericDefinition == typeof(IEnumerable<>))
                {
                    Type elementType = interfaceType.GetGenericArguments()[0];
                    instance = ResolveAllEnumerable(elementType, context);
                }
                else if (genericDefinition == typeof(Lazy<>))
                {
                    Type argumentType = interfaceType.GetGenericArguments()[0];
                    instance = CreateLazy(argumentType, context);
                }
                else if (genericDefinition == typeof(Func<>))
                {
                    Type argumentType = interfaceType.GetGenericArguments()[0];
                    instance = CreateFunc(argumentType, context);
                }
            }
            
            if (instance == null)
            {
                Registration registration = GetRegistration(interfaceType, context).ThrowInvalidIfNull($"No binding found for {interfaceType}");
                instance = ResolveRegistration(registration, interfaceType, context);
            }

            return ResolveDecorators(instance, interfaceType, context);
        }

        private object ResolveAllEnumerable(Type elementType, ResolutionContext context)
        {
            Type instanceType = typeof(List<>).MakeGenericType(elementType);
            IList list = (IList)Activator.CreateInstance(instanceType);

            IEnumerable<object> items = context.CurrentScope.GetAllExactRegistration()
                .Where(pair => elementType.IsAssignableFrom(pair.Key))
                .Select(pair => ResolveInternal(pair.Key, context));

            foreach (object item in items)
            {
                list.Add(item);
            }

            return list;
        }

        private object CreateLazy(Type argumentType, ResolutionContext context)
        {
            Type lazyType = typeof(Lazy<>).MakeGenericType(argumentType);
            object func = CreateFunc(argumentType, context);

            return Activator.CreateInstance(lazyType, func);
        }

        private object CreateFunc(Type argumentType, ResolutionContext context)
        {
            Scope scope = context.CurrentScope;
            Resolver resolver = this;

            Type funcType = typeof(Func<>).MakeGenericType(argumentType);
            ConstantExpression resolverInstance = Expression.Constant(resolver);
            ConstantExpression typeConst = Expression.Constant(argumentType);
            ConstantExpression scopeConst = Expression.Constant(scope);
            ConstantExpression nullObject = Expression.Constant(null, typeof(object));

            MethodCallExpression methodCall = Expression.Call(resolverInstance, s_resolveMethod, typeConst, scopeConst, nullObject);
            UnaryExpression convert = Expression.Convert(methodCall, argumentType);
            LambdaExpression lambda = Expression.Lambda(funcType, convert);

            return lambda.Compile();
        }

        private Registration GetRegistration(Type interfaceType, ResolutionContext context)
        {
            if (context.Id != null)
            {
                Registration reg = context.CurrentScope.FindConditionalRegistration(interfaceType, context.Id)
                    ?? throw new InvalidOperationException($"No binding found for {interfaceType} with id '{context.Id}'");

                return reg;
            }
            else if (context.RequestType != null)
            {
                Registration reg = context.CurrentScope.FindConditionalRegistration(interfaceType, context.RequestType);

                if (reg != null)
                    return reg;
            }

            Registration registration = context.CurrentScope.FindExactRegistration(interfaceType);

            if (registration != null)
                return registration;

            if (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false))
            {
                registration = GetClosedGenericRegistration(interfaceType, context.CurrentScope);

                if (registration != null)
                    return registration;
            }
            
            if (interfaceType.IsConcreteClass(isIgnoreGeneratedType: true))
            {
                ConstructorInfo ctor = interfaceType.GetInjectableConstructor(s_injectAttributeType);
                Func<object[], object> factory = ctor.GetFactoryMethod();

                return new Registration(interfaceType, LifeTime.Transient, factory, null);
            }

            return null;
        }

        private Registration GetClosedGenericRegistration(Type interfaceType, Scope scope)
        {
            Type genericDefinition = interfaceType.GetGenericTypeDefinition();
            Registration registration = scope.FindOpenGenericRegistration(interfaceType);

            if (registration == null)
                return null;

            Type[] args = interfaceType.GetGenericArguments();
            Type closedImplementation = registration.Implementation.MakeGenericType(args);

            if (closedImplementation.SatisfiesOpenedGenericConstraints(genericDefinition) == false)
                return null;

            ConstructorInfo ctor = closedImplementation.GetInjectableConstructor(s_injectAttributeType);
            Func<object[], object> factory = ctor.GetFactoryMethod();

            return new Registration(closedImplementation, registration.Lifetime, factory, null);
        }

        private object ResolveRegistration(Registration registration, Type interfaceType, ResolutionContext context)
        {
            object instance = GetExistingInstance(registration, interfaceType, context);

            if (instance != null)
                return instance;

            ConstructorInfo ctor = registration.Implementation.GetInjectableConstructor(s_injectAttributeType);
            object[] parameters = ResolveParameters(ctor.GetParameters(), context, registration.Implementation);
            instance = registration.Factory(parameters);

            InjectMembers(instance, registration.Implementation, context);
            StoreInstance(registration, interfaceType, instance, context);

            _invoker.Invoke<IInitializable>(instance);

            return instance;
        }

        private object[] ResolveConstructorWithArguments(Type implementation, ConstructorInfo ctor, ResolutionContext context, params object[] arguments)
        {
            List<object> additionalArguments = arguments == null ? new List<object>() : arguments.ToList();
            ParameterInfo[] parameters = ctor.GetParameters();
            object[] result = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                Type parameterType = parameters[i].ParameterType;
                int selectedArgument = -1;

                for (int j = 0; j < additionalArguments.Count; j++)
                {
                    Type type = additionalArguments[j].GetType();
                    
                    if (parameterType.IsAssignableFrom(type) == false)
                        continue;

                    selectedArgument = j;
                    break;
                }

                if (selectedArgument == -1)
                {
                    result[i] = ResolveMember(parameterType, context, implementation);
                }
                else
                {
                    result[i] = additionalArguments[selectedArgument];
                    additionalArguments.RemoveAt(selectedArgument);
                }
            }

            return result;
        }

        private void InjectMembers(object instance, Type implementation, ResolutionContext context)
        {
            foreach (MemberInfo member in implementation.GetInjectableMembers(s_injectAttributeType))
            {
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(instance, ResolveField(fieldInfo, context, implementation));
                        break;

                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(instance, ResolveProperty(propertyInfo, context, implementation));
                        break;

                    case MethodInfo methodInfo:
                        object[] methodParameters = ResolveParameters(methodInfo.GetParameters(), context, implementation);
                        methodInfo.Invoke(instance, methodParameters);
                        break;
                }
            }
        }

        private object[] ResolveParameters(ParameterInfo[] parameters, ResolutionContext context, Type injectedInto)
        {
            object[] result;
            Type previousRequestType = context.RequestType;
            context.RequestType = injectedInto;

            try
            {
                result = parameters
                    .Select(p => ResolveInternal(p.ParameterType, context))
                    .ToArray();
            }
            finally
            {
                context.RequestType = previousRequestType;
            }

            return result;
        }

        private object ResolveField(FieldInfo fieldInfo, ResolutionContext context, Type injectedInto)
        {
            return ResolveMember(fieldInfo.FieldType, context, injectedInto);
        }

        private object ResolveProperty(PropertyInfo propertyInfo, ResolutionContext context, Type injectedInto)
        {
            return ResolveMember(propertyInfo.PropertyType, context, injectedInto);
        }

        private object ResolveMember(Type interfaceType, ResolutionContext context, Type injectedInto)
        {
            object result;
            Type previousRequestType = context.RequestType;
            context.RequestType = injectedInto;

            try
            {
                result = ResolveInternal(interfaceType, context);
            }
            finally
            {
                context.RequestType = previousRequestType;
            }

            return result;
        }

        private object GetExistingInstance(Registration registration, Type interfaceType, ResolutionContext context)
        {
            return registration.Lifetime switch
            {
                LifeTime.Singleton => registration.Instance ?? context.CurrentScope.GetSingleton(interfaceType),
                LifeTime.Scoped => context.CurrentScope.GetScoped(interfaceType),
                LifeTime.Cached => context.CachedInstances.TryGetValue(interfaceType, out object instance)
                    ? instance : null,
                _ => null
            };
        }

        private void StoreInstance(Registration registration, Type interfaceType, object instance, ResolutionContext context)
        {
            switch (registration.Lifetime)
            {
                case LifeTime.Transient:
                    context.CurrentScope.TrackDisposable(instance);
                    break;

                case LifeTime.Singleton:
                    if (registration.Instance == null)
                        context.CurrentScope.StoreSingleton(interfaceType, instance);

                    break;

                case LifeTime.Cached:
                    context.CachedInstances[interfaceType] = instance;
                    context.CurrentScope.TrackDisposable(instance);
                    break;

                case LifeTime.Scoped:
                    context.CurrentScope.StoreScoped(interfaceType, instance);
                    break;
            }
        }

        private object ResolveDecorators(object instance, Type interfaceType, ResolutionContext context)
        {
            List<DecoratorRegistration> decorators = context.CurrentScope.GetDecoratorRegistrations(interfaceType);

            if (decorators.Count == 0)
                return instance;

            foreach (DecoratorRegistration decorator in decorators)
            {
                object decoratorInstance = null;
                LifeTime lifetime = decorator.Lifetime;

                if (lifetime == LifeTime.Singleton)
                    decoratorInstance = context.CurrentScope.GetSingleton(decorator.DecoratorType);
                else if (lifetime == LifeTime.Scoped)
                    decoratorInstance = context.CurrentScope.GetScoped(decorator.DecoratorType);

                if (decoratorInstance != null)
                {
                    instance = decoratorInstance;
                    continue;
                }

                instance = CreateDecoratorInstance(instance, context, decorator, lifetime);
            }

            return instance;
        }

        private object CreateDecoratorInstance(object instance, ResolutionContext context, DecoratorRegistration decorator, LifeTime lifetime)
        {
            object decoratorInstance;
            object[] parameters = ResolveConstructorWithArguments(decorator.DecoratorType, decorator.Constructor, context, instance);
            decoratorInstance = decorator.Factory(parameters);

            StoreDecorator(context, decoratorInstance, decorator, lifetime);
            InjectMembers(instance, decorator.DecoratorType, context);

            _invoker.Invoke<IInitializable>(instance);

            return decoratorInstance;
        }

        private static void StoreDecorator(ResolutionContext context, object decoratorInstance, DecoratorRegistration decorator, LifeTime lifetime)
        {
            if (lifetime == LifeTime.Singleton)
            {
                context.CurrentScope.StoreSingleton(decorator.DecoratorType, decoratorInstance);
            }
            else if (lifetime == LifeTime.Scoped)
            {
                context.CurrentScope.StoreScoped(decorator.DecoratorType, decoratorInstance);
            }
            else
            {
                if (lifetime == LifeTime.Cached)
                    context.CachedInstances[decorator.DecoratorType] = decoratorInstance;

                context.CurrentScope.TrackDisposable(decoratorInstance);
            }
        }
    }
}
