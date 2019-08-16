using System.Linq;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Attribute
    {
        private readonly CustomAttribute _attribute;

        public Attribute(CustomAttribute attribute)
        {
            _attribute = attribute;
        }

        public object[] GetArgumentsValues()
        {
            return _attribute.ConstructorArguments.Select(p => p.Value).ToArray();
        }
    }
}