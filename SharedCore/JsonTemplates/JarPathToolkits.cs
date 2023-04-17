using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CoreLaunching.JsonTemplates
{
    public static class JarPathToolkits
    {
        /// <summary>
        /// 代码抄自 TT702 谢谢 TT702 https://github.com/TT702/Forge-InstallProcessor.NET
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetLibraryFileName(this string name, string librariesPath)
        {
            var extinction = ".jar";
            if (name.Contains("@"))
            {
                extinction = $".{name.Substring(name.LastIndexOf('@') + 1)}";
                name = name.Substring(0, name.LastIndexOf('@'));
            }
            string final = "";
            string[] targets = name.Split(':');
            if (targets.Length < 3) return null;
            else
            {
                var pathBase = string.Join("\\", targets[0].Replace('.', '\\'), targets[1], targets[2], targets[1]) + '-' + targets[2];
                for (var i = 3; i < targets.Length; i++)
                {
                    pathBase = $"{pathBase}-{targets[i]}";
                }

                pathBase = $"{pathBase}{extinction}";
                final= pathBase;
            }

            final = Path.Combine(librariesPath, final);
            return final;
        }

    }
}
