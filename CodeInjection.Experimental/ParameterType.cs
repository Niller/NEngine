namespace CodeInjection.Experimental
{
    public class ParameterType 
    {
        public Type Type
        {
            get;
        }

        public enum ParameterModifier
        {
            None,
            In,
            Out,
            Ref
        }

        public ParameterModifier Modifier
        {
            get;
        }

        public ParameterType(Type t, ParameterModifier m = ParameterModifier.None)
        {
            Type = t;
            Modifier = m;
        }
    }
}