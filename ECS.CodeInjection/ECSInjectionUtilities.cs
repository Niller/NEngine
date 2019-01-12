using System.Linq;

namespace ECS.CodeInjection
{
    public static class ECSInjectionUtilities 
    {
        public static string GetTypeName(string fullname)
        {
            return fullname.Split('.').Last();
        }
    }
}