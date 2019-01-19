using System.Collections.Generic;
using CodeInjection;

namespace ECS.CodeInjection
{
    public static class ECSInjectionCache
    {
        public static Dictionary<string, List<string>> ComponentsForContexts = new Dictionary<string, List<string>>();
        public static Dictionary<string, TypeDefinitionWrapper>  Components = new Dictionary<string, TypeDefinitionWrapper>();

        public static TypeDefinitionWrapper BaseContextType
        {
            get;
            set;
        }
    }
}