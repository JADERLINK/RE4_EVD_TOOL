using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_EVD_TOOL_BASIC_BIG_ENDIAN
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("RE4_EVD_TOOL_BASIC_BIG_ENDIAN");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine("Version 1.0 (2024-12-27)");
            Console.WriteLine("");

            RE4_EVD_TOOL.MainAction.Continue(args, SimpleEndianBinaryIO.Endianness.BigEndian, false);
        }
    }
}
