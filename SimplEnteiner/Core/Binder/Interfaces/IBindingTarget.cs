using System;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    internal interface IBindingTarget
    {
        void Register(BindingBuilderInternal bindingBuilder);
        void RegisterDecorator(BindingBuilderInternal bindingBuilder);
    }
}
