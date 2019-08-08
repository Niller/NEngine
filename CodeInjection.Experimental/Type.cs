using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Logger;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CodeInjection.Experimental
{
    public class Type
    {
        private readonly TypeDefinition _definition;
        private readonly TypeReference _reference;

        public string FullName => _definition.FullName;

        public Type(TypeReference reference)
        {
            _definition = reference.Resolve();
            _reference = reference;
        }

        internal TypeDefinition GetDefinition()
        {
            return _definition;
        }

        public TypeReference GetReference()
        {
            return _reference;
        }

        public Field GetField(string name)
        {
            var field = _definition.Fields.FirstOrDefault(f => f.Name == name);
            return field?.ToWrapper();
        }

        public Field AddField(string name, Type type, FieldAttributes attributes)
        {
            var newField = new FieldDefinition(name, attributes, type.GetReference());
            _definition.Fields.Add(newField);
            return newField.ToWrapper();
        }

        public Field AddField(string name, TypeReference type, FieldAttributes attributes)
        {
            var newField = new FieldDefinition(name, attributes, type);
            _definition.Fields.Add(newField);
            return newField.ToWrapper();
        }

        public bool HasAttribute(Type type)
        {
            return _definition.HasCustomAttributes && _definition.CustomAttributes.Any(attr => attr.AttributeType.FullName == type.FullName);
        }

        public IEnumerable<Property> GetProperties()
        {
            return _definition.Properties.Select(p => p.ToWrapper());
        }

        public Method GetMethod(string name, params ParameterType[] parameters)
        {
            foreach (var method in _definition.Methods)
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

                    var targetParameter = parameters[index];
                    var targetParameterName = targetParameter.Type.FullName;
                    targetParameterName = targetParameter.ByRef
                        ? "&" + targetParameterName
                        : targetParameterName;

                    if (parameters.Length <= index || parameter.ParameterType.FullName != targetParameterName)
                    {
                        isMatch = false;
                        break;
                    }

                    index++;
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

            _definition.Methods.Add(method);
            
            return method.ToWrapper();
        }

        public ParameterType ToParameterType(string name = "", bool byRef = false, ParameterAttributes attributes = ParameterAttributes.None)
        {
            return new ParameterType(name, this, byRef, attributes);
        }

        public override int GetHashCode()
        {
            return _definition.FullName.GetHashCode();
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