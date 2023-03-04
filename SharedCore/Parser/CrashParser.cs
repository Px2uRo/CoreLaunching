using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLaunching.Parser
{
    public static class CrashParser
    {
        public static string Parse(string OriginalMessage,string Language)
        {
            string ret ="";
            if(Language == "zh_CN")
            {
                if (OriginalMessage.Contains("加载主类"))
                {
                    ret = "客户端 Jar 文件损坏";
                }
            }
            else
            {
                ret = $"对不起，但是 {Language} 不受支持";
            }
            return ret;
        }
    }
}
