using System.Collections.Generic;

namespace ECS.CodeInjection
{
    public static class ECSInjectionCache
    {
        public static Dictionary<string, List<string>> ComponentsForContexts = new Dictionary<string, List<string>>();
    }
}