using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class Field : MethodValue
    {
        private readonly FieldDefinition _definition;
        private readonly FieldReference _reference;
        private readonly MethodValue _source;

        public Field(FieldReference reference, MethodValue source) : base(reference.Name, 0, reference.FieldType.ToWrapper())
        {
            _definition = reference.Resolve();
            _reference = reference;
            _source = source;
        }

        public FieldDefinition GetDefinition()
        {
            return _definition;
        }

        public FieldReference GetReference()
        {
            return _reference;
        }

        internal override IEnumerable<Instruction> ToStack()
        {
            foreach (var instruction in _source.ToStack())
            {
                yield return instruction;
            }
            yield return Instruction.Create(OpCodes.Ldfld, _definition);
        }

        internal override IEnumerable<Instruction> FromStack()
        {
            yield return Instruction.Create(OpCodes.Stfld, _definition);
        }
    }
}