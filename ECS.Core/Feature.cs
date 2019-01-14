using System;
using System.Collections.Generic;

namespace ECS
{
    public class Feature
    {
        public string Name
        {
            get;
        }

        public bool Enabled
        {
            get;
            internal set;
        } = true;

        private readonly List<IExecuteSystem> _executeSystems = new List<IExecuteSystem>();

        public Feature(string name)
        {
            Name = name;
        }

        public void AddSystem(ISystem system)
        {
            var executeSystem = system as IExecuteSystem;
            if (system != null)
            {
                _executeSystems.Add(executeSystem);
                return;
            }
            throw new NotImplementedException();
        }

        public void RemoveSystem(ISystem system)
        {
            var executeSystem = system as IExecuteSystem;
            if (system != null)
            {
                _executeSystems.Remove(executeSystem);
                return;
            }
            throw new NotImplementedException();
        }

        public void Execute()
        {
            if (!Enabled)
            {
                return;
            }

            foreach (var system in _executeSystems)
            {
                system.Execute();
            }
        }
    }
}