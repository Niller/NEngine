using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class Field : MethodValue
    {
        private readonly FieldDefinition _field;

        public Field(FieldDefinition field) : base(field.Name, 0, field.FieldType.ToWrapper())
        {
            _field = field;
        }

        internal override Instruction ToStack()
        {
            return Instruction.Create(OpCodes.Stfld, _field);
        }

        internal override Instruction FromStack()
        {
            return Instruction.Create(OpCodes.Ldfld, _field);
        }
    }
}