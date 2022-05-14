using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLaunching
{
    public static class StringListCommand
    {
        public static List<String> StringToStringList(string Target)
        {
            List<string> StringList = new List<string>();
            int StartIndex = 0;
            int Leng = 0;
            for (int i = 0; i < Target.Length; i++)
            {
                if ((Target.Substring(i, 1)) == " " || i + 1 == Target.Length)
                {
                    if (i + 1 == Target.Length)
                    {
                        Leng += 1;
                    }
                    StringList.Add(Target.Substring(StartIndex, Leng));
                    StartIndex += Leng += 1;
                    Leng = 0;
                }
                else
                {
                    Leng += 1;
                }
            }
            for (int i = 0; i < StringList.Count; i++)
            {
                if (StringList[i] == string.Empty)
                {
                    StringList.RemoveAt(i);
                    i -= 1;
                }
            }
            StartIndex = 0;
            Leng = 0;
            return StringList;
        }
        public static string StringListToString(List<String> List)
        {
            string str = string.Empty;
            for (int i = 0; i < List.Count; i++)
            {
                if (i + 1 == 0)
                {
                    str += List[i];
                }
                else
                {
                    str += List[i] += " ";
                }
            }
            return str;
        }
    }
}
