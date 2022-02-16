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
        ObjectTemplates.Root root;
        GameArgsInfo GameArgs;
        JVMArgsInfo JVMArgs;
        string clientJarPath;
        string FinnalCommand;
        string javaPath;
        string classLibPath;
        string nativeLibPath;
        int javaMajorVersion;

        #region ctor
        /// <summary>
        /// Todo:写注释
        /// </summary>
        /// <param name="TargetJSON"></param>
        /// <param name="gameArgs"></param>
        /// <param name="jVMArgs"></param>
        public Launcher(string TargetJSON, GameArgsInfo gameArgs, JVMArgsInfo jVMArgs,string ClassLibPath,string ClientJarPath,string NativeLibPath,string JavaPath)
        {
            root = JsonConvert.DeserializeObject<ObjectTemplates.Root>(LoadJson(TargetJSON).ToString());
            GameArgs = gameArgs;
            JVMArgs = jVMArgs;
            classLibPath = ClassLibPath;
            clientJarPath = ClientJarPath;
            nativeLibPath = NativeLibPath;
            javaPath = JavaPath;
            javaMajorVersion=GetFileversion(JavaPath);
        }
        #endregion
        #region 自动获取系统版本并传参
        String SetOSNameVersion(string Name, string Version)
        {
            return " " + @"""-Dos.name=" + Name + @"""" + " " + "-Dos.version=" + Version;
        }
        /// <summary>
        /// 自动获取版本并且传参数
        /// </summary>
        String AutoSystemVersion()
        {
            var str = "";
            switch (System.Environment.OSVersion.Version.Major + "." + System.Environment.OSVersion.Version.Minor)
            {
                case "6.1":
                    str = SetOSNameVersion("Windows7", "6.1");
                    break;
                case "6.2":
                    str = SetOSNameVersion("Windows 8", "6.2");
                    break;
                case "6.3":
                    str = SetOSNameVersion("Windows 8.1", "6.3");
                    break;
                case "10.0":
                    str = SetOSNameVersion("Windows10", "10.0");
                    break;
            }
            return str;
        }
        #endregion
        #region 拼接参数以及解压Natives
        /// <summary>
        /// 拼接游戏参数用的
        /// </summary>
        /// <param name="allow">是否 allow</param>
        /// <param name="is_demo_user">是否非正版</param>
        /// <param name="has_custom_resolution">是否有自定义分辨率</param>
        /// <param name="width">宽多少</param>
        /// <param name="height">高多少</param>
        String ParseMinecraftArguments(string action,bool is_demo_user,bool has_custom_resolution,int width,int height)
        {
            string minecraftArguments = "";
            for (int i = 0; i < root.arguments.game.Array.Count; i++)
            {
                minecraftArguments = minecraftArguments + " " + root.arguments.game.Array[i].ToString();
            }
            for (int i = 0; i < root.arguments.game.RuleValuePairs.Count; i++)
            {
                if (action == root.arguments.game.RuleValuePairs[i].rules.ActionFeaturesOSGroup.action)
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
            return minecraftArguments;
        }
        String AutoCpCommandLine(ObjectTemplates.Root root,string classLibPath,bool AutoDownload,string ClientJarPath)
        {
            var ELcpCommandLine = "";
            for (int i = 0; i < root.libraries.Array.Count; i++)
            {
                var aa = Path.Combine(classLibPath, root.libraries.Array[i].downloads.artifact.path.ToString().Replace("/", "\\"));
                if (File.Exists(aa) == false && AutoDownload == true)
                {
                    MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                    multiThreadDownloader.GoGoGo(root.libraries.Array[i].downloads.artifact.url.ToString(), 64, aa.Replace(Path.GetFileName(aa), ""));
                }
                ELcpCommandLine = ELcpCommandLine + aa + ";";
            }
            ELcpCommandLine = ELcpCommandLine + ClientJarPath;
            return ELcpCommandLine;
        }
        public enum MyPlatforms
        {
            Windows,Osx,Linux
        }
        void ExportNative(ObjectTemplates.Root root,MyPlatforms platform,string nativePath,string classLibPath)
        {
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
                for (int i = 0; i < path.Count; i++)
                {
                    if (File.Exists(path[i]) == true)
                    {
                        ZipFile.ExtractToDirectory(path[i], nativePath,true);
                    }
                    else if (File.Exists(path[i]) == false)
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
        String AutoRuledJVMArguments(ObjectTemplates.Root root,string action,MyPlatforms Platform,string arch)
        {
            var ELJvmArgumets = "";
            //拼接有规定的参数
            for (int i = 0; i < root.arguments.jvm.RuleValuePairs.Count; i++)
            {
                if (action == root.arguments.jvm.RuleValuePairs[i].rules.ActionFeaturesOSGroup.action.ToString())
                {
                    //当你是 Windows 时
                    if (Platform == MyPlatforms.Windows&&root.arguments.jvm.RuleValuePairs[i].rules.ActionFeaturesOSGroup.os.name=="windows"&& root.arguments.jvm.RuleValuePairs[i].rules.ActionFeaturesOSGroup.os.version==null)
                    {
                        for (int j = 0; j < root.arguments.jvm.RuleValuePairs[i].value.Array.Count; j++)
                        {
                            ELJvmArgumets = ELJvmArgumets+" " + root.arguments.jvm.RuleValuePairs[i].value.Array[j].ToString();
                        }
                    }
                    //当你是 osx 时
                    else if (Platform == MyPlatforms.Osx)
                    {

                    }
                    //当你是 Linux 时
                    else if (Platform == MyPlatforms.Linux)
                    {

                    }
                    if(arch == root.arguments.jvm.RuleValuePairs[i].rules.ActionFeaturesOSGroup.os.arch)
                    {
                        for (int j = 0; j < root.arguments.jvm.RuleValuePairs[i].value.Array.Count; j++)
                        {
                            ELJvmArgumets = ELJvmArgumets + " " + root.arguments.jvm.RuleValuePairs[i].value.Array[j].ToString();
                        }
                    }
                }
            }
            //拼接普通参数
            for (int i = 0; i < root.arguments.jvm.Array.Count; i++)
            {
                ELJvmArgumets = ELJvmArgumets + " " + root.arguments.jvm.Array[i].ToString();
            }
            return ELJvmArgumets;
        }
        #endregion
        #region JSON 文档转 JsonObject
        /// <summary>
        /// 解析JSON，并且把东西传到变量里面。
        /// </summary>
        /// <param name="TargetFile">目标文件</param>
        public JObject LoadJson(string TargetFile)
        {
            StreamReader loader = File.OpenText(TargetFile);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            var jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            loader.Close();
            reader.Close();
            return jsonObject;
        }
        #endregion
        public void Launch(bool AutoDownload,MyPlatforms Platform,string Action,string Arch,int MinMemory,int MaxMemory,string OtherArguments)
        {
            if (javaMajorVersion != root.javaVersion.majorVersion)
            {
                throw new Exception("你的版本不符合Json上的要求");
            }
            FinnalCommand = @""""+javaPath+@"""";
            FinnalCommand += AutoSystemVersion();
            //拼接 classpath 函数
            JVMArgs.classpath = AutoCpCommandLine(root, classLibPath, AutoDownload, clientJarPath);
            ExportNative(root, Platform, nativeLibPath, classLibPath);
            //版本大于等于 1.13 时
            if (root.minecraftArguments == null && root.arguments != null)
            {
                FinnalCommand += AutoRuledJVMArguments(root,Action,Platform,Arch);
                FinnalCommand += " -Xmx"+MaxMemory.ToString()+"M"+" -Xmn"+MinMemory.ToString()+"M";
                FinnalCommand += " " + root.logging.client.argument.ToString();
                FinnalCommand += " " + OtherArguments;
                FinnalCommand += " " + root.mainClass.ToString();
                FinnalCommand += " " + ParseMinecraftArguments("false", false, false, 0, 0);
            }
            else if (root.arguments != null && root.minecraftArguments == null)
            {

            }
            else if (root.arguments == null && root.minecraftArguments == null)
            {
                Console.WriteLine("不支持的版本");
            }
            var ELList = new List<String>
            {
                "${natives_directory}",
                "${launcher_name}",
                "${launcher_version}",
                "${classpath}",
                "${path}",

                "${auth_player_name}",
                "${version_name}",
                "${game_directory}",
                "${assets_root}",
                "${assets_index_name}",
                "${auth_uuid}",
                "${auth_access_token}",
                "${clientid}",
                "${auth_xuid}",
                "${user_type}",
                "${version_type}"
            };
            var ELListToConv = new List<String>
            {
                nativeLibPath,
                JVMArgs.launcher_name,
                JVMArgs.launcher_version.ToString(),
                JVMArgs.classpath,
                JVMArgs.log4jPath,

                GameArgs.auth_player_name,
                GameArgs.version_name,
                GameArgs.game_directory,
                GameArgs.assets_root,
                GameArgs.assets_index_name,
                GameArgs.auth_uuid,
                GameArgs.auth_access_token,
                GameArgs.clientId,
                GameArgs.auth_xuid,
                GameArgs.user_type,
                GameArgs.version_type
            };
            for (int i = 0; i < ELList.Count; i++)
            {
                FinnalCommand = FinnalCommand.Replace(ELList[i], ELListToConv[i]);
            }
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();
            p.StandardInput.WriteLine(FinnalCommand);
        }
        int GetFileversion (string Path)
        {
            int s = FileVersionInfo.GetVersionInfo(Path).FileMajorPart;
            return s;
        }
    }
    #region 参数信息类
    /// <summary>
    /// 游戏参数信息
    /// </summary>
    public class GameArgsInfo
    {
        /// <summary>
        /// 游戏参数信息
        /// </summary>
        /// <param name="Auth_Player_Name">玩家名</param>
        /// <param name="Version_Name">版本名</param>
        /// <param name="Game_Directory">游戏目录</param>
        /// <param name="Assets_Root">资源父目录</param>
        /// <param name="Assets_index_name">资源索引名称</param>
        /// <param name="Auth_Uuid">uuid</param>
        /// <param name="Auth_Access_Token">登录令牌</param>
        /// <param name="ClientId">客户端id</param>
        /// <param name="Auth_Xuid">xuid</param>
        /// <param name="UserType">用户类型</param>
        /// <param name="Version_Type">版本类型</param>
        /// <param name="Resolution_Width">窗口宽</param>
        /// <param name="Resolution_Height">窗口高</param>
        public GameArgsInfo(string Auth_Player_Name, string Version_Name, string Game_Directory, string Assets_Root, string Assets_index_name, string Auth_Uuid, string Auth_Access_Token, string ClientId, string Auth_Xuid, string User_Type, string Version_Type,int Resolution_Width,int Resolution_Height)
        {
            auth_player_name = Auth_Player_Name;
            version_name = Version_Name;
            game_directory = Game_Directory;
            assets_root = Assets_Root;
            assets_index_name = Assets_index_name;
            auth_uuid = Auth_Uuid;
            auth_access_token = Auth_Access_Token;
            clientId = ClientId;
            auth_xuid = Auth_Xuid;
            user_type = User_Type;
            version_type = Version_Type;
            resolution_width = Resolution_Width;
            resolution_height = Resolution_Height;
        }
        public string auth_player_name { get; set; }
        public string version_name { get; set; }
        public string game_directory { get; set; }
        public string assets_root { get; set; }
        public string assets_index_name { get; set; }
        public string auth_uuid { get; set; }
        public string auth_access_token { get; set; }
        public string clientId { get; set; }
        public string auth_xuid { get; set; }
        public string user_type { get; set; }
        public string version_type { get; set; }
        public int resolution_width { get; set; }
        public int resolution_height { get; set; }
    }

    /// <summary>
    /// JVM 参数类
    /// </summary>
    public class JVMArgsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Natives_Directory">native 文件夹路径</param>
        /// <param name="Launcher_Name">启动器名称</param>
        /// <param name="Launcher_Version">启动器版本</param>
        public JVMArgsInfo(string Natives_Directory,string Launcher_Name,Version Launcher_Version,string Log4jPath)
        {
            natives_directory = Natives_Directory;
            launcher_name = Launcher_Name;
            launcher_version = Launcher_Version;
            log4jPath = Log4jPath;
        }
        public string natives_directory { get; set; }
        public string launcher_name { get; set; }
        public Version launcher_version { get; set; }
        public string log4jPath { get; set; }
        public string classpath { get; set; }
    }
    #endregion
}
