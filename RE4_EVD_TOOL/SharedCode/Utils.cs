using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode
{
    internal static class Utils
    {
        public static string ValidFileName(string fileName, int index)
        {
            string res = "";

            foreach (char c in fileName.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(c)
                    || c == '.'
                    || c == ' '
                    || c == '-'
                    || c == '+'
                    || c == '_'
                    || c == '='
                    || c == ','
                    || c == '&'
                    || c == '$'
                    || c == '\\'
                    || c == '/'
                    )
                {
                    res += c;
                }
            }

            if (res.Length > 0 && res[res.Length - 1] == '.')
            {
                res += index + ".error";
            }
            if (res.Length == 0)
            {
                res = $"null.{index}.error";
            }
            return res;
        }
    }
}
