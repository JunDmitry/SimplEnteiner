using System;
using System.Collections.Generic;

namespace SimplEnteiner.Core.Configuration
{
    [Serializable]
    internal class BindingConfig
    {
        public string InterfaceType { get; set; }
        public string ImplementationType { get; set; }
        public string Lifetime { get; set; }
        public string InstanceJson { get; set; }
        public List<string> ArgumentsJson { get; set; }
        public string Id { get; set; }
        public string Condition { get; set; }
    }
}
