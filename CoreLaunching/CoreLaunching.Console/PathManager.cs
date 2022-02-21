using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CoreLaunching.Console.Properties;

namespace CoreLaunching.Console
{
    /// <summary>
    /// 前端的动态测试路径管理类，全部都是完整路径。=。
    /// </summary>
    internal static class PathManager
    {
        /// <summary>
        /// 主要测试的 Json 文件。
        /// </summary>
        public static string MainJsonPath => Path.Combine(Environment.CurrentDirectory, Resources.JsonFileShortName);

        /// <summary>
        /// 主要测试的 Json 文件。
        /// </summary>
        public static string JavawPath => Resources.JavawFullPath;



    }
}
