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
        private readonly List<ICleanupSystem> _cleanupSystems = new List<ICleanupSystem>();
        private readonly List<IInitializeSystem> _initializeSystems = new List<IInitializeSystem>();

        public Feature(string name)
        {
            Name = name;
        }

        public void AddSystem<T>() where T : ISystem, new()
        {
            AddSystem(new T());
        }

        public void AddSystem(ISystem system)
        {
            if (system is IExecuteSystem executeSystem)
            {
                _executeSystems.Add(executeSystem);
                return;
            }

            if (system is IInitializeSystem initializeSystem)
            {
                _initializeSystems.Add(initializeSystem);
                return;
            }

            if (system is ICleanupSystem cleanupSystem)
            {
                _cleanupSystems.Add(cleanupSystem);
                return;
            }

            throw new NotImplementedException();
        }

        public void RemoveSystem(ISystem system)
        {
            if (system is IExecuteSystem executeSystem)
            {
                _executeSystems.Remove(executeSystem);
                return;
            }

            if (system is IInitializeSystem initializeSystem)
            {
                _initializeSystems.Remove(initializeSystem);
                return;
            }

            if (system is ICleanupSystem cleanupSystem)
            {
                _cleanupSystems.Remove(cleanupSystem);
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

            if (_initializeSystems.Count > 0)
            {
                foreach (var system in _initializeSystems)
                {
                    system.Execute();
                }

                _initializeSystems.Clear();
            }

            foreach (var system in _executeSystems)
            {
                system.Execute();
            }

            foreach (var system in _cleanupSystems)
            {
                system.Execute();
            }
        }
    }
}