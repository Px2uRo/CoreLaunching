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
        /// <param name="Name"></param>
        /// <param name="Version"></param>
        public void SetLauncherInfo(string Name,string Version)
        {
            LauncherInfo = " "+ "-Dminecraft.launcher.brand=" + Name +" " + "-Dminecraft.launcher.version="+Version;
        }
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
        Dictionary<string, string> ELTable = new Dictionary<string, string>
        {
            {"${auth_player_name}", PlayerName},
            {"${version_name}",GameVersion },
            {"${game_directory}",GameDir },
            {"${assets_root}",assetsDir},
            {"${assets_index_name}",assetIndex },
            {"${auth_uuid}",uuid },
            {"${auth_access_token}",accessToken },
            {"${user_properties}",userProperties },
            {"${user_type}",userType }
        };
        public void Launch()
        {
            StreamReader loader = File.OpenText(TargetJSON);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            var mainClass = " " + jsonObject["mainClass"]; //读取 JSON 里面的 mainClass 项目。
            var minecraftArguments = " " + jsonObject["minecraftArguments"]; //读取 JSON 里面的 minecraftArguments 项目。
            loader.Close();

            minecraftArguments.Replace()

            string FinalCommand = JavaPath + " " + OtherArguments + Memory +LauncherInfo+mainClass+minecraftArguments;
            Console.WriteLine(FinalCommand);
        }
    }
}
