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
using System.Net;

namespace CoreLaunching
{
    /// <summary>
    /// CoreLaunching 启动部分
    /// </summary>
    public class Launcher
    {
        #region Launcher 类里面可以复用的变量
        ObjectTemplates.Root root;
        GameArgsInfo GameArgs;
        JVMArgsInfo JVMArgs;
        string clientJarPath;
        string FinnalCommand;
        string javaPath;
        string classLibPath;
        string nativeLibPath;
        int javaMajorVersion;
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
        public Launcher(string TargetJSON, string JavaPath,JVMArgsInfo jVMArgs, string ClassLibPath,string ClientJarPath,string NativeLibPath,GameArgsInfo gameArgs)
        {
            root = JsonConvert.DeserializeObject<ObjectTemplates.Root>(LoadJson(TargetJSON).ToString());
            GameArgs = gameArgs;
            JVMArgs = jVMArgs;
            classLibPath = ClassLibPath;
            clientJarPath = ClientJarPath;
            nativeLibPath = NativeLibPath;
            javaPath = JavaPath;
            javaMajorVersion=FileVersionInfo.GetVersionInfo(JavaPath).FileMajorPart;
        }
        #endregion
        #region 自动获取系统版本并传参
        String SetOSNameVersion(string Name, string Version)
        {
            return " -Dos.name=" + Name + " -Dos.version=" + Version;
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
        String AutoCpCommandLine(ObjectTemplates.Root root,string classLibPath,bool AutoDownload,string ClientJarPath,MyPlatforms Platform)
        {
            var ELcpCommandLine = "";
            if (Platform == MyPlatforms.Windows)
            {
                for (int i = 0; i < root.libraries.Array.Count; i++)
                {
                    var aa = Path.Combine(classLibPath, root.libraries.Array[i].downloads.artifact.path.Replace("/", @"\"));
                    if (root.libraries.Array[i].downloads.classifiers == null && root.libraries.Array[i].rules == null)
                    {
                        ELcpCommandLine += aa + ";";
                        if (File.Exists(aa) == false && AutoDownload == true)
                        {
                            MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                            multiThreadDownloader.GoGoGo(root.libraries.Array[i].downloads.artifact.url.ToString(), 64, aa.Replace(Path.GetFileName(aa), ""));
                        }
                    }
                    else if (root.libraries.Array[i].downloads.artifact != null && root.libraries.Array[i].downloads.classifiers == null && root.libraries.Array[i].rules.Count == 2)
                    {
                        ELcpCommandLine += aa + ";";
                        if (File.Exists(aa) == false && AutoDownload == true)
                        {
                            MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                            multiThreadDownloader.GoGoGo(root.libraries.Array[i].downloads.artifact.url.ToString(), 64, aa.Replace(Path.GetFileName(aa), ""));
                        }
                    }
                }
            }
            else if(Platform == MyPlatforms.Osx)
            {

            }
            else if(Platform == MyPlatforms.Linux)
            {

            }
            ELcpCommandLine =@""""+ ELcpCommandLine + ClientJarPath+@"""";
            return ELcpCommandLine;
        }
        public enum MyPlatforms
        {
            Windows,Osx,Linux
        }
        void ExportNative(ObjectTemplates.Root root,MyPlatforms platform,string nativePath,string classLibPath,bool AutoDownload)
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
                    else if (File.Exists(path[i]) == false&&AutoDownload==true)
                    {
                        MultiThreadDownloader multiThreadDownloader = new MultiThreadDownloader();
                        multiThreadDownloader.GoGoGo(urls[i], 64, path[i].Replace(Path.GetFileName(path[i]), ""));
                        ZipFile.ExtractToDirectory(path[i], nativePath, true);
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
        void AuthAssets(string AssetsDir,bool AutoDownload)
        {
            if (AutoDownload==true)
            {
                var request = WebRequest.Create(root.assetIndex.url) as HttpWebRequest; //设置参数
                WebResponse Response = request.GetResponse();
                Stream stream = Response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string aa = reader.ReadToEnd();
                var bb = JsonConvert.DeserializeObject<ObjectTemplates.AssetIndexRoot>(aa);
                for (int i = 0; i < bb.assetObject.SizeHashPairList.Count; i++)
                {
                    var hash = bb.assetObject.SizeHashPairList[i].Hash;
                    var url = Path.Combine("http://resources.download.minecraft.net", hash.Substring(0, 2).Replace("\\","/"), hash.Replace("\\","/"));
                    var path = Path.Combine(AssetsDir, "objects", hash.Substring(0, 2), hash);
                    if (File.Exists(path) == false)
                    {
                        MultiThreadDownloader md = new MultiThreadDownloader();
                        md.GoGoGo(url, 64, Path.GetDirectoryName(path));
                    }
                    else if (File.Exists(path) == true)
                    {
                        if (new FileInfo(path).Length.ToString() != bb.assetObject.SizeHashPairList[i].Size)
                        {
                            MultiThreadDownloader md = new MultiThreadDownloader();
                            md.GoGoGo(url, 64, Path.GetDirectoryName(path));
                        }
                    }
                }
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
            FinnalCommand = @""""+javaPath+@"""";
            FinnalCommand += " " + OtherArguments;
            FinnalCommand += AutoSystemVersion();
            //拼接 classpath 函数
            JVMArgs.classpath = AutoCpCommandLine(root, classLibPath, AutoDownload, clientJarPath,Platform);
            ExportNative(root, Platform, nativeLibPath, classLibPath,AutoDownload);
            AuthAssets(GameArgs.assets_root, AutoDownload);
            //版本大于等于 1.13 时
            if (root.minecraftArguments == null && root.arguments != null)
            {
                if (javaMajorVersion != root.javaVersion.majorVersion)
                {
                    throw new Exception("你的版本不符合 Json 上的要求");
                }
                FinnalCommand += AutoRuledJVMArguments(root,Action,Platform,Arch);
                FinnalCommand += " -Xmn"+MinMemory.ToString()+"M"+ " -Xmx" + MaxMemory.ToString() + "M";
                FinnalCommand += " " + root.logging.client.argument.ToString();
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
        public GameArgsInfo(string Auth_Player_Name, string Game_Directory, string Assets_Root, string Asset_Index_JSON, string Auth_Uuid, string Auth_Access_Token, string ClientId, string Auth_Xuid, string User_Type, string Version_Type,int Resolution_Width,int Resolution_Height)
        {
            StreamReader loader = File.OpenText(Asset_Index_JSON);
            JsonTextReader reader = new JsonTextReader(loader);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            ObjectTemplates.Root root = JsonConvert.DeserializeObject<ObjectTemplates.Root>(jsonObject.ToString());
            loader.Close();
            reader.Close();
            auth_player_name = Auth_Player_Name;
            version_name = root.id;
            game_directory = Game_Directory;
            assets_root = Assets_Root;
            assets_index_name = root.assetIndex.id;
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
        public JVMArgsInfo(string Natives_Directory,string Launcher_Name,Version Launcher_Version)
        {
            natives_directory = Natives_Directory;
            launcher_name = Launcher_Name;
            launcher_version = Launcher_Version;
        }
        public string natives_directory { get; set; }
        public string launcher_name { get; set; }
        public Version launcher_version { get; set; }
        public string classpath { get; set; }
    }
    #endregion
}
