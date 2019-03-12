using System;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public class Method
    {
        public enum DefaultStates
        {
            MethodStart,
            MethodEnd,
        }

        private readonly MethodDefinition _method;

        public string Name => _method.Name;

        public Type ReturnType
        {
            get;
        }

        public ParameterType[] ParameterTypes
        {
            get;
        }

        public Method(MethodDefinition methodDefinition)
        {
            _method = methodDefinition;
            ReturnType = methodDefinition.ReturnType.ToWrapper();

            ParameterTypes = new ParameterType[methodDefinition.Parameters.Count];
            var i = 0;
            foreach (var parameter in methodDefinition.Parameters)
            {
                ParameterTypes[i++] = new ParameterType(parameter.ParameterType.ToWrapper(), 
                    parameter.Name[0] == '&' ? ParameterType.ParameterModifier.Ref : ParameterType.ParameterModifier.None);
            }
        }

        public MethodState GetState(DefaultStates state)
        {
            switch (state)
            {
                case DefaultStates.MethodStart:
                    return new MethodState(_method, 0);
                case DefaultStates.MethodEnd:
                    //Before RET
                    return new MethodState(_method, _method.Body.Instructions.Count - 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
    }
}