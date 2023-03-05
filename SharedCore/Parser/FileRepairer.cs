using CoreLaunching.Down.Web;
using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CoreLaunching.Parser
{
    public class FileRepairer
    {
        public static NativeLoaclPair[] GetDownloadURIs(string JsonPath,string dotMCPath,string VersionPath)
        {
            using (var webc = new WebClient())
            {
                #region jar
                var lst = new List<NativeLoaclPair>();
                var content = System.IO.File.ReadAllText(JsonPath);
                var root = JsonConvert.DeserializeObject<Root>(content);
                var cltPth = Path.Combine(VersionPath,Path.GetFileName(VersionPath)+".jar");
                if(!System.IO.File.Exists(cltPth))
                {
                    lst.Add(new(root.Downloads.Client.Url, cltPth, root.Downloads.Client.Size));
                }
                #endregion
                #region Assets
                var assetIndexLocal = Path.Combine(dotMCPath, $"assets\\indexes\\{root.AssetIndex.Id}.json");
                AssetsObject assetRoot = null;
                if (!System.IO.File.Exists(assetIndexLocal))
                {
                    var assetIndexNative = root.AssetIndex.Url;
                    var assetWebString = webc.DownloadString(assetIndexNative);
                    assetRoot = JsonConvert.DeserializeObject<AssetsObject>(assetWebString);
                    Directory.CreateDirectory(Path.GetDirectoryName(assetIndexLocal));
                    System.IO.File.WriteAllText(assetIndexLocal, assetWebString);
                }
                else
                {
                    assetRoot = JsonConvert.DeserializeObject<AssetsObject>(System.IO.File.ReadAllText(assetIndexLocal));
                }
                Directory.CreateDirectory(Path.Combine(dotMCPath, $"assets\\objects\\"));
                foreach (var item in assetRoot.Objects)
                {
                    var localp = Path.Combine(dotMCPath, $"assets\\objects\\{item.Hash[..2]}\\{item.Hash}");
                    if (!System.IO.File.Exists(localp))
                    {
                        var native = $"http://resources.download.minecraft.net/{item.Hash[..2]}/{item.Hash}";
                        lst.Add(new(native, localp, item.Size));
                    }
                }
                #endregion
                #region Lib
                Path.Combine(dotMCPath, "libraries");
                foreach (var item in root.Libraries)
                {
                    var native = string.Empty;
                    var local = string.Empty;
                    long size = 0;
                    if (item.Downloads.Artifact != null)
                    {
                        native = item.Downloads.Artifact.Url;
                        local = Path.Combine(dotMCPath, "libraries", item.Downloads.Artifact.Path.Replace("/", "\\"));
                        size = item.Downloads.Artifact.Size;
                    }
                    if (item.Downloads.Classifiers != null)
                    {
                        for (int j = 0; j < item.Downloads.Classifiers.Count; j++)
                        {
                            var classifier = item.Downloads.Classifiers[j];
                            var classnative = classifier.Item.Url;
                            var classlocal = Path.Combine(dotMCPath, "libraries", classifier.Item.Path.Replace("/", "\\"));
                            if (!System.IO.File.Exists(classlocal))
                            {
                                lst.Add(new(classnative, classlocal, classifier.Item.Size));
                            }
                        }
                    }
                    if (!(string.IsNullOrEmpty(native) && string.IsNullOrEmpty(local)))
                    {
                        if (!System.IO.File.Exists(local))
                        {
                            lst.Add(new(native, local, size));
                        }
                    }
                }
                #endregion 
                return lst.ToArray();

            }
        }
    }

    public class NativeLoaclPair 
    {

        public string Native { get; set; }
        public string Loacl { get; set; }
        public long Length { get; set;}
        public NativeLoaclPair(string native, string loacl, long length)
        {
            Native = native;
            Loacl = loacl;
            Length = length;
        }
    }
}
