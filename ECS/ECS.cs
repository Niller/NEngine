using System;
using System.Collections.Generic;

namespace ECS
{
    public static class ECS
    {
        private static Dictionary<string, Context> _contexts = new Dictionary<string, Context>();

        public static Context GetContext(string contextName)
        {
            Context context;
            if (!_contexts.TryGetValue(contextName, out context))
            {
                throw new Exception($"Context with name {contextName} doesn't exist");
            }
            return context;
        }
    }
}