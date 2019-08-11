using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public static class MonoCecilUtilities
    {
        public static MethodAttributes GetPublicInterfaceMethodAttributes()
        {
            return MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Final |
                   MethodAttributes.NewSlot | MethodAttributes.Virtual;
        }

        /*
        public static Instruction Create(OpCode opcode)
        {
            return Instruction.Create(opcode);
        }

        public static Instruction Create(OpCode opcode, TypeReference type)
        {
            return Instruction.Create(opcode, type);
        }

        public static Instruction Create(OpCode opcode, CallSite site)
        {
            return Instruction.Create(opcode, site);
        }

        public static Instruction Create(OpCode opcode, MethodReference method)
        {
            return Instruction.Create(opcode, method);
        }

        public static Instruction Create(OpCode opcode, FieldReference field)
        {
            return Instruction.Create(opcode, field);
        }

        public static Instruction Create(OpCode opcode, string value)
        {
            return Instruction.Create(opcode, value);
        }

        public static Instruction Create(OpCode opcode, sbyte value)
        {
            return Instruction.Create(opcode, value);
        }

        public static Instruction Create(OpCode opcode, byte value)
        {
            if (opcode.OperandType == OperandType.ShortInlineVar)
                return Instruction.Create(opcode, this.body.Variables[(int)value]);
            if (opcode.OperandType == OperandType.ShortInlineArg)
                return Instruction.Create(opcode, this.body.GetParameter((int)value));
            return Instruction.Create(opcode, value);
        }

        public Instruction Create(OpCode opcode, int value)
        {
            if (opcode.OperandType == OperandType.InlineVar)
                return Instruction.Create(opcode, this.body.Variables[value]);
            if (opcode.OperandType == OperandType.InlineArg)
                return Instruction.Create(opcode, this.body.GetParameter(value));
            return Instruction.Create(opcode, value);
        }

        public Instruction Create(OpCode opcode, long value)
        {
            return Instruction.Create(opcode, value);
        }

        public Instruction Create(OpCode opcode, float value)
        {
            return Instruction.Create(opcode, value);
        }

        public Instruction Create(OpCode opcode, double value)
        {
            return Instruction.Create(opcode, value);
        }

        public Instruction Create(OpCode opcode, Instruction target)
        {
            return Instruction.Create(opcode, target);
        }

        public Instruction Create(OpCode opcode, Instruction[] targets)
        {
            return Instruction.Create(opcode, targets);
        }

        public Instruction Create(OpCode opcode, VariableDefinition variable)
        {
            return Instruction.Create(opcode, variable);
        }

        public Instruction Create(OpCode opcode, ParameterDefinition parameter)
        {
            return Instruction.Create(opcode, parameter);
        }

    */
    }

}
