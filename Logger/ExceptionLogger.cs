using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class ExceptionLogger
    {
        private static readonly StringBuilder StringBuilder;

        static ExceptionLogger()
        {
            StringBuilder = new StringBuilder();
        }

        public static void Log(string message)
        {
            //StringBuilder.AppendLine($"[{DateTime.Now.ToLongTimeString()}]: {message}");
            StringBuilder.AppendLine(message);
        }

        public static void Save()
        {
            /*
            var fileName = $"{DateTime.Now.ToShortDateString()} + {DateTime.Now.ToLongTimeString()}.txt";
            using (var stream = File.CreateText($"D:\\{fileName}"))
            {
                stream.Write(StringBuilder.ToString());       
            }

            StringBuilder.Clear();
            */
            throw new Exception(StringBuilder.ToString());
        }
    }
}
