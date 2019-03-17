using System;
using System.CodeDom;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CodeInjection.Experimental
{
    public class Type
    {
        private readonly TypeDefinition _type;

        public string FullName => _type.FullName;

        public Type(TypeDefinition t)
        {
            _type = t;
        }

        internal TypeDefinition GetDefinition()
        {
            return _type;
        }

        public Field GetField(string name)
        {
            var field = _type.Fields.FirstOrDefault(f => f.Name == name);
            return field?.ToWrapper();
        }

        public Field AddField(string name, Type type, FieldAttributes attributes)
        {
            var newField = new FieldDefinition(name, attributes, type._type);
            _type.Fields.Add(newField);
            return newField.ToWrapper();
        }

        public Method GetMethod(string name, params ParameterType[] parameters)
        {
            foreach (var method in _type.Methods)
            {
                if (method.Name != name)
                {
                    continue;
                }

                var index = 0;
                var isMatch = true;

                foreach (var parameter in method.Parameters)
                {
                    if (parameter.ParameterType.ContainsGenericParameter)
                    {
                        continue;
                    }

                    var targetParameter = parameters[index++];
                    var targetParameterName = targetParameter.Type.FullName;
                    targetParameterName = targetParameter.ByRef
                        ? "&" + targetParameterName
                        : targetParameterName;

                    if (parameters.Length <= index || parameter.ParameterType.FullName != targetParameterName)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return method.ToWrapper();
                }
            }

            return null;
        }

        public Method GetMethod(string name, Type returnType, params ParameterType[] parameters)
        {
            var method = GetMethod(name, parameters);
            return method.ReturnType == returnType ? method : null;
        }

        public Method AddMethod(string name, MethodAttributes attributes, Type returnType, params ParameterType[] parameters)
        {
            var method = new MethodDefinition(name, attributes, returnType.GetDefinition());
            foreach (var parameterType in parameters)
            {
                method.Parameters.Add(parameterType.ToDefinition());
            }
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            _type.Methods.Add(method);
            
            return method.ToWrapper();
        }

        public override int GetHashCode()
        {
            return _type.FullName.GetHashCode();
        }

        public static bool operator ==(Type type1, Type type2)
        {
            if (ReferenceEquals(type1, type2))
            {
                return true;
            }

            if (ReferenceEquals(type1, null))
            {
                return false;
            }

            if (ReferenceEquals(type2, null))
            {
                return false;
            }

            return type1.Equals(type2);
        }

        public static bool operator !=(Type type1, Type type2)
        {
            return !(type1 == type2);
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        
    }
}