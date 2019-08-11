using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class ParameterType 
    {
        public Type Type
        {
            get;
        }

        public string Name
        {
            get;
        }

        public ParameterAttributes Attributes
        {
            get;
        }

        public bool ByRef
        {
            get;
        }

        public ParameterType(string name, Type t, bool byRef = false, ParameterAttributes attr = ParameterAttributes.None)
        {
            Type = t;
            Attributes = attr;
            Name = name;
            ByRef = byRef;
        }

        public ParameterDefinition ToDefinition()
        {
            return new ParameterDefinition(Name, Attributes, Type.GetReference());
        }
    }
}