using System;

namespace SimplEnteiner.Core.Configuration
{
    [Serializable]
    internal class DecoratorConfig
    {
        public string InterfaceType { get; set; }
        public string DecoratorType { get; set; }
        public int Order { get; set; }
        public string Lifetime { get; set; }
    }
}
