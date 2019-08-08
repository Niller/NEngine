using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public static class MonoCecilExtensions
    {
        public static Type ToWrapper(this TypeReference type)
        {
            return new Type(type);
        }

        public static Method ToWrapper(this MethodReference method)
        {
            return new Method(method);
        }

        public static Field ToWrapper(this FieldReference field, MethodValue source)
        {
            return new Field(field, source);
        }

        public static Property ToWrapper(this PropertyDefinition property)
        {
            return new Property(property);
        }
    }
}