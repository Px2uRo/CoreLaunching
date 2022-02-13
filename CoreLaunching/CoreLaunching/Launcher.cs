using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreLaunching;
using System.IO.Compression;
using System.Diagnostics;

namespace CoreLaunching
{
    public class Launcher
    {
        CoreLaunching.ObjectTemplates.Root root;
        public string FinnalCommand;
        #region JVM&DebugArguments
        public static string JavaPath = @"""${JavaPath}""";//指定 Java 路径
        public static string OtherArguments = " ${OtherArguments}";//指定额外参数
        public static string classLibPath { get; set; }
        public static string clinetJarPath { get; set; }
        public static string HeapDumpPath=" ${HeapDumpPath}";
        public static string loggingArgs = " ${logging}";
        public static string nativeLibExportPath = " -Djava.library.path=${natives_directory}";
        public static string Memory { get; set; }
        static string LauncherInfo;
        static string DebugOSNameVersion;
        #endregion
        #region ELList
        List<string> ELList = new List<string>
            {
            "${OtherArguments}",
            "${natives_directory}",
            "${HeapDumpPath}",
            "${JavaPath}",
            "${auth_player_name}",
            "${version_name}",
            "${game_directory}",
            "${assets_root}",
            "${assets_index_name}",
            "${auth_uuid}",
            "${auth_access_token}",
            "${user_properties}",
            "${user_type}",
            "${cp}",
            "${mainClass}",
            "${minecraftArgumets}",
            "${logging}"
            };
        List<string> ELConv = new List<string>
            {
            OtherArguments,
            nativeLibExportPath,
            HeapDumpPath,
            JavaPath,
            PlayerName,
            GameVersion,
            GameDir,
            assetsDir,
            assetIndex,
            uuid,
            accessToken,
            userProperties,
            userType,
            cpCommandLine,
            mainClass,
            minecraftArguments,
            loggingArgs
            };
        #endregion
        #region MCArgs
        static string minecraftArguments = "${minecraftArgumets}";
        public static string mainClass=" ${mainClass}";
        public static string cpCommandLine = @" -cp ${cp}";
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
        #region ctor
        /// <summary>
        /// 核心
        /// </summary>
        /// <param name="MaxMemory">最大内存</param>
        /// <param name="MinMemory">最小内存</param>
        /// <param name="Name">启动器名称</param>
        /// <param name="Version">启动器版本</param>
        public Launcher(int MinMemory, int MaxMemory, string Name, string Version,string TargetJson,string CpPath)
        {
            AutoSystemVersion();
            Memory = " -Xmx" + MaxMemory.ToString() + "m -Xmn" + MinMemory.ToString() + "m";
            LauncherInfo = " -Dminecraft.launcher.brand=" + Name + " -Dminecraft.launcher.version=" + Version;
            FinnalCommand = JavaPath + OtherArguments + HeapDumpPath + DebugOSNameVersion + nativeLibExportPath + LauncherInfo + cpCommandLine + Memory + loggingArgs + mainClass + minecraftArguments;
            root = JsonConvert.DeserializeObject<CoreLaunching.ObjectTemplates.Root>(LoadJson(TargetJson).ToString());
            classLibPath = CpPath;
            nativeLibExportPath = "";
        }

