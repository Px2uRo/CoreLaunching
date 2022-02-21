using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace CoreLaunching
{
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
        public GameArgsInfo(string Auth_Player_Name, string Game_Directory, string Assets_Root, string Asset_Index_JSON, string Auth_Uuid, string Auth_Access_Token, string ClientId, string Auth_Xuid, string User_Type, string Version_Type, int Resolution_Width, int Resolution_Height)
        {
            StreamReader loader = File.OpenText(Asset_Index_JSON);
            JsonTextReader reader = new JsonTextReader(loader);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            ObjectTemplates.Root root = JsonConvert.DeserializeObject<ObjectTemplates.Root>(jsonObject.ToString());
            loader.Close();
            reader.Close();
            Auth_player_name = Auth_Player_Name;
            Version_name = root.Id;
            Game_directory = Game_Directory;
            Assets_root = Assets_Root;
            Assets_index_name = root.AssetIndex.Id;
            Auth_uuid = Auth_Uuid;
            Auth_access_token = Auth_Access_Token;
            this.ClientId = ClientId;
            Auth_xuid = Auth_Xuid;
            User_type = User_Type;
            Version_type = Version_Type;
            Resolution_width = Resolution_Width;
            Resolution_height = Resolution_Height;
        }
        public string Auth_player_name { get; set; }
        public string Version_name { get; set; }
        public string Game_directory { get; set; }
        public string Assets_root { get; set; }
        public string Assets_index_name { get; set; }
        public string Auth_uuid { get; set; }
        public string Auth_access_token { get; set; }
        public string ClientId { get; set; }
        public string Auth_xuid { get; set; }
        public string User_type { get; set; }
        public string Version_type { get; set; }
        public int Resolution_width { get; set; }
        public int Resolution_height { get; set; }
    }

    /// <summary>
    /// JVM 基础参数信息类。
    /// </summary>
    public class JVMArgsInfo
    {
        /// <summary>
        /// JVM 基础参数信息类。
        /// </summary>
        /// <param name="Natives_Directory">native 文件夹路径</param>
        /// <param name="Launcher_Name">启动器名称</param>
        /// <param name="Launcher_Version">启动器版本</param>
        public JVMArgsInfo(string Natives_Directory, string Launcher_Name, Version Launcher_Version)
        {
            Natives_directory = Natives_Directory;
            Launcher_name = Launcher_Name;
            Launcher_version = Launcher_Version;
        }
        public string Natives_directory { get; set; }
        public string Launcher_name { get; set; }
        public Version Launcher_version { get; set; }
        public string Classpath { get; set; }
    }
    #endregion

    /// <summary>
    /// CoreLaunching 启动部分
    /// </summary>
    public class Launcher
    {
        #region Enum
        public enum MyPlatforms
        {
            Windows, Osx, Linux
        }
        #endregion

        #region Launcher 类里面可以复用的变量
        private ObjectTemplates.Root root;
        private GameArgsInfo GameArgs;
        private JVMArgsInfo JVMArgs;
        private string clientJarPath;
        private string FinnalCommand;
        private string javaPath;
        private string classLibPath;
        private string nativeLibPath;
        private int javaMajorVersion;
        #endregion

        #region ctor(构造函数)
        /// <summary>
        /// CoreLaunching.Launcher 的构造函数
        /// </summary>
        /// <param name="TargetJSON">目标 JSON</param>
        /// <param name="JavaPath">目标 Java</param>
        /// <param name="jVMArgs">请 new 一个 CoreLaunching.JVMArgsInfo 类</param>
        /// <param name="ClassLibPath">Cp 参数的根目录</param>
        /// <param name="ClientJarPath">客户端 (游戏 Jar) 的目录</param>
        /// <param name="NativeLibPath">Native 库的路径</param>
        /// <param name="gameArgs">请 new 一个 CoreLaunching.GameArgsInfo 类</param>
        public Launcher(string TargetJSON, string JavaPath, JVMArgsInfo jVMArgs, string ClassLibPath, string ClientJarPath, string NativeLibPath, GameArgsInfo gameArgs)
        {
            root = JsonConvert.DeserializeObject<ObjectTemplates.Root>(LoadJson(TargetJSON).ToString());
            GameArgs = gameArgs;
            JVMArgs = jVMArgs;
            classLibPath = ClassLibPath;
            clientJarPath = ClientJarPath;
            nativeLibPath = NativeLibPath;
            javaPath = JavaPath;
            javaMajorVersion = FileVersionInfo.GetVersionInfo(JavaPath).FileMajorPart;
        }
        #endregion

        #region 自动获取系统版本并传参
        string SetOSNameVersion(string Name, string Version)
        {
            return " -Dos.name=" + Name + " -Dos.version=" + Version;
        }
        /// <summary>
        /// 自动获取版本并且传参数
        /// </summary>
        string AutoSystemVersion()
        {
            var str = "";
            switch (Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor)
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
        string ParseMinecraftArguments(string action, bool is_demo_user, bool has_custom_resolution, int width, int height)
        {
            string minecraftArguments = "";
            var game = root.Arguments.Game;

            for (int i = 0; i < game.Array.Count; i++)
            {
                minecraftArguments = minecraftArguments + " " + game.Array[i].ToString();
            }
            for (int i = 0; i < game.RuleValuePairs.Count; i++)
            {
                var ruleValuePair = game.RuleValuePairs[i];
                var osGroup = ruleValuePair.Rules.ActionFeaturesOSGroup;
                var features = osGroup.Features;
                var array = ruleValuePair.Value.Array;
                if (action == osGroup.Action)
                {
                    if (is_demo_user == features.Is_demo_user && true == features.Is_demo_user)
                    {
                        for (int j = 0; j < array.Count; j++)
                        {
                            minecraftArguments = $"{minecraftArguments} {array[j]}";
                        }
                    }
                    else if (has_custom_resolution == features.Has_custom_resolution)
                    {
                        for (int j = 0; j < array.Count; j++)
                        {
                            // minecraftArguments = minecraftArguments + " " + array[j].ToString();
                            minecraftArguments = $"{minecraftArguments} {array[j]}";
                        }
                    }
                }
            }
            return minecraftArguments;
        }
        string AutoCpCommandLine(ObjectTemplates.Root root, string classLibPath, bool AutoDownload, string ClientJarPath, MyPlatforms Platform)
        {
            var ELcpCommandLine = string.Empty;
            var array = root.Libraries.Array;
            if (Platform == MyPlatforms.Windows)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    var download = array[i].Downloads;
                    var aa = Path.Combine(classLibPath, download.Artifact.Path.Replace("/", @"\"));
                    if (download.Classifiers == null && array[i].Rules == null)
                    {
                        ELcpCommandLine += $"{aa};";
                        if (File.Exists(aa) == false && AutoDownload == true)
                        {
                            MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                            multiThreadDownloader.GoGoGo(download.Artifact.Url, 64, aa.Replace(Path.GetFileName(aa), ""));
                        }
                    }
                    else if (download.Artifact != null && download.Classifiers == null && array[i].Rules.Count == 2)
                    {
                        ELcpCommandLine += aa + ";";
                        if (File.Exists(aa) == false && AutoDownload == true)
                        {
                            MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                            multiThreadDownloader.GoGoGo(download.Artifact.Url, 64, aa.Replace(Path.GetFileName(aa), ""));
                        }
                    }
                }
            }
            else if (Platform == MyPlatforms.Osx)
            {
                // TODO OSX
                throw new NotImplementedException("OSX 版本暂时无具体实现");
            }
            else if (Platform == MyPlatforms.Linux)
            {
                // TODO Lunix
                throw new NotImplementedException("Linux 版本暂时无具体实现");
            }
            ELcpCommandLine = @"""" + ELcpCommandLine + ClientJarPath + @"""";
            return ELcpCommandLine;
        }

        void ExportNative(ObjectTemplates.Root root, MyPlatforms platform, string nativePath, string classLibPath, bool AutoDownload)
        {
            List<string> path = new List<string>();
            List<string> urls = new List<string>();
            if (platform == MyPlatforms.Windows)
            {
                for (int i = 0; i < root.Libraries.Array.Count; i++)
                {
                    if (root.Libraries.Array[i].Downloads.Classifiers != null)
                    {
                        if (root.Libraries.Array[i].Downloads.Classifiers.Natives_windows != null)
                        {
                            path.Add(Path.Combine(classLibPath, root.Libraries.Array[i].Downloads.Classifiers.Natives_windows.Path.ToString().Replace("/", "\\")));
                            urls.Add(root.Libraries.Array[i].Downloads.Classifiers.Natives_windows.Url);
                        }
                    }
                }
                for (int i = 0; i < path.Count; i++)
                {
                    if (File.Exists(path[i]) == true)
                    {
                        ZipFile.ExtractToDirectory(path[i], nativePath, true);
                    }
                    else if (File.Exists(path[i]) == false && AutoDownload == true)
                    {
                        MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                        multiThreadDownloader.GoGoGo(urls[i], 64, path[i].Replace(Path.GetFileName(path[i]), ""));
                        ZipFile.ExtractToDirectory(path[i], nativePath, true);
                    }
                }
            }
            else if (platform == MyPlatforms.Osx)
            {
                // TODO OSX
                throw new NotImplementedException("OSX 版本暂时无具体实现");
            }
            else if (platform == MyPlatforms.Linux)
            {
                // TODO Lunix
                throw new NotImplementedException("Linux 版本暂时无具体实现");
            }


        }
        void AuthAssets(string AssetsDir, bool AutoDownload)
        {
            if (AutoDownload == true)
            {
                var request = WebRequest.Create(root.AssetIndex.Url) as HttpWebRequest; //设置参数
                WebResponse Response = request.GetResponse();
                Stream stream = Response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string aa = reader.ReadToEnd();
                var bb = JsonConvert.DeserializeObject<ObjectTemplates.AssetIndexRoot>(aa);
                for (int i = 0; i < bb.AssetObject.SizeHashPairList.Count; i++)
                {
                    var hash = bb.AssetObject.SizeHashPairList[i].Hash;
                    var url = Path.Combine("http://resources.download.minecraft.net", hash.Substring(0, 2).Replace("\\", "/"), hash.Replace("\\", "/"));
                    var path = Path.Combine(AssetsDir, "objects", hash.Substring(0, 2), hash);
                    if (File.Exists(path) == false)
                    {
                        MultiThreadDownloader md = new MultiThreadDownloader();
                        md.GoGoGo(url, 64, Path.GetDirectoryName(path));
                    }
                    else if (File.Exists(path) == true)
                    {
                        if (new FileInfo(path).Length != bb.AssetObject.SizeHashPairList[i].Size)
                        {
                            MultiThreadDownloader md = new MultiThreadDownloader();
                            md.GoGoGo(url, 64, Path.GetDirectoryName(path));
                        }
                    }
                }
                stream.Close();
            }
        }
        string AutoRuledJVMArguments(ObjectTemplates.Root root, string action, MyPlatforms Platform, string arch)
        {
            var ELJvmArgumets = string.Empty;
            //拼接有规定的参数
            for (int i = 0; i < root.Arguments.Jvm.RuleValuePairs.Count; i++)
            {
                if (action == root.Arguments.Jvm.RuleValuePairs[i].Rules.ActionFeaturesOSGroup.Action.ToString())
                {
                    //当你是 Windows 时
                    if (Platform == MyPlatforms.Windows && root.Arguments.Jvm.RuleValuePairs[i].Rules.ActionFeaturesOSGroup.Os.Name == "windows" && root.Arguments.Jvm.RuleValuePairs[i].Rules.ActionFeaturesOSGroup.Os.Version == null)
                    {
                        for (int j = 0; j < root.Arguments.Jvm.RuleValuePairs[i].Value.Array.Count; j++)
                        {
                            // ELJvmArgumets = ELJvmArgumets + " " + root.Arguments.Jvm.RuleValuePairs[i].Value.Array[j].ToString();
                            ELJvmArgumets = $"{ELJvmArgumets} {root.Arguments.Jvm.RuleValuePairs[i].Value.Array[j]}";
                        }
                    }
                    //当你是 osx 时
                    else if (Platform == MyPlatforms.Osx)
                    {
                        // TODO OSX
                        throw new NotImplementedException("OSX 版本暂时无具体实现");
                    }
                    //当你是 Linux 时
                    else if (Platform == MyPlatforms.Linux)
                    {
                        // TODO Lunix
                        throw new NotImplementedException("Linux 版本暂时无具体实现");
                    }
                    if (arch == root.Arguments.Jvm.RuleValuePairs[i].Rules.ActionFeaturesOSGroup.Os.Arch)
                    {
                        for (int j = 0; j < root.Arguments.Jvm.RuleValuePairs[i].Value.Array.Count; j++)
                        {
                            // ELJvmArgumets = ELJvmArgumets + " " + root.Arguments.Jvm.RuleValuePairs[i].Value.Array[j].ToString();
                            ELJvmArgumets = $"{ELJvmArgumets} {root.Arguments.Jvm.RuleValuePairs[i].Value.Array[j]}";
                        }
                    }
                }
            }
            //拼接普通参数
            for (int i = 0; i < root.Arguments.Jvm.Array.Count; i++)
            {
                ELJvmArgumets = ELJvmArgumets + " " + root.Arguments.Jvm.Array[i].ToString();
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
        public void Launch(bool AutoDownload, MyPlatforms Platform, string Action, string Arch, int MinMemory, int MaxMemory, string OtherArguments)
        {
            FinnalCommand = @"""" + javaPath + @"""";
            FinnalCommand += " " + OtherArguments;
            FinnalCommand += AutoSystemVersion();
            //拼接 classpath 函数
            JVMArgs.Classpath = AutoCpCommandLine(root, classLibPath, AutoDownload, clientJarPath, Platform);
            ExportNative(root, Platform, nativeLibPath, classLibPath, AutoDownload);
            AuthAssets(GameArgs.Assets_root, AutoDownload);
            //版本大于等于 1.13 时
            if (root.MinecraftArguments == null && root.Arguments != null)
            {
                if (javaMajorVersion != root.JavaVersion.MajorVersion)
                {
                    throw new Exception(
                        $"当前 Java 版本不符合 Minecraft 所需的启动环境。{Environment.NewLine}" +
                        $"您的版本为{javaMajorVersion}, 要求版本为{root.JavaVersion.MajorVersion}");
                }
                FinnalCommand += AutoRuledJVMArguments(root, Action, Platform, Arch);
                FinnalCommand += " -Xmn" + MinMemory.ToString() + "M" + " -Xmx" + MaxMemory.ToString() + "M";
                FinnalCommand += " " + root.Logging.Client.Argument.ToString();
                FinnalCommand += " " + root.MainClass.ToString();
                FinnalCommand += " " + ParseMinecraftArguments("false", false, false, 0, 0);
            }
            else if (root.Arguments != null && root.MinecraftArguments == null)
            {

            }
            else if (root.Arguments == null && root.MinecraftArguments == null)
            {
                throw new ArgumentNullException("json 文件内容为空。");
            }
            var ELList = new List<string>
            {
                "${natives_directory}",
                "${launcher_name}",
                "${launcher_version}",
                "${classpath}",

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
            var ELListToConv = new List<string>
            {
                nativeLibPath,
                JVMArgs.Launcher_name,
                JVMArgs.Launcher_version.ToString(),
                JVMArgs.Classpath,

                GameArgs.Auth_player_name,
                GameArgs.Version_name,
                GameArgs.Game_directory,
                GameArgs.Assets_root,
                GameArgs.Assets_index_name,
                GameArgs.Auth_uuid,
                GameArgs.Auth_access_token,
                GameArgs.ClientId,
                GameArgs.Auth_xuid,
                GameArgs.User_type,
                GameArgs.Version_type
            };
            for (int i = 0; i < ELList.Count; i++)
            {
                FinnalCommand = FinnalCommand.Replace(ELList[i], ELListToConv[i]);
            }
            Console.WriteLine(FinnalCommand);
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(FinnalCommand);
        }
    }


}
