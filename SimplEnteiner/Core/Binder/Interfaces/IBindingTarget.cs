using System;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.Binder.Interfaces
{
    internal interface IBindingTarget
    {
        void Register(BindingBuilder bindingBuilder);
        void RegisterDecorator(BindingBuilder bindingBuilder);
    }
}
