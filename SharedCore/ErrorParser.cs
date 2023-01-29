using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCore
{
    /// <summary>
    /// 崩溃分析器
    /// </summary>
    public static class CrashParser
    {
        /// <summary>
        /// 解析错误
        /// </summary>
        /// <param name="OriginalMsg">原来的消息</param>
        /// <param name="LanguageCode">语言代码，形如zh_CN</param>
        /// <returns></returns>
        public static string Parse(string OriginalMsg,string LanguageCode)
        {
            var ret = string.Empty;
            if (LanguageCode == "zh_CN")
            {
                if (OriginalMsg.Contains("找不到或无法加载主类"))
                {
                    ret = "客户端 Jar 文件损坏";
                }
            }
            else
            {
                ret= $"对不起但是只支持中文";
            }
            return ret;
        }
    }
}
