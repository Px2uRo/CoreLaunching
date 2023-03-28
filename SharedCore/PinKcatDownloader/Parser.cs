using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using File = System.IO.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

namespace CoreLaunching.PinKcatDownloader
{
    public class Parser
    {
        public const string VersionInfo = "https://download.mcbbs.net/mc/game/version_manifest.json";
        public const string AssetsSource = "https://download.mcbbs.net/assets";
        public const string librarySource = "https://download.mcbbs.net/maven/";

        public static MCFileInfo[] ParseFromJson(string contentOrPath,ParseType type,string dotMCFolder,string customVersionName,bool removeLocal)
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
                var clturl = $"https://download.mcbbs.net/version/{root.Id}/client";
                #region Redirect
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(clturl);
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;
                using (WebResponse response = req.GetResponse())
                {
                    clturl = "https://download.mcbbs.net" + response.Headers["Location"];
                }
                #endregion
                res.Add(new("client", cltinf.Sha1, cltinf.Size, clturl, Path.Combine(dotMCFolder, "versions",customVersionName, $"{customVersionName}.jar")));
                #endregion
                using (var clt = new WebClient())
                {
                    var Assets = JsonConvert.DeserializeObject<AssetsObject>(clt.DownloadString(root.AssetIndex.Url));
                    foreach (var item in Assets.Objects)
                    {
                        res.Add(new(item,AssetsSource,dotMCFolder));
                    }
                }
                foreach (var item in root.Libraries)
                {
                    if (item.Downloads.Artifact != null)
                    {
                        var native = item.Downloads.Artifact.Url.Replace("https://libraries.minecraft.net/",librarySource);
                        var local = Path.Combine(dotMCFolder, "libraries", item.Downloads.Artifact.Path.Replace("/", "\\"));
                        var size = item.Downloads.Artifact.Size;
                        var name = item.Name;
                        var sha1 = item.Downloads.Artifact.Sha1;
                        res.Add(new(name,sha1,size,native,local));
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
                            res.Add(new(name, sha1, size, classnative, classlocal)); ;
                        }
                    }
                }
            }
            if (removeLocal)
            {
                var Lq = res.Where(x => File.Exists(x.Local)).ToArray();
                foreach (var item in Lq)
                {
                    res.Remove(item);
                }
            }
            return res.ToArray();
        }

        public enum ParseType
        {
            FilePath,
            Json,
            NativeUrl
        }
    }
}
