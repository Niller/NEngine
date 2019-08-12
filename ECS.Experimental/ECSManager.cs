using System;
using System.Collections.Generic;
using ECS.Experimental;

namespace ECS
{
    public class ECSManager
    {
        private readonly Dictionary<string, Feature> _features = new Dictionary<string, Feature>();
        protected readonly Dictionary<Type, Context> Contexts = new Dictionary<Type, Context>();

        public void AddContext<T>() where T : Context, new()
        {
            Contexts.Add(typeof(T), new T());
        }

        public T GetContext<T>() where T : Context
        {
            return (T) GetContext(typeof(T));
        }

        public Context GetContext(Type type) 
        {
            if (Contexts.TryGetValue(type, out var context))
            {
                return context;
            }
            throw new ArgumentException($"Context {type.FullName} could not be found");
        }

        public Feature AddFeature(string name)
        {
            var feature = new Feature(name, this);
            if (_features.ContainsKey(name))
            {
                throw new ArgumentException($"Feature with name {name} already exists");
            }

            _features.Add(name, feature);
            return feature;
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

            foreach (var context in Contexts)
            {
                context.Value.ClearChanges();
            }
        }
    }
}