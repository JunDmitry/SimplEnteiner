using System;
using System.Reflection;

namespace SimplEnteiner.Core.RegistrationService
{
    internal class DecoratorRegistration
    {
        public DecoratorRegistration(Type interfaceType, Type decoratorType, int order, ConstructorInfo constructor, Func<object[], object> factory)
        {
            InterfaceType = interfaceType;
            DecoratorType = decoratorType;
            Order = order;
            Constructor = constructor;
            Factory = factory;
        }

        public Type InterfaceType { get; }
        public Type DecoratorType { get; }
        public int Order { get; set; }
        public ConstructorInfo Constructor { get; }
        public Func<object[], object> Factory { get; }
    }
}
