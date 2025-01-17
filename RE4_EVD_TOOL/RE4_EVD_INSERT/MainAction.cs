using System;
using System.Collections.Generic;
using System.Text;
using SimpleEndianBinaryIO;
using System.IO;

namespace RE4_EVD_INSERT
{
    internal static class MainAction
    {
        public const string Version = "Version 1.1 (2025-01-17)";

        public static void Continue(string[] args, Endianness endianness)
        {
            bool usingBatFile = false;
            int start = 0;
            if (args.Length > 0 && args[0].ToLowerInvariant() == "-bat")
            {
                usingBatFile = true;
                start = 1;
            }

            for (int i = start; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i], endianness);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + args[i]);
                        Console.WriteLine(ex);
                    }
                }
            }

            if (args.Length == 0)
            {
                Console.WriteLine("How to use: drag the file to the executable.");
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4_EVD_TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Finished!!!");
                if (!usingBatFile)
                {
                    Console.WriteLine("Press any key to close the console.");
                    Console.ReadKey();
                }
            }
        }

        private static void Action(string file, Endianness endianness)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".EVD")
            {
                INSERT_IN_EVD.INSERT_IN_EVD_FILE(fileInfo, endianness);
            }
            else if (Extension == ".INSERT_IN_EVD")
            {
                Repack.RepackFile(fileInfo, endianness);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }
    }
}
