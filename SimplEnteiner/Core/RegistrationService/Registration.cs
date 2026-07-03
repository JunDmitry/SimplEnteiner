using System;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.RegistrationService
{
    public class Registration
    {
        public Registration(Type implementation, LifeTime lifetime, Func<object[], object> factory, object instance, object[] arguments = null)
        {
            Implementation = implementation;
            Lifetime = lifetime;
            Factory = factory;
            Instance = instance;
            Arguments = arguments ?? Array.Empty<object>();
        }

        public Type Implementation { get; }
        public LifeTime Lifetime { get; }
        public Func<object[], object> Factory { get; }
        public object Instance { get; }
        public object[] Arguments { get; }
        public Action<object> OnActivation { get; set; }
        public Action<object> OnRelease { get; set; }
    }
}
