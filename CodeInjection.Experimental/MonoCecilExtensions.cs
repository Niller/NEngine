using Mono.Cecil;

namespace CodeInjection.Experimental
{
    public static class MonoCecilExtensions
    {
        public static Type ToWrapper(this TypeReference type)
        {
            return new Type(type.Resolve());
        }

        public static Method ToWrapper(this MethodReference method)
        {
            return new Method(method.Resolve());
        }

        public static Field ToWrapper(this FieldReference field)
        {
            return new Field(field.Resolve());
        }
    }
}