using CoreLaunching.JsonTemplates;
using CoreLaunching.PinKcatDownloader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CoreLaunching.Forge
{
    public class ForgeInstaller
    {
        public event EventHandler<string> Output;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jREPath">JAVA 运行时路径（只能本地）</param>
        /// <param name="minecraft_Jar">原版游戏 JAR 路径（只能本地）</param>
        /// <param name="mCJson"></param>
        /// <param name="installerProfiles"></param>
        /// <param name="type">解析类型</param>
        public static void ClientInstall(string jREPath,string workPath, string minecraft_Jar,string mCJson, string installerProfiles,ParseType type)
        {
            InstallProfile insp;
            Root mCr;
            if(type==ParseType.Json)
            {
                insp = JsonConvert.DeserializeObject<InstallProfile>(installerProfiles);
                mCr = JsonConvert.DeserializeObject<Root>(mCJson);
            }
            else if(type == ParseType.FilePath)
            {
                insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(installerProfiles));
                mCr = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(mCJson));
            }
            else
            {
                using (var clt = new WebClient())
                {
                    var inst = clt.DownloadString(installerProfiles);
                    var mCt = clt.DownloadString(mCJson);
                    insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(inst));
                    mCr = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(mCt));
                }
            }
            var datab = insp.Data;
            datab.Add("{SIDE}", new ClientAndServerPair("client", "server"));
            datab.Add("{MINECRAFT_JAR}", new ClientAndServerPair(minecraft_Jar, string.Empty));
            datab["{BINPATCH}"].Client = Path.Combine(workPath, datab["{BINPATCH}"].Client.Remove(0, 1).Replace("/", "\\"));
            datab["{BINPATCH}"].Server = Path.Combine(workPath, datab["{BINPATCH}"].Server.Remove(0, 1).Replace("/", "\\"));
            var procs = insp.Processors.Where((x) => x.IsForClient).ToArray();
            foreach (var item in procs)
            {
                var proc = item.GetProcess(jREPath,workPath,datab);
                proc.StartInfo.RedirectStandardOutput= true;
                proc.Start();
                proc.WaitForExit();
            }
        }

        public void InstallClient(string jREPath, string workPath, string minecraft_Jar, string mCJson, string installerProfiles, ParseType type)
        {
            InstallProfile insp;
            Root mCr;
            if (type == ParseType.Json)
            {
                insp = JsonConvert.DeserializeObject<InstallProfile>(installerProfiles);
                mCr = JsonConvert.DeserializeObject<Root>(mCJson);
            }
            else if (type == ParseType.FilePath)
            {
                insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(installerProfiles));
                mCr = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(mCJson));
            }
            else
            {
                using (var clt = new WebClient())
                {
                    var inst = clt.DownloadString(installerProfiles);
                    var mCt = clt.DownloadString(mCJson);
                    insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(inst));
                    mCr = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(mCt));
                }
            }
            var datab = insp.Data;
            datab.Add("{SIDE}", new ClientAndServerPair("client", "server"));
            datab.Add("{MINECRAFT_JAR}", new ClientAndServerPair(minecraft_Jar, string.Empty));
            datab["{BINPATCH}"].Client = Path.Combine(workPath, datab["{BINPATCH}"].Client.Remove(0, 1).Replace("/", "\\"));
            datab["{BINPATCH}"].Server = Path.Combine(workPath, datab["{BINPATCH}"].Server.Remove(0, 1).Replace("/", "\\"));
            var procs = insp.Processors.Where((x) => x.IsForClient).ToArray();
            foreach (var item in procs)
            {
                var proc = item.GetProcess(jREPath, workPath, datab);
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardOutput=true;
                proc.Start();
                proc.OutputDataReceived += Proc_OutputDataReceived;
                proc.BeginOutputReadLine();
                proc.WaitForExit();
            }
        }

        private void Proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Output?.Invoke(this,e.Data);
        }
    }
}
