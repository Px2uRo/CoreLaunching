using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace CoreLaunching
{
    /// <summary>
    /// 这是一个启动器类，里面提供从 JSON 读取 Minecraft CP 参数
    /// 为确保能正确启动，你得设置 LauncherInfo GameInfo 
    /// 和 JVMInfo 变量
    /// </summary>
    public class Launcher
    {
        #region Methos
        /// <summary>
        /// 启动器信息变量
        /// </summary>
        public LauncherInfo LauncherInfo { get; set; }
        /// <summary>
        /// 游戏信息变量
        /// </summary>
        public GameInfo GameInfo { get; set; }
        /// <summary>
        /// Java 虚拟机变量
        /// </summary>
        public JVMInfo JVMInfo { get; set; }
        /// <summary>
        /// 解析Json文件的路径
        /// </summary>
        public string JsonPath { get; set; }
        /// <summary>
        /// (.minecraft/)Libraries 文件夹的路径
        /// </summary>
        public string LibrariesFolderPath { get; set; }
        /// <summary>
        /// 游戏 (.minecraft/versions/1.7.10.)jar 的路径
        /// </summary>
        public string GameJarPath { get; set; }
        #endregion
        #region Commands
        /// <summary>
        /// 这个是验证值为不为空的
        /// </summary>
        /// <exception cref="ArgumentNullException">为空抛出值为空异常</exception>
        void VerifyNullException()
        {
            if (JVMInfo == null)
            {
                throw new ArgumentNullException(nameof(JVMInfo));
            }
            else if (GameInfo == null)
            {
                throw new ArgumentNullException(nameof(GameInfo));
            }
            else if (LauncherInfo == null)
            {
                throw new ArgumentNullException(nameof(LauncherInfo));
            }
            else if (LibrariesFolderPath == null)
            {
                throw new ArgumentNullException(nameof(LibrariesFolderPath));
            }
            else if(GameJarPath == null)
            {
                throw new ArgumentNullException(nameof(GameJarPath));
            }
        }
        /// <summary>
        /// 构建 Process
        /// </summary>
        /// <returns>返回一个Process 可以自行启动或者
        /// 直接用这个库启动</returns>
        public Process BuildProcess()
        {
            VerifyNullException();
            ProcessStartInfo i = new ProcessStartInfo();
            i.FileName = JVMInfo.JavaPath;
            i.Arguments += this.JVMInfo.FinnalCommand;
            i.Arguments += this.GameInfo.FinalCommand;
            var p = new Process();
            p.StartInfo = i;
            return p;
        }
        /// <summary>
        /// 启动游戏
        /// </summary>
        public void LaunchMinecraft()
        {
            BuildProcess()
                .Start();
        }

        public JsonTemplates.Root ParseRoot()
        {
            return JsonConvert.DeserializeObject
<JsonTemplates.Root>
(FileToJObject.LoadJson(JsonPath)
.ToString());
        }

        public string GetClassPaths()
        {
            VerifyNullException();
            string ret = string.Empty;
            foreach (var item in ParseRoot().Libraries)
            {
                if (item.Downloads.Artifact != null)
                {
                    ret += Path.Combine(LibrariesFolderPath, item.Downloads.Artifact.Path.Replace("/", "\\"));
                    ret += ";";
                }
            }
            ret += GameJarPath;
            return ret;
        }
        public string GetClassPaths(JsonTemplates.Root ParsedRoot)
        {
            VerifyNullException();
            string ret = string.Empty;
            foreach (var item in ParsedRoot.Libraries)
            {
                if (item.Downloads.Artifact != null)
                {
                    ret += Path.Combine(LibrariesFolderPath, item.Downloads.Artifact.Path.Replace("/", "\\"));
                    ret += ";";
                }
            }
            ret += GameJarPath;
            return ret;
        }

        public void AutoParseJson()
        {
            var parsed = ParseRoot();
            this.GameInfo.MainClass = parsed.MainClass;
            this.JVMInfo.ClassPath = GetClassPaths(parsed);
            parsed = null;
        }
        #endregion
        #region Static Commands
        public static string GetMainClassString(string JsonPath)
        {
            return FileToJObject.LoadJson(JsonPath)["MainClass"].ToString();
        }
        public static string GetClassPaths(JsonTemplates.Root ParsedRoot,string LibrariesFolderPath,string GameJarPath)
        {
            string ret = string.Empty;
            foreach (var item in ParsedRoot.Libraries)
            {
                if (item.Downloads.Artifact != null)
                {
                    ret += Path.Combine(LibrariesFolderPath, item.Downloads.Artifact.Path.Replace("/", "\\"));
                    ret += ";";
                }
            }
            return ret;
        }

        #endregion
    }
    public class LauncherInfo
    {
        public Version Version { get; set; }
        public string Name { get; set; }
        public bool FollowsRules { get; set; }
    }
    /// <summary>
    /// 这个类的名字叫做游戏参数，什么意思呢？这个类大多是存储信息用的。
    /// </summary>
    public class GameInfo
    {
        /// <summary>
        /// 如果你想要随时查看当前替换的是什么的话，我提供这个公开变量，但是是只读的，你要格外添加的话，还是设置 OtherArgs 就好
        /// </summary>
        public Dictionary<string,string> ApiFormatMap { get; private set; }
        /// <summary>
        /// 这个就是刷新公开变量的办法
        /// </summary>
        /// <returns>意义不明返回</returns>
        public Dictionary<string, string> RefreshApiFormatMap()
        {
            ApiFormatMap = new Dictionary<string, string>()
            {
                {"${auth_player_name}",this.UserName},
                {"${version_name}",this.GameVersion},
                {"${game_directory}",this.GameDir},
                {"${assets_root}",this.AssetsDir},
                {"${assets_index_name}",this.AssetIndex },
                {"${auth_uuid}",this.Uuid},
                {"${auth_access_token}",this.AccessToken },
                {"${clientid}",this.ClientID },
                {"${auth_xuid}",this.Xuid},
                {"${user_type}",this.UserType},
                {"${version_type}",this.VersionType},
                {"${userProperties}",this.UserProperties}
            };
            return ApiFormatMap;
        }
        public string MainClass { get; set; }
        #region 俗称Minecraft Arguments
        public string UserName { get; set; }
        public string GameVersion { get; set; }
        public string GameDir { get; set; }
        public string AssetsDir { get; set; }
        public string AssetIndex { get; set; }
        public string Uuid { get; set; }
        public string AccessToken { get; set; }
        public string UserProperties { set; get; }
        public string UserType { get; set; }
        public string ClientID { get; set; }
        public string Xuid { get; set; }
        public string VersionType { get; set; }
        #endregion
        /// <summary>
        /// 公开的，可以设置其他要替换的游戏参数。
        /// </summary>
        public Dictionary<string, string> OtherArguments { get; set; }
        /// <summary>
        /// 格式化游戏参数
        /// </summary>
        /// <param name="Input">导入的内容</param>
        /// <returns>返回的还是这个Input</returns>
        public string FormatGameArgs(string Input)
        {
            ApiFormatMap = RefreshApiFormatMap();
            foreach (var item in ApiFormatMap)
            {
                Input = Input.Replace(item.Key, item.Value);
            }
            if(OtherArguments != null)
            {
                foreach (var item in OtherArguments)
                {
                    Input = Input.Replace(item.Key, item.Value);
                }
            }
            return Input;
        }
        /// <summary>
        /// 这是静态的，你可以直接通过一个自己的 Dictionary<string,string> 来替换参数
        /// </summary>
        /// <param name="Input">输入指令</param>
        /// <param name="Map">是一个Dictionary<string,string></param>
        /// <returns>返回就是整理好的启动游戏指令</returns>
        public static string FormatGameArgsFromMap(string Input,Dictionary<string,string> Map)
        {
            foreach (var item in Map)
            {
                Input = Input.Replace(item.Key, item.Value);
            }
            return Input;
        }
        /// <summary>
        /// 这是给 Launcher 这个 class 看的，Launcher 这个 class 会通过这部分启动游戏
        /// </summary>
        public string FinalCommand { get; set; }
        public GameInfo()
        {
            RefreshApiFormatMap();
        }
    }
    public class JVMInfo
    {
        public string OtherArgs { get; set; }
        public string JavaPath { get; set; }
        public string OSNameArg { get; set; }
        public int MaxMem { get; set; }
        public int MinMem { get; set; }
        public string NativesDirectory { get; set; }
        public string ClassPath { get; set; }
        public string FinnalCommand { get; set; }
        public string FormatCommand(string Input,LauncherInfo LauncherInfo)
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"${natives_directory}", this.NativesDirectory},
                {"${launcher_name}", LauncherInfo.Name},
                {"${launcher_version}", $"{LauncherInfo.Version.Major}.{LauncherInfo.Version.Minor}"},
                {"${classpath}", this.ClassPath}
            };
            foreach (var item in map)
            {
                Input = Input.Replace(item.Key,item.Value);
            }
            map = null;
            return Input;
        }
        
        public JVMInfo()
        {
            string Name()
            {
                var sys = Environment.OSVersion;
                switch (sys.Platform)
                {
                    case PlatformID.Win32NT:
                        if (sys.Version.Major == 10)
                        {
                            return "Windows 10";
                        }
                        else if (sys.Version.Major == 6)
                        {
                            if (sys.Version.Minor == 0)
                            {
                                return "Windows Vista";
                            }
                        }
                        break;
                    case PlatformID.Unix:
                        //Todo
                        break;
                }
                return null;
            }
            this.OSNameArg = $"\"-Dos.name={Name()}\" -Dos.version={System.Environment.OSVersion.Version.Major}.{System.Environment.OSVersion.Version.Minor}";
        }
    }
}