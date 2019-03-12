using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Field : MethodValue
    {
        private FieldDefinition _field;

        public Field(FieldDefinition field) : base(field.Name, 0, field.FieldType.ToWrapper())
        {
            _field = field;
        }
    }
}