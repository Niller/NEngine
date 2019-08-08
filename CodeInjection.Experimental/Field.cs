using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class Field : MethodValue
    {
        private readonly FieldDefinition _definition;
        private readonly FieldReference _reference;

        public Field(FieldReference reference) : base(reference.Name, 0, reference.FieldType.ToWrapper())
        {
            _definition = reference.Resolve();
            _reference = reference;
        }

        public FieldDefinition GetDefinition()
        {
            return _definition;
        }

        public FieldReference GetReference()
        {
            return _reference;
        }

        internal override Instruction ToStack()
        {
            return Instruction.Create(OpCodes.Stfld, _definition);
        }

        internal override Instruction FromStack()
        {
            return Instruction.Create(OpCodes.Ldfld, _definition);
        }
    }
}