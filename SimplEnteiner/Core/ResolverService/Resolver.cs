using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplEnteiner.Core.RegistrationService;
using SimplEnteiner.Core.ScopeFeature;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.ResolverService
{
    internal class Resolver : IResolver
    {
        private static readonly Type s_injectAttributeType = Constants.InjectAttributeType;

        public T Resolve<T>(Scope scope) => (T)Resolve(typeof(T), scope);

        public object Resolve(Type interfaceType, Scope scope)
        {
            using ResolutionContext context = new ResolutionContext(scope);
            
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
            else
            {
                Registration registration = GetRegistration(interfaceType, context.CurrentScope).ThrowInvalidIfNull($"No binding found for {interfaceType}");
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
            MethodInfo resolveMethod = typeof(Resolver).GetMethod(nameof(Resolve), new[] { typeof(Type), typeof(Scope) });
            ConstantExpression resolverInstance = Expression.Constant(resolver);
            ConstantExpression typeConst = Expression.Constant(argumentType);
            ConstantExpression scopeConst = Expression.Constant(scope);
            MethodCallExpression methodCall = Expression.Call(resolverInstance, resolveMethod, typeConst, scopeConst);
            UnaryExpression convert = Expression.Convert(methodCall, argumentType);
            LambdaExpression lambda = Expression.Lambda(funcType, convert);

            return lambda.Compile();
        }

        private Registration GetRegistration(Type interfaceType, Scope scope)
        {
            Registration registration = scope.FindExactRegistration(interfaceType);

            if (registration != null)
                return registration;

            if (interfaceType.IsGenericType && (interfaceType.IsGenericTypeDefinition == false))
            {
                registration = GetClosedGenericRegistration(interfaceType, scope);

                if (registration != null)
                    return registration;
            }
            
            if (interfaceType.IsConcreteClass(isIgnoreGeneratedType: true))
            {
                ConstructorInfo ctor = interfaceType.GetInjectableConstructor(s_injectAttributeType);
                Func<object[], object> factory = ctor.GetFactoryMethod();

                return new Registration(interfaceType, LifeScope.LifeTime.Transient, factory, null);
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
            object[] parameters = ctor.GetParameters()
                .Select(p => ResolveInternal(p.ParameterType, context))
                .ToArray();
            instance = registration.Factory(parameters);

            InjectMembers(instance, registration.Implementation, context);
            StoreInstance(registration, interfaceType, instance, context);

            return instance;
        }

        private object[] ResolveConstructorWithArguments(ConstructorInfo ctor, ResolutionContext context, params object[] arguments)
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
                    result[i] = ResolveInternal(parameterType, context);
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
                        fieldInfo.SetValue(instance, ResolveInternal(fieldInfo.FieldType, context));
                        break;

                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(instance, ResolveInternal(propertyInfo.PropertyType, context));
                        break;

                    case MethodInfo methodInfo:
                        object[] methodParameters = methodInfo.GetParameters()
                            .Select(p => ResolveInternal(p.ParameterType, context))
                            .ToArray();
                        methodInfo.Invoke(instance, methodParameters);
                        break;
                }
            }
        }

        private object GetExistingInstance(Registration registration, Type interfaceType, ResolutionContext context)
        {
            return registration.Lifetime switch
            {
                LifeScope.LifeTime.Singleton => registration.Instance ?? context.CurrentScope.GetSingleton(interfaceType),
                LifeScope.LifeTime.Scoped => context.CurrentScope.GetScoped(interfaceType),
                LifeScope.LifeTime.Cached => context.CachedInstances.TryGetValue(interfaceType, out object instance)
                    ? instance : null,
                _ => null
            };
        }

        private void StoreInstance(Registration registration, Type interfaceType, object instance, ResolutionContext context)
        {
            switch (registration.Lifetime)
            {
                case LifeScope.LifeTime.Transient:
                    break;

                case LifeScope.LifeTime.Singleton:
                    if (registration.Instance == null)
                        context.CurrentScope.StoreSingleton(interfaceType, instance);

                    break;

                case LifeScope.LifeTime.Cached:
                    context.CachedInstances[interfaceType] = instance;
                    break;

                case LifeScope.LifeTime.Scoped:
                    context.CurrentScope.StoreScoped(interfaceType, instance);
                    break;
            }
        }

        private object ResolveDecorators(object instance, Type interfaceType, ResolutionContext context)
        {
            // TODO
            // Easy decorators, optimize apply lifetime same as interfaceType lifetime
            // and replace slow Activator.CreateInstance(...) to FactoryMethod(with transient it doesn't make sense)

            List<DecoratorRegistration> decorators = context.CurrentScope.GetDecoratorRegistrations(interfaceType);

            if (decorators.Count == 0)
                return instance;

            foreach (DecoratorRegistration decorator in decorators)
            {
                ConstructorInfo ctor = decorator.DecoratorType.GetInjectableConstructor(s_injectAttributeType);
                object[] parameters = ResolveConstructorWithArguments(ctor, context, instance);
                instance = Activator.CreateInstance(decorator.DecoratorType, parameters);

                InjectMembers(instance, decorator.DecoratorType, context);
            }

            return instance;
        }
    }
}
