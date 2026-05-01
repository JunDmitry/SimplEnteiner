using System;

namespace SimplEnteiner.Core.RegistrationService
{
    internal class DecoratorRegistration
    {
        public DecoratorRegistration(Type interfaceType, Type decoratorType, int order = 0)
        {
            InterfaceType = interfaceType;
            DecoratorType = decoratorType;
            Order = order;
        }

        public Type InterfaceType { get; }
        public Type DecoratorType { get; }
        public int Order { get; set; }
    }
}
