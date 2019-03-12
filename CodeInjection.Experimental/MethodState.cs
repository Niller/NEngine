﻿using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeInjection.Experimental
{
    public class MethodState
    {
        private int _freeStackPosition;

        private List<MethodVariable> _variables;

        private int _instructionIndex;

        private MethodDefinition _method;

        public MethodState(MethodDefinition method, int instructionIndex)
        {
            _method = method;
            _instructionIndex = instructionIndex;

            _variables = new List<MethodVariable>();
            foreach (var methodVariable in method.Body.Variables)
            {
                _variables.Add(new MethodVariable(string.Empty, methodVariable));
            }

            _freeStackPosition = method.Body.MaxStackSize;

        }

        public MethodState AddVariable(string name, Type t)
        {
            var newVariable = new VariableDefinition(t.GetDefinition());
            _method.Body.Variables.Add(newVariable);
            _variables.Add(new MethodVariable(name, newVariable));
            return this;
        }

        public MethodState AddVariableSet(string name, MethodValue value)
        {
            throw new NotImplementedException();
        }

        public MethodState AddVariableSet(string name, int value)
        {
            throw new NotImplementedException();
        }

        public MethodState Call(Method method, params MethodValue[] parameters)
        {
            throw new NotImplementedException();
        }

        public MethodState Call(Method method, MethodValue returnValue = null, params MethodValue[] parameters)
        {
            throw new NotImplementedException();
        }

        public MethodState If(MethodValue condition)
        {
            throw new NotImplementedException();
        }

        public MethodState EndIf(MethodValue condition)
        {
            throw new NotImplementedException();
        }
    }
}
