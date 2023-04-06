using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using File = System.IO.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CoreLaunching.PinKcatDownloader
{
    public class Parser
    {
        private string _versionInfo = "https://download.mcbbs.net/mc/game/version_manifest.json";

        public string VersionInfo
        {
            get { return _versionInfo; }
            set {
                if (value.EndsWith("/")){
                    value = value.Substring(0, value.Length - 1);
                }
                _versionInfo = value
                    ; }
        }
        private string _assetsSource = "https://download.mcbbs.net/assets";

        public string AssetsSource
        {
            get { return _assetsSource; }
            set {
                if (value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                _assetsSource = value;
            }
        }

        private string _librarySource = "https://download.mcbbs.net/maven/";

        public string LibrarySource
        {
            get { return _librarySource; }
            set
            {
                if (!value.EndsWith("/"))
                {
                    value += "/";
                }
                _librarySource = value; 
            }
        }


        public MCFileInfo[] ParseFromJson(string contentOrPath,ParseType type,string dotMCFolder,string customVersionName,bool removeLocal)
        {
            List<MCFileInfo> res = new();
            Root root= null;
            if(type==ParseType.Json)
            {
                root=JsonConvert.DeserializeObject<Root>(contentOrPath);
            }
            else if(type==ParseType.FilePath)
            {
                root = JsonConvert.DeserializeObject<Root>(File.ReadAllText(contentOrPath));
            }
            else if (type==ParseType.NativeUrl)
            {
                using (var clt = new WebClient())
                {
                    root = JsonConvert.DeserializeObject<Root>(clt.DownloadString(contentOrPath));
                }
            }
            if (root == null)
            {
                throw new ArgumentException("无法解析。");
            }
            else
            {
                #region Client
                var cltinf = root.Downloads.Client;
                var clturl = string.Empty;
                if(root.InheritsFrom != null) 
                {
                    clturl = $"https://download.mcbbs.net/version/{root.InheritsFrom}/client";
                }
                else
                {
                    clturl = $"https://download.mcbbs.net/version/{root.Id}/client";
                }
                #region Redirect
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(clturl);
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;
                using (WebResponse response = req.GetResponse())
                {
                    clturl = "https://download.mcbbs.net" + response.Headers["Location"];
                }
                #endregion
                var cltlocal = Path.Combine(dotMCFolder, "versions", customVersionName, $"{customVersionName}.jar");
                if (removeLocal)
                {
                    if (!File.Exists(cltlocal))
                    {
                        res.Add(new("client", cltinf.Sha1, cltinf.Size, clturl, cltlocal));
                    }
                }
                else{
                    res.Add(new("client", cltinf.Sha1, cltinf.Size, clturl, cltlocal));
                }
                #endregion
                using (var clt = new WebClient())
                {
                    var strin = clt.DownloadString(root.AssetIndex.Url);
                    var indLocal = Path.Combine(dotMCFolder, "assets\\indexes",Path.GetFileName(root.AssetIndex.Url));
                    Directory.CreateDirectory(Path.GetDirectoryName(indLocal));
                    File.Create(indLocal).Close();
                    File.WriteAllText(indLocal,strin);
                    var Assets = JsonConvert.DeserializeObject<AssetsObject>(strin);
                    foreach (var item in Assets.Objects)
                    {
                        var addOne = new MCFileInfo(item, AssetsSource, dotMCFolder);
                        if (removeLocal)
                        {
                            if (!File.Exists(addOne.Local))
                            {
                                res.Add(addOne);
                            }
                        }
                        else
                        {
                            res.Add(addOne);
                        }
                    }
                }
                foreach (var item in root.Libraries)
                {
                    if (item.Downloads.Artifact != null)
                    {
                        var native = item.Downloads.Artifact.Url.Replace("https://libraries.minecraft.net/",LibrarySource);
                        native = item.Downloads.Artifact.Url.Replace("https://maven.minecraftforge.net/", LibrarySource);
                        var local = Path.Combine(dotMCFolder, "libraries", item.Downloads.Artifact.Path.Replace("/", "\\"));
                        var size = item.Downloads.Artifact.Size;
                        var name = item.Name;
                        var sha1 = item.Downloads.Artifact.Sha1;
                        if (removeLocal)
                        {
                            if (!File.Exists(local))
                            {
                                res.Add(new(name, sha1, size, native, local));
                            }
                        }
                        else
                        {
                            res.Add(new(name, sha1, size, native, local));
                        }
                    }
                    if (item.Downloads.Classifiers != null)
                    {
                        for (int j = 0; j < item.Downloads.Classifiers.Count; j++)
                        {
                            var classifier = item.Downloads.Classifiers[j];
                            var name = item.Name;
                            var classnative = classifier.Item.Url;
                            var classlocal = Path.Combine(dotMCFolder, "libraries", classifier.Item.Path.Replace("/", "\\"));
                            var size = classifier.Item.Size;
                            var sha1 = classifier.Item.Sha1;
                            if (removeLocal)
                            {
                                if (!File.Exists(classlocal))
                                {
                                    res.Add(new(name, sha1, size, classnative, classlocal));
                                }
                            }
                            else
                            {
                                res.Add(new(name, sha1, size, classnative, classlocal));
                            }
                        }
                    }
                }
            }
            return res.ToArray();
        }
    }

    public enum ParseType
    {
        FilePath,
        Json,
        NativeUrl
    }
}
