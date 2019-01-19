using System;
using System.Collections.Generic;

namespace ECS
{
    public class ECSManager
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly Dictionary<string, BaseContext> _contexts = new Dictionary<string, BaseContext>();
        private readonly Dictionary<string, Feature> _features = new Dictionary<string, Feature>();

        public BaseContext GetContext(string contextName)
        {
            if (!_contexts.TryGetValue(contextName, out var context))
            {
                throw new Exception($"Context with name {contextName} doesn't exist");
            }
            return context;
        }

        public void AddContext(string contextName, BaseContext context)
        {
            if (_features.ContainsKey(contextName))
            {
                throw new ArgumentException($"Context with name {contextName} already exists");
            }

            _contexts.Add(contextName, context);
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