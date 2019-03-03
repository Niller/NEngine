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
            Out
        }

        private ParameterModifier _modifier;

        public ParameterType(Type t, ParameterModifier m = ParameterModifier.None)
        {
            Type = t;
            _modifier = m;
        }
    }
}