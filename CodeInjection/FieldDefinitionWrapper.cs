using Mono.Cecil;

namespace CodeInjection
{
    public class FieldDefinitionWrapper
    {
        private readonly FieldDefinition _fieldDefinition;

        public FieldDefinitionWrapper(FieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
        }

        internal FieldDefinition GetDefinition()
        {
            return _fieldDefinition;
        }
    }
}