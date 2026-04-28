using System;
using System.Collections.Generic;
using SimplEnteiner.Core.Binder.BuilderStages;
using SimplEnteiner.Core.LifeScope;

namespace SimplEnteiner.Core.Binder
{
    internal class BindingBuilderInternal
    {
        private readonly BuilderStateMachine _state;

        public BindingBuilderInternal(Type interfaceType)
        {
            InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));

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

        public void SetImplementation(Type implementation)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            ImplementationType = implementation ?? throw new ArgumentNullException(nameof(implementation));
            _state.ChangeTo<ImplementationStage>();
        }

        public void SetFactoryMethod(Func<object> factory)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            FactoryMethod = factory ?? throw new ArgumentNullException(nameof(factory));
            _state.ChangeTo<ImplementationStage>();
        }

        public void SetInstance(object instance)
        {
            ThrowIfCantTransit<ImplementationStage>("Implementation");

            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
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

            Id = id ?? throw new ArgumentNullException(nameof(id));
            _state.ChangeTo<OptionsStage>();
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
