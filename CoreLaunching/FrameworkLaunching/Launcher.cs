using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreLaunching.ObjectTemples;
using System.IO.Compression;
using System.Diagnostics;

namespace CoreLaunching
{
    public class Launcher
    {
        public static string JavaPath { get; set; }//指定 Java 路径
        public static string OtherArguments { get; set; }//指定额外参数
        public static string classLibPath { get; set; }
        public static string clinetJarPath { get; set; }
        public static string HeapDumpPath {get; set; }
        public static string log4jConfigurationFilePath { get; set; }
        public static string nativeLibExportPath { get; set; }

        static string Memory;
        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="MaxMemory">最大内存</param>
        /// <param name="MinMemory">最小内存</param>
        public void SetMemory(int MinMemory, int MaxMemory)
        {
            Memory =" " + "-Xmx" + MaxMemory.ToString() + "m" + " -Xmn" + MinMemory.ToString() + "m";
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

        static string DebugOSNameVersion;
        void SetOSNameVersion(string Name,string Version)
        {
            DebugOSNameVersion = " " + @"""-Dos.name=" + Name + @"""" + " " + "-Dos.version=" + Version;
        }

        #region 自动获取系统版本并传参
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

        string minecraftArguments;
        string mainClass;
        string cpCommandLine = @" -cp ";

        #region 解析 JSON
        /// <summary>
        /// 自动解析 JSON 并处理 -cp 参数，并且解包。
        /// </summary>
        void JieXiJSON()
        {
            StreamReader loader = File.OpenText(TargetJSON);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            mainClass = " " + jsonObject["mainClass"]; //读取 JSON 里面的 mainClass 项目。
            minecraftArguments = " " + jsonObject["minecraftArguments"]; //读取 JSON 里面的 minecraftArguments 项目。
            JToken library = jsonObject["libraries"];
            var libraryStr = library.ToString();
            var libraryStrFormatted = libraryStr.Replace("_", "__");
            libraryStrFormatted = libraryStrFormatted.Replace("-", "_");
            List<libInfo> libInfos = JsonConvert.DeserializeObject<List<libInfo>>(libraryStr);
            List<libInfo> libInfoFormatted = JsonConvert.DeserializeObject<List<libInfo>>(libraryStrFormatted);
            loader.Close();
            //#region ExportDlls
            //for (int i = 0; i < libInfoFormatted.Count; i++)
            //{
            //    if(libInfoFormatted[i].downloads.artifact == null)
            //    {
            //        switch (libInfoFormatted[i].downloads.classifiers.natives_windows != null, System.Environment.Is64BitOperatingSystem == true)
            //        {
            //            case (true, true):
            //                var ReFotmattedLibPath1 = classLibPath+@"\" + libInfoFormatted[i].downloads.classifiers.natives_windows.path.ToString().Replace(@"/", @"\").Replace(@"_", @"-").Replace(@"__", @"_");
            //                try
            //                {
            //                    ZipFile.ExtractToDirectory(ReFotmattedLibPath1, nativeLibExportPath);
            //                }
            //                catch(Exception ex)
            //                {
            //                    Console.WriteLine(ex);
            //                }
            //                break;
            //            case (true, false):
            //                var ReFotmattedLibPath2 = classLibPath + @"\" + libInfoFormatted[i].downloads.classifiers.natives_windows.path.ToString().Replace(@"/", @"\").Replace(@"_", @"-").Replace(@"__", @"_");
            //                try
            //                {
            //                    ZipFile.ExtractToDirectory(ReFotmattedLibPath2, nativeLibExportPath);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine(ex);
            //                }
            //                break;
            //                case(false, true):
            //                var ReFotmattedLibPath3 = classLibPath + @"\" + libInfoFormatted[i].downloads.classifiers.natives_windows_64.path.ToString().Replace(@"/", @"\").Replace(@"_", @"-").Replace(@"__", @"_");
            //                try
            //                {
            //                    ZipFile.ExtractToDirectory(ReFotmattedLibPath3, nativeLibExportPath);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine(ex);
            //                    if (new FileInfo(ReFotmattedLibPath3.Replace("64", "32")).Exists == true)
            //                    {
            //                        try
            //                        {
            //                            ZipFile.ExtractToDirectory(ReFotmattedLibPath3.Replace("64", "32"), nativeLibExportPath);
            //                        }
            //                        catch
            //                        {
            //                            Console.WriteLine(ex);   
            //                        }
            //                    }
            //                }
            //                break;
            //            case (false, false):
            //                var ReFotmattedLibPath4 = classLibPath + @"\" + libInfoFormatted[i].downloads.classifiers.natives_windows_86.path.ToString().Replace(@"/", @"\").Replace(@"_", @"-").Replace(@"__", @"_");
            //                try
            //                {
            //                    ZipFile.ExtractToDirectory(ReFotmattedLibPath4, nativeLibExportPath);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine(ex);
            //                }
            //                break;
            //        }
            //    }
            //}
            //#endregion

            #region SetCpArg
            for (int i = 0; i <libInfos.Count; i++)
            {
                if (libInfoFormatted[i].downloads.artifact != null)
                {
                    cpCommandLine = cpCommandLine + classLibPath + @"\" + libInfos[i].downloads.artifact.path.Replace(@"/", @"\") + ";";
                }
            }
            #endregion
        }
        #endregion
        public void Launch()
        {
            AutoSystemVersion();
            JieXiJSON();
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

            cpCommandLine = cpCommandLine + clinetJarPath;
            clinetJarPath = @" -Dminecraft.client.jar=" + clinetJarPath;
            nativeLibExportPath = @" -Djava.library.path=" + nativeLibExportPath;
            log4jConfigurationFilePath = @" -Dlog4j.configurationFile=" + log4jConfigurationFilePath;
            OtherArguments = " "+OtherArguments;
            HeapDumpPath = @" -XX:HeapDumpPath="+HeapDumpPath;
            var FinalCommand = JavaPath + DebugOSNameVersion + HeapDumpPath + nativeLibExportPath + LauncherInfo + clinetJarPath + cpCommandLine + Memory + OtherArguments + log4jConfigurationFilePath + mainClass + minecraftArguments;
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            p.StandardInput.WriteLine(FinalCommand+@"&exit");
            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            Console.WriteLine(output);
        }
    }
}
