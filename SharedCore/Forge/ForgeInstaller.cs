using CoreLaunching.PinKcatDownloader;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLaunching.Forge
{
    public static class ForgeInstaller
    {
        private static void Install(string baseJson,string forgeJson, ParseType type,string newVersionFolder = "")
        {
            Install(baseJson, type, forgeJson, type,newVersionFolder);
        }

        private static void Install(string baseJson,ParseType type1,string forgeJson,ParseType type2, string newVersionFolder = "")
        {
            if (string.IsNullOrEmpty(newVersionFolder))
            {
                
            }
            else
            {

            }
        }

        public static void CreateProcess()
        {

        }
    }
}
