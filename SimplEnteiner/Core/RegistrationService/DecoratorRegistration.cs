using System;
using System.Reflection;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.RegistrationService
{
    public class DecoratorRegistration
    {
        public DecoratorRegistration(Type interfaceType, Type decoratorType, int? order, LifeTime lifetime, ConstructorInfo constructor, Func<object[], object> factory)
        {
            InterfaceType = interfaceType;
            DecoratorType = decoratorType;
            Order = order;
            Lifetime = lifetime;
            Constructor = constructor;
            Factory = factory;
        }

        public Type InterfaceType { get; }
        public Type DecoratorType { get; }
        public int? Order { get; set; }
        public LifeTime Lifetime { get; }
        public ConstructorInfo Constructor { get; }
        public Func<object[], object> Factory { get; }
    }
}
