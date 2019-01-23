﻿using System;
using System.Collections.Generic;

namespace ECS
{
    public abstract class ECSManager
    {
        private readonly Dictionary<string, Feature> _features = new Dictionary<string, Feature>();
        protected readonly Dictionary<Type, BaseContext> Contexts = new Dictionary<Type, BaseContext>();

        public T GetContext<T>() where T : BaseContext
        {
            if (Contexts.TryGetValue(typeof(T), out var context))
            {
                return (T)context;
            }
            throw new ArgumentException($"Context {typeof(T).FullName} could not be found");
        }

        public void AddFeature(Feature feature)
        {
            var name = feature.Name;
            if (_features.ContainsKey(name))
            {
                throw new ArgumentException($"Feature with name {name} already exists");
            }

            _features.Add(name, feature);
        }

        public Feature GetFeature(string featureName)
        {
            if (!_features.TryGetValue(featureName, out var feature))
            {
                throw new Exception($"Feature with name {featureName} doesn't exist");
            }
            return feature;
        }

        public void RemoveFeature(Feature feature)
        {
            _features.Remove(feature.Name);
        }

        public void DisableFeature(Feature feature)
        {
            feature.Enabled = false;
        }

        public void EnableFeature(Feature feature)
        {
            feature.Enabled = true;
        }

        public void Execute()
        {
            foreach (var feature in _features.Values)
            {
                feature.Execute();
            }
        }
    }
}