using System;
using System.Collections.Generic;
using SimplEnteiner.Core.Binder.BuilderStages;
using SimplEnteiner.Core.LifeScope;
using SimplEnteiner.Utilities;

namespace SimplEnteiner.Core.Binder
{
    internal class BindingBuilderInternal
    {
        private readonly BuilderStateMachine _state;

        public BindingBuilderInternal(Type interfaceType)
        {
            InterfaceType = interfaceType.ThrowIfArgumentNull();

            _state = CreateAndConfigureStateMachine();
        }

        public Type InterfaceType { get; }
        public Type ImplementationType { get; private set; }
        public Func<object> FactoryMethod { get; private set; }
        public object Instance { get; private set; }
        public LifeTime LifeTime { get; private set; }
        public List<object> Arguments { get; } = new List<object>();
        public Type ConditionType { get; private set; }
        public object Id { get; private set; }
        public List<(Type, int)> Decorators { get; } = new List<(Type, int)>();
        public bool HasDecorators => Decorators.Count > 0;
        public bool IsComplete => _state.CurrentIs<FinalStage>();
        public bool IsRegistered { get; private set; }

        public void SetImplementation(Type implementation)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            ImplementationType = implementation.ThrowIfArgumentNull();
            _state.ChangeTo<ImplementationStage>();
        }

        public void SetFactoryMethod(Func<object> factory)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            FactoryMethod = factory.ThrowIfArgumentNull();
            _state.ChangeTo<ImplementationStage>();
        }

        public void SetInstance(object instance)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            Instance = instance.ThrowIfArgumentNull();
            _state.ChangeTo<ImplementationStage>();
        }

        public void SetLifetime(LifeTime lifetime)
        {
            ThrowIfCantTransit<LifetimeStage>("Lifetime");

            _state.ExecuteTo<ImplementationStage>();
            LifeTime = lifetime;
            _state.ChangeTo<LifetimeStage>();
        }

        public void SetCondition<T>()
        {
            SetCondition(typeof(T));
        }

        public void SetCondition(Type type)
        {
            if (_state.CanTransit<ImplementationStage>())
                _state.ExecuteTo<ImplementationStage>();

            ConditionType = type;
            _state.ChangeTo<OptionsStage>();
        }

        public void SetId(object id)
        {
            if (_state.CanTransit<ImplementationStage>())
                _state.ExecuteTo<ImplementationStage>();

            Id = id.ThrowIfArgumentNull();
            _state.ChangeTo<OptionsStage>();
        }

        public void AddDecorator(Type type, int? order = null)
        {
            int concreteOrder = order ?? (HasDecorators ? Decorators[^1].Item2 + 1 : 0);
            
            int indexToInsert = Decorators.FindBinaryIndexMoreThan((type, concreteOrder));
            Decorators.Insert(indexToInsert, (type, concreteOrder));
        }

        internal void ExecuteAllStages()
        {
            _state.ExecuteTo<FinalStage>();
        }

        internal void MarkRegistered()
        {
            IsRegistered = true;
        }

        private BuilderStateMachine CreateAndConfigureStateMachine()
        {
            Stage finalStage = new FinalStage(this, null);
            Stage optionsStage = new OptionsStage(this, finalStage);
            Stage lifetimeStage = new LifetimeStage(this, optionsStage);
            Stage implementationStage = new ImplementationStage(this, lifetimeStage);
            Stage initialStage = new InitialStage(this, implementationStage);

            return new BuilderStateMachine(new Stage[]
            {
                initialStage, implementationStage, lifetimeStage,
                optionsStage, finalStage
            },
            typeof(InitialStage));
        }

        private void ThrowIfCantTransit<T>(string operationName)
            where T : Stage
        {
            if (_state.CanTransit<T>() == false)
                throw new InvalidOperationException($"{operationName} already set!");
        }
    }
}
