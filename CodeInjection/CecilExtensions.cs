using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CodeInjection
{
    public static class CecilExtensions
    {
        public static TypeDefinitionWrapper AsWrapper(this TypeDefinition typeDefinition)
        {
            return new TypeDefinitionWrapper(typeDefinition);
        }

        public static TypeDefinitionWrapper AsWrapper(this TypeReference typeReference)
        {
            return new TypeDefinitionWrapper(typeReference.Resolve());
        }

        public static MethodDefinitionWrapper AsWrapper(this MethodDefinition methodDefinition)
        {
            return new MethodDefinitionWrapper(methodDefinition);
        }

        public static MethodDefinitionWrapper AsWrapper(this MethodReference methodReference)
        {
            return new MethodDefinitionWrapper(methodReference.Resolve());
        }

        public static FieldDefinitionWrapper AsWrapper(this FieldDefinition fieldDefinition)
        {
            return new FieldDefinitionWrapper(fieldDefinition);
        }
    }
}
