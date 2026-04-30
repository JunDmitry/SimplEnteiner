using System;
using SimplEnteiner.Core.LifeScope;

namespace SimplEnteiner.Core.RegistrationService
{
    internal class Registration
    {
        public Registration(Type implementation, LifeTime lifetime, Func<object[], object> factory, object instance)
        {
            Implementation = implementation;
            Lifetime = lifetime;
            Factory = factory;
            Instance = instance;
        }

        public Type Implementation { get; }
        public LifeTime Lifetime { get; }
        public Func<object[], object> Factory { get; }
        public object Instance { get; }
    }
}
