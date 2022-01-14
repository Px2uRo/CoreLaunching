using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreLaunching
{
    public class Launcher
    {
        public static string JavaPath { get; set; }//指定 Java 路径
        public static string OtherArguments { get; set; }//指定额外参数
        public static string classLibPath { get; set; }

        static string Memory;
        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="MaxMemory">最大内存</param>
        /// <param name="MinMemory">最小内存</param>
        public void SetMemory(int MinMemory, int MaxMemory)
        {
            Memory =" " + "-Xmx" + MaxMemory.ToString() + "m" + " " + "-Xmn" + MinMemory.ToString() + "m";
        }
        static string LauncherInfo;
        /// <summary>
        /// 设置启动器信息
        /// </summary>
        /// <param name="Name">OS名称</param>
        /// <param name="Version">OS版本</param>
        public void SetLauncherInfo(string Name,string Version)
        {
            LauncherInfo = " "+ "-Dminecraft.launcher.brand=" + Name +" " + "-Dminecraft.launcher.version="+Version;
        }

        #region MCArgs
        public static string PlayerName { get; set; }
        public static string GameVersion { get; set; }
        public static string GameDir { get; set; }
        public static string assetsDir { get; set; }
        public static string assetIndex { get; set; }
        public static string uuid { get; set; }
        public static string accessToken { get; set; }
        public static string userProperties { get; set; }
        public static string userType { get; set; }
        public static string TargetJSON { get; set; }
        #endregion

        static string OSNameVersion;
        public void SetOSNameVersion(string Name,string Version)
        {
            OSNameVersion = " " + "-Dos.name=" + @"""" + Name + @"""" + " " + "-Dos.version=" + Version;
        }

        public class libInfo
        {
            public class downloads
            {
                public  class artifact
                {
                    public static string path { get; set; }
                    public static string sha1 { get; set; }
                    public static string size { get; set; }
                    public static string url { get; set; }
                }
            };
            public static string name { get; set; }
        }

        public void Launch()
        {
            StreamReader loader = File.OpenText(TargetJSON);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            var mainClass = " " + jsonObject["mainClass"]; //读取 JSON 里面的 mainClass 项目。
            var minecraftArguments = " " + jsonObject["minecraftArguments"]; //读取 JSON 里面的 minecraftArguments 项目。
            JToken library = jsonObject["libraries"];
            library=library.ToString();
            List<libInfo.downloads.artifact> libInfos = JsonConvert.DeserializeObject<List<libInfo.downloads.artifact>>(library.ToString());

            loader.Close();

            List<string> ELList = new List<string>
            {
            "${auth_player_name}",
            "${version_name}",
            "${game_directory}",
            "${assets_root}",
            "${assets_index_name}",
            "${auth_uuid}",
            "${auth_access_token}",
            "${user_properties}",
            "${user_type}",
            };
            List<string> ELConv = new List<string>
            {
            PlayerName,
            GameVersion,
            GameDir,
            assetsDir,
            assetIndex,
            uuid,
            accessToken,
            userProperties,
            userType,
            };
            for(int i = 0;i< ELList.Count; i++)
            {
                minecraftArguments = minecraftArguments.Replace(ELList[i],ELConv[i]);
            }
            ELList.Clear();
            ELConv.Clear();
            string cpCommandLine = @" -cp """;
            for(int i = 0; i < libInfos.Count; i++)
            {
                cpCommandLine = cpCommandLine + classLibPath + libInfos[i] + ";";
            }
            cpCommandLine = cpCommandLine + @"""";
            string FinalCommand = JavaPath + " " + OtherArguments +OSNameVersion +LauncherInfo+cpCommandLine+mainClass+minecraftArguments + Memory;
            Console.WriteLine(FinalCommand);
        }
    }
}
