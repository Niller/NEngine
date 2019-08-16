using System;
using ECSTest;

namespace ECS.Experimental.ConsoleApplication
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int x = 10;
            int y = 10;
            object xObject = x;
            object yObject = y;
            IComparable xComparable = x;
            xComparable.CompareTo(y);
            
            var environment = new TestEnvironment();
        }
    }
}
