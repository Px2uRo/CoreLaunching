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
        public static string nativeLibPath { get; set; }

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

        static string OSNameVersion;
        public void SetOSNameVersion(string Name,string Version)
        {
            OSNameVersion = " " + @"""-Dos.name=" + Name + @"""" + " " + "-Dos.version=" + Version;
        }
        public void Launch()
        {
            StreamReader loader = File.OpenText(TargetJSON);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            var mainClass = " " + jsonObject["mainClass"]; //读取 JSON 里面的 mainClass 项目。
            var minecraftArguments = " " + jsonObject["minecraftArguments"]; //读取 JSON 里面的 minecraftArguments 项目。
            JToken library = jsonObject["libraries"];
            var libraryStr = library.ToString();
            List<libInfo> libInfos = JsonConvert.DeserializeObject<List<libInfo>>(libraryStr.Replace("-","_"));

            loader.Close();

            for(int i = 0; i < libInfos.Count; i++)
            {
                if (libInfos[i].natives != null&&libInfos[i].downloads.classifiers.natives_windows!=null)
                {
                    var tmp1 = libInfos[i].downloads.classifiers.natives_windows.path.ToString();
                    tmp1 = tmp1.Replace(@"/", @"\");
                    var tmp2 = classLibPath + @"\" + tmp1;
                    tmp2 = tmp2.Replace("_", "-");
                    FileInfo tmp2Info = new FileInfo(tmp2);
                    try
                    {
                        ZipFile.ExtractToDirectory(tmp2, nativeLibPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    if (libInfos[i].natives != null && libInfos[i].downloads.classifiers.natives_windows_64 != null)
                    {
                        var tmp1 = libInfos[i].downloads.classifiers.natives_windows_64.path.ToString();
                        tmp1 = tmp1.Replace(@"/", @"\");
                        var tmp2 = classLibPath + @"\" + tmp1;
                        tmp2 = tmp2.Replace("_", "-");
                        FileInfo tmp2Info = new FileInfo(tmp2);
                        try
                        {
                            ZipFile.ExtractToDirectory(tmp2, nativeLibPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }

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
            string cpCommandLine = @" -cp ";
            for(int i = 0; i < libInfos.Count; i++)
            {
                if(libInfos[i].downloads.artifact != null)
                {
                    libInfos[i].downloads.artifact.path = libInfos[i].downloads.artifact.path.Replace(@"/", @"\");
                    libInfos[i].downloads.artifact.path = libInfos[i].downloads.artifact.path.Replace(@"_", @"-");
                    var File = new FileInfo(classLibPath + @"\" + libInfos[i].downloads.artifact.path);
                    if (File.Exists==true)
                    {
                        cpCommandLine = cpCommandLine + classLibPath + @"\" + libInfos[i].downloads.artifact.path + ";";
                    }
                    else
                    {
                        cpCommandLine = cpCommandLine + classLibPath + @"\" + @"org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar" + ";";
                    }
                }
            }
            cpCommandLine = cpCommandLine + clinetJarPath;
            clinetJarPath = @" -Dminecraft.client.jar=" + clinetJarPath;
            nativeLibPath = @" -Djava.library.path=" + nativeLibPath;
            log4jConfigurationFilePath = @" -Dlog4j.configurationFile=" + log4jConfigurationFilePath;
            OtherArguments = " "+OtherArguments;
            HeapDumpPath = @" -XX:HeapDumpPath="+HeapDumpPath;
            #region UselessCommand
            Console.WriteLine(JavaPath);
            Console.WriteLine(OSNameVersion);
            Console.WriteLine(HeapDumpPath);
            Console.WriteLine(nativeLibPath);
            Console.WriteLine(LauncherInfo);
            Console.WriteLine(clinetJarPath);
            Console.WriteLine(cpCommandLine);
            Console.WriteLine(Memory);
            Console.WriteLine(OtherArguments);
            Console.WriteLine(log4jConfigurationFilePath);
            Console.WriteLine(mainClass);
            Console.WriteLine(minecraftArguments);
            #endregion
            var FinalCommand = JavaPath + OSNameVersion + HeapDumpPath + nativeLibPath + LauncherInfo + clinetJarPath + cpCommandLine + Memory + OtherArguments + log4jConfigurationFilePath + mainClass + minecraftArguments;
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
