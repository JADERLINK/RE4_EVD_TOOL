using System;
using System.Collections.Generic;
using System.Text;
using SimpleEndianBinaryIO;
using System.IO;

namespace RE4_EVD_TOOL
{
    internal static class MainAction
    {
        public static void Continue(string[] args, Endianness endianness, bool useDiff)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i], endianness, useDiff);
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
            }
        }

        private static void Action(string file, Endianness endianness, bool useDiff)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".EVD")
            {
                Extract.ExtractFile(fileInfo, endianness, useDiff);
            }
            else if (Extension == ".IDXEVD")
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
