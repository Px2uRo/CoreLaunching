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
        public event EventHandler<long> DownloadedSizeUpdated;
        public void InstallClient(string jREPath, string librariesPath, string minecraft_Jar, string mCJson, string installerJar, ParseType type)
        {
            var workPath = Path.Combine(librariesPath, "[CL]InstallTmp");
            Directory.CreateDirectory(workPath);
            InstallProfile insp;
            Root mCr;
            if (type == ParseType.Json)
            {
                ZipFile.ExportAll(installerJar, workPath);
                insp = JsonConvert.DeserializeObject<InstallProfile>(Path.Combine(workPath, "install_profile.json"));
                mCr = JsonConvert.DeserializeObject<Root>(mCJson);
            }
            else if (type == ParseType.FilePath)
            {
                ZipFile.ExportAll(installerJar, workPath);
                insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(Path.Combine(workPath, "install_profile.json")));
                mCr = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(mCJson));
            }
            else
            {
                using (var clt = new WebClient())
                {
                    clt.DownloadFile(installerJar, Path.Combine(workPath, "[CL]FileTmp.jar"));
                    ZipFile.ExportAll(Path.Combine(workPath, "[CL]FileTmp.jar"), workPath);
                    insp = JsonConvert.DeserializeObject<InstallProfile>(System.IO.File.ReadAllText(Path.Combine(workPath, "install_profile.json")));
                    var mCt = clt.DownloadString(mCJson);
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
                var proc = item.GetProcess(jREPath, librariesPath,workPath, datab);
                proc.StartInfo.WorkingDirectory = workPath;
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
