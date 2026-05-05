using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimplEnteiner.Analysis
{
    public class ReachabilityAnalyzer
    {
        private static ReachabilityAnalyzer s_instance = null;

        private ReachabilityAnalyzer()
        {
        }

        public static ReachabilityAnalyzer Instance => s_instance ??= new ReachabilityAnalyzer();

        public HashSet<Type> ComputeReachability(IEnumerable<Type> roots, IReadOnlyDictionary<Type, Type> bindings, Type injectAttribute)
        {
            HashSet<Type> reachable = new HashSet<Type>();
            Queue<Type> queue = new Queue<Type>(roots);

            while (queue.Count > 0)
            {
                Type type = queue.Dequeue();

                if (reachable.Add(type) == false)
                    continue;

                Type implementation = bindings.TryGetValue(type, out Type impl) ? impl : type;
                ConstructorInfo constructor = implementation.GetInjectableConstructor(injectAttribute);

                if (constructor == null)
                    continue;

                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    Type dependencyType = parameter.ParameterType.GetUnderlyingDependencyType();
                    queue.Enqueue(dependencyType);
                }

                foreach (MemberInfo member in implementation.GetInjectableMembers(injectAttribute))
                {
                    switch (member)
                    {
                        case FieldInfo fieldInfo:
                            queue.Enqueue(fieldInfo.FieldType.GetUnderlyingDependencyType());
                            break;

                        case PropertyInfo propertyInfo:
                            queue.Enqueue(propertyInfo.PropertyType.GetUnderlyingDependencyType());
                            break;

                        case MethodInfo methodInfo:
                            foreach (ParameterInfo parameter in methodInfo.GetParameters())
                                queue.Enqueue(parameter.ParameterType.GetUnderlyingDependencyType());

                            break;

                        default:
                            break;
                    }
                }
            }

            return reachable;
        }
    }
}
