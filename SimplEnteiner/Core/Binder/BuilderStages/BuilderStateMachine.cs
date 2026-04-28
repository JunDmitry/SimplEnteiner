using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplEnteiner.Core.Binder.BuilderStages
{
    internal class BuilderStateMachine
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Type, Stage> _stages;

        private Stage _currentStage;

        public BuilderStateMachine(IEnumerable<Stage> stages, Type startStage = null)
        {
            _stages = stages.ToDictionary(s => s.GetType(), s => s);
            startStage ??= typeof(InitialStage);
            _currentStage = _stages[startStage];
        }

        public void ExecuteTo<TStage>()
            where TStage : Stage
        {
            lock (_lock)
            {

                Type toType = typeof(TStage);

                while (_currentStage != null && _currentStage.GetType() != toType)
                {
                    _currentStage.Execute();
                    _currentStage = _currentStage.Next;
                }
            }
        }

        public bool CanTransit<TStage>()
            where TStage : Stage
        {
            lock (_lock)
            {
                return _currentStage.Id < _stages[typeof(TStage)].Id;
            }
        }

        public void ChangeTo<TStage>()
            where TStage : Stage
        {
            lock(_lock)
            { 
                _currentStage = _stages[typeof(TStage)];
            }
        }
    }
}