        #endregion
        #region 自动获取系统版本并传参
        void SetOSNameVersion(string Name, string Version)
        {
            DebugOSNameVersion = " " + @"""-Dos.name=" + Name + @"""" + " " + "-Dos.version=" + Version;
        }
        /// <summary>
        /// 自动获取版本并且传参数
        /// </summary>
        void AutoSystemVersion()
        {
            switch (System.Environment.OSVersion.Version.Major + "." + System.Environment.OSVersion.Version.Minor)
            {
                case "6.1":
                    SetOSNameVersion("Windows7", "6.1");
                    break;
                case "6.2":
                    SetOSNameVersion("Windows 8", "6.2");
                    break;
                case "6.3":
                    SetOSNameVersion("Windows 8.1", "6.3");
                    break;
                case "10.0":
                    SetOSNameVersion("Windows10", "10.0");
                    break;
            }
        }
        #endregion
        #region 解析 JSON
        /// <summary>
        /// 解析JSON，并且把东西传到变量里面。
        /// </summary>
        public JObject LoadJson(string Target)
        {
            StreamReader loader = File.OpenText(Target);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            var jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            return jsonObject;
        }
        #endregion
        public void Launch(string javaPath,string nativePath,string allow,bool is_demo_user,bool has_custom_resolution,int width,int height)
        {
            JavaPath = javaPath;
            minecraftArguments = "";
            cpCommandLine = "";
            if (root.minecraftArguments == null&&root.arguments!=null)
            {
                ParseMinecraftArguments(allow, is_demo_user, has_custom_resolution, width, height);
                cpCommandLine = AutoCpCommandLine(root);
                ExportNative(root,0,nativePath);
            }
            else if (root.arguments != null && root.minecraftArguments == null)
            {
                
            }
            else if (root.arguments == null && root.minecraftArguments == null)
            {
                Console.WriteLine("不支持的版本");
            }
            for (int i = 0; i < ELList.Count; i++)
            {
                minecraftArguments.Replace(ELList[i],ELConv[i]);
            }
        }
        #region 拼接参数以及解压Natives
        /// <summary>
        /// 拼接游戏参数用的
        /// </summary>
        /// <param name="allow">是否 allow</param>
        /// <param name="is_demo_user">是否非正版</param>
        /// <param name="has_custom_resolution">是否有自定义分辨率</param>
        /// <param name="width">宽多少</param>
        /// <param name="height">高多少</param>
        void ParseMinecraftArguments(string allow,bool is_demo_user,bool has_custom_resolution,int width,int height)
        {
            for (int i = 0; i < root.arguments.game.Array.Count; i++)
            {
                minecraftArguments = minecraftArguments + " " + root.arguments.game.Array[i].ToString();
            }
            for (int i = 0; i < root.arguments.game.RuleValuePairs.Count; i++)
            {
                if (allow == root.arguments.game.RuleValuePairs[i].rules.ActionFeaturesOSGroup.action)
                {
                    if (is_demo_user == root.arguments.game.RuleValuePairs[i].rules.ActionFeaturesOSGroup.features.is_demo_user && true == root.arguments.game.RuleValuePairs[i].rules.ActionFeaturesOSGroup.features.is_demo_user)
                    {
                        for (int j = 0; j < root.arguments.game.RuleValuePairs[i].value.Array.Count; j++)
                        {
                            minecraftArguments = minecraftArguments + " " + root.arguments.game.RuleValuePairs[i].value.Array[j].ToString();
                        }
                    }
                    else if (has_custom_resolution == root.arguments.game.RuleValuePairs[i].rules.ActionFeaturesOSGroup.features.has_custom_resolution)
                    {
                        for (int j = 0; j < root.arguments.game.RuleValuePairs[i].value.Array.Count; j++)
                        {
                            minecraftArguments = minecraftArguments + " " + root.arguments.game.RuleValuePairs[i].value.Array[j].ToString();
                        }
                    }
                }
            }
        }
        
        string AutoCpCommandLine(ObjectTemplates.Root root)
        {
            string cpCommandLine = "";
            for (int i = 0; i < root.libraries.Array.Count; i++)
            {
                cpCommandLine = cpCommandLine + Path.Combine(classLibPath, root.libraries.Array[i].downloads.artifact.path.ToString().Replace("/","\\")) + ";";
            }
            return cpCommandLine;
        }
        public enum MyPlatforms
        {
            Windows,Osx,Linux
        }
        void ExportNative(ObjectTemplates.Root root,MyPlatforms platform,string nativePath)
        {
            nativeLibExportPath = nativePath;
            List<string> path = new List<string>();
            List<string> urls = new List<string>();
            if(platform == MyPlatforms.Windows)
            {
                for (int i = 0; i < root.libraries.Array.Count; i++)
                {
                    if (root.libraries.Array[i].downloads.classifiers != null)
                    {
                        if (root.libraries.Array[i].downloads.classifiers.natives_windows!=null)
                        {
                            path.Add(Path.Combine(classLibPath, root.libraries.Array[i].downloads.classifiers.natives_windows.path.ToString().Replace("/", "\\")));
                            urls.Add(root.libraries.Array[i].downloads.classifiers.natives_windows.url);
                        }
                    }
                }
                System.Threading.Thread[] thr = new System.Threading.Thread[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    if (File.Exists(path[i]) == true)
                    {
                        ZipFile.ExtractToDirectory(path[i], nativeLibExportPath,true);
                    }
                    else if (File.Exists(path[i]) != true)
                    {
                        MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                        multiThreadDownloader.GoGoGo(urls[i], 64, path[i].Replace(Path.GetFileName(path[i]), ""));
                    }
                }
            }
            else if(platform == MyPlatforms.Osx)
            {

            }
            else if (platform == MyPlatforms.Linux)
            {

            }
        }
        #endregion
    }
}
