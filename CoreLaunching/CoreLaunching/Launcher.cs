﻿using System;
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
        public class downloads
        {
            public artifact artifact;
        }
        public class artifact
        {
            public string path;
            public string sha1;
            public string size;
            public string url;
        }
        public class libInfo
        {
            public downloads downloads;
            public string name;
        }
        public void Launch()
        {
            StreamReader loader = File.OpenText(TargetJSON);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            var mainClass = " " + jsonObject["mainClass"]; //读取 JSON 里面的 mainClass 项目。
            var minecraftArguments = " " + jsonObject["minecraftArguments"]; //读取 JSON 里面的 minecraftArguments 项目。
            JToken library = jsonObject["libraries"];
            List<libInfo> libInfos = JsonConvert.DeserializeObject<List<libInfo>>(library.ToString());

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
                cpCommandLine = cpCommandLine + classLibPath + libInfos[i].downloads.artifact.path + ";";
            }
            cpCommandLine = cpCommandLine + @"""";
            string FinalCommand = JavaPath + " " + OtherArguments +OSNameVersion +LauncherInfo+cpCommandLine+mainClass+minecraftArguments + Memory;
            Console.WriteLine(FinalCommand);
        }
    }
}
