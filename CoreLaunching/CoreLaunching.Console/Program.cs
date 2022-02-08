using System;
using Cons = System.Console;
using cl = CoreLaunching;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreLaunching.ObjectTemples;
using System.Collections.Generic;

namespace CoreLaunching.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader loader = File.OpenText(@"I:\Test\1.17.json");
            JsonReader reader = new JsonTextReader(loader);
            JObject Object = (JObject)JToken.ReadFrom(reader);
            JToken token = Object["arguments"];
            JToken token2 = token["game"];
            JToken jvmToken = token["jvm"];
            object[] gamesarray = JsonConvert.DeserializeObject<object[]>(token2.ToString());
            List<game> games = new List<game>();
            List<String> minecraftArguments = new List<String>{};
            for (int i = 0; i < gamesarray.Length; i++)
            {
                try
                {
                    games.Add(JsonConvert.DeserializeObject<game>(gamesarray[i].ToString()));
                }
                catch
                {
                minecraftArguments.Add(gamesarray[i].ToString());
                }
            }
            for(int i = 0;i < games.Count; i++)
            {
                try
                {
                    List<String> arr = JsonConvert.DeserializeObject<List<String>>(games[i].value.ToString());
                    for (int j = 0; j < arr.Count; j++)
                    {
                        minecraftArguments.Add(arr[j]);
                    }
                }
                catch
                {
                    minecraftArguments.Add(games[i].value.ToString());
                }
            }
        }

        /*private static void OldTestCommand1()
        {
            cl.Launcher launcher = new cl.Launcher();
            cl.Launcher.JavaPath = @"""C:\Program Files\Java\jre1.8.0_291\bin\javaw.exe""";
            launcher.SetMemory(256, 1536);
            launcher.SetLauncherInfo("CoreLaunching", "0.1");
            cl.Launcher.classLibPath = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\libraries";
            cl.Launcher.clinetJarPath = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\versions\1.7.10\1.7.10.jar";
            cl.Launcher.nativeLibExportPath = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\bin\Random";
            cl.Launcher.log4jConfigurationFilePath = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\assets\log_configs\client-1.7.xml";
            cl.Launcher.HeapDumpPath = "MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump";
            cl.Launcher.OtherArguments = "-XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M";
            cl.Launcher.assetsDir = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\assets";
            cl.Launcher.log4jConfigurationFilePath = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\assets\log_configs\client-1.7.xml";

            cl.Launcher.PlayerName = "Xia";
            cl.Launcher.GameVersion = "1.7.10";
            cl.Launcher.GameDir = @"C:\Users\Lenovo\AppData\Roaming\.minecraft";
            cl.Launcher.assetIndex = "1.7.10";
            cl.Launcher.uuid = "0000000000000003A98F501BCC514FFA";
            cl.Launcher.accessToken = @"0000000000000003A98F501BCC514FFA";
            cl.Launcher.userProperties = "{}";
            cl.Launcher.userType = "Legacy";
            
            cl.Launcher.TargetJSON = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\versions\1.7.10\1.7.10.json";

            launcher.Launch();

        }*/

        /*private static void OldTestCommand2()
        {
            VersionManifestReader versionManifestReader = new VersionManifestReader();
            versionManifestReader.Load(@"I:\Test\VM.json");
            Cons.WriteLine("目前最新发行版,{0}", versionManifestReader.latestInfo.release.ToString());
            Cons.WriteLine("目前最新测试版,{0}", versionManifestReader.latestInfo.snapshot.ToString());
            Cons.WriteLine("所有已知版本：");
            for (int i = 0; i < versionManifestReader.verInfo.Count; i++)
            {
                Cons.WriteLine(versionManifestReader.verInfo[i].id.ToString());
            }

        }*/
    }
}
 