using System;
using System.Collections.Generic;

namespace SimplEnteiner.Core.Configuration
{
    [Serializable]
    internal class ScopeConfig
    {
        public List<BindingConfig> ExactBindings = new List<BindingConfig>();
        public List<BindingConfig> OpenGenericBindings = new List<BindingConfig>();
        public List<DecoratorConfig> DecoratorBindings = new List<DecoratorConfig>();
        public List<BindingConfig> ConditionalBindings = new List<BindingConfig>();

        public List<ScopeConfig> Childrens = new List<ScopeConfig>();
    }
}
