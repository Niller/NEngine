using System.IO;
using System.Linq;

namespace CodeInjection.Experimental
{
    public static class NameUtilities
    {
        public static string GetNamespace(string fullname)
        {
            return fullname.Substring(0, fullname.Length - GetName(fullname).Length - 1);
        }

        public static string GetName(string fullname)
        {
            return fullname.Split('.').Last(); ;
        }
    }
}