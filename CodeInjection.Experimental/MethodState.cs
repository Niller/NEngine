using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodState
    {
        private int _freeStackPosition;

        private readonly List<MethodVariable> _variables;
        private readonly List<MethodArgument> _arguments;

        private readonly Instruction _lastInstruction;

        private readonly MethodDefinition _method;

        private readonly ILProcessor _ilProcessor;

        public MethodState(MethodDefinition method, Instruction instruction)
        {
            _method = method;
            _lastInstruction = instruction;
            _ilProcessor = _method.Body.GetILProcessor();

            _variables = new List<MethodVariable>();
            foreach (var methodVariable in method.Body.Variables)
            {
                _variables.Add(new MethodVariable(string.Empty, _variables.Count, methodVariable));
            }

            _arguments = new List<MethodArgument>(method.Parameters.Count);
            foreach (var parameter in method.Parameters)
            {
                _arguments.Add(new MethodArgument(parameter));
            }

            _freeStackPosition = method.Body.MaxStackSize;
        }

        public MethodArgument GetArgument(int index)
        {
            return _arguments[index];
        }

        public MethodArgument GetArgument(string name)
        {
            return _arguments.FirstOrDefault(a => a.Name == name);
        }

        public MethodVariable GetVariable(int index)
        {
            return _variables[index];
        }

        public MethodVariable GetVariable(string name)
        {
            return _variables.FirstOrDefault(v => v.Name == name);
        }

        public MethodState AddVariable(string name, Type t)
        {
            var newVariable = new VariableDefinition(t.GetDefinition());
            _method.Body.Variables.Add(newVariable);
            _variables.Add(new MethodVariable(name, _variables.Count, newVariable));
            return this;
        }

        public MethodState AddVariableSet(string name, MethodValue value = null)
        {
            var variable = _variables.FirstOrDefault(v => v.Name == name);

            if (variable != null)
            {
                if (value != null)
                {
                    Insert(value.ToStack());
                }

                Insert(variable.FromStack());
            }

            return this;
        }

        public MethodState ReturnValue(MethodValue value)
        {
            Insert(value.ToStack());
            return this;
        }

        public MethodState Call(Method method, MethodValue returnValue, params MethodValue[] parameters)
        {
            Call(method, parameters);
            if (returnValue != null)
            {
                Insert(returnValue.FromStack());
            }
            return this;
        }

        public MethodState MathOperation(MathOperation operation, params MethodValue[] parameters)
        {
            foreach (var methodValue in parameters)
            {
                Insert(methodValue.ToStack());
            }

            switch (operation)
            {
                case Experimental.MathOperation.Add:
                    Insert(Instruction.Create(OpCodes.Add));
                    break;
                case Experimental.MathOperation.Sub:
                    Insert(Instruction.Create(OpCodes.Sub));
                    break;
                case Experimental.MathOperation.Mul:
                    Insert(Instruction.Create(OpCodes.Mul));
                    break;
                case Experimental.MathOperation.Div:
                    Insert(Instruction.Create(OpCodes.Div));
                    break;
            }

            return this;
        }

        public MethodState If(MethodValue condition)
        {
            throw new NotImplementedException();
        }

        public MethodState EndIf(MethodValue condition)
        {
            throw new NotImplementedException();
        }

        private MethodState Call(Method method, params MethodValue[] parameters)
        {
            foreach (var methodValue in parameters)
            {
                Insert(methodValue.ToStack());
            }

            Insert(method.Call());

            return this;
        }

        private void Insert(IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                Insert(instruction);
            }
        }

        private void Insert(Instruction instruction)
        {
            _ilProcessor.InsertBefore(_lastInstruction, instruction);
        }
    }
}
