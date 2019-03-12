using System.IO;
using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public static class MonoCecilExtensions
    {
        public static Type ToWrapper(this TypeDefinition type)
        {
            return new Type(type);
        }

        public static Method ToWrapper(this MethodDefinition method)
        {
            return new Method(method);
        }

        public static Field ToField(this FieldDefinition field)
        {
            return new Field(field);
        }
    }


    public static class NameUtilities
    {
        public static string GetNamespace(string fullname)
        {
            return Path.GetDirectoryName(fullname);
        }

        public static string GetName(string fullname)
        {
            return Path.GetFileNameWithoutExtension(fullname);
        }
    }
}