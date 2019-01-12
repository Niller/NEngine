using System;
using System.Collections.Generic;
using ECS;

namespace NEngine.ECS
{
    public static class ECS
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        private static readonly Dictionary<string, BaseContext> Contexts = new Dictionary<string, BaseContext>();

        public static BaseContext GetContext(string contextName)
        {
            BaseContext context;
            if (!Contexts.TryGetValue(contextName, out context))
            {
                throw new Exception($"Context with name {contextName} doesn't exist");
            }
            return context;
        }
    }
}