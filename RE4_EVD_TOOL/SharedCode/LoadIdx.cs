using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SharedCode
{
    internal static class LoadIdx
    {
        public static string[] LoadIdxevd(FileInfo fileInfo)
        {
            StreamReader idx = fileInfo.OpenText();

            List<string> files = new List<string>();
            List<string> check = new List<string>();

            int i = 0;
            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();

                if (endLine != null)
                {
                    endLine = endLine.Trim();

                    if (!(endLine.Length == 0
                        || endLine.StartsWith("#")
                        || endLine.StartsWith("\\")
                        || endLine.StartsWith("/")
                        || endLine.StartsWith(":")
                        ))
                    {
                        string validFile = Utils.ValidFileName(endLine, i);
                        string toUpper = validFile.ToUpperInvariant();
                        if (!check.Contains(toUpper))
                        {
                            files.Add(validFile);
                            check.Add(toUpper);
                            i++;
                        }
                    }

                }

            }
            idx.Close();
            return files.ToArray();
        }

    }
}
