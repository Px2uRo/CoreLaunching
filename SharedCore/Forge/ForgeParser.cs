using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using File = System.IO.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CoreLaunching.PinKcatDownloader;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace CoreLaunching.Forge
{
    public class ForgeParser
    {
        private string _versionInfo = "https://download.mcbbs.net/mc/game/version_manifest.json";

        public string VersionInfo
        {
            get { return _versionInfo; }
            set
            {
                if (value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                _versionInfo = value
                    ;
            }
        }
        private string _assetsSource = "https://download.mcbbs.net/assets";

        public string AssetsSource
        {
            get { return _assetsSource; }
            set
            {
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
        public static string GetVersionContentFromInstaller(string path, bool isUrl)
        {
            if (isUrl)
            {
                #region Redirect
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(path);
                req.Method = "GET";
                using (WebResponse response = req.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var data = ZipFile.GetSubFileData("version.json", stream);
                        return Encoding.UTF8.GetString(data);
                    }
                }
                #endregion
            }
            else
            {
                return File.ReadAllText(path);
            }
        }
        public static string GetInstallProfileContentFromInstaller(string path, bool isUrl)
        {
            if (isUrl)
            {
                #region Redirect
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(path);
                req.Method = "GET";
                using (WebResponse response = req.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var data = ZipFile.GetSubFileData("install_profile.json", stream);
                        return Encoding.UTF8.GetString(data);
                    }
                }
                #endregion
            }
            else
            {
                return File.ReadAllText(path);
            }
        }
        public MCFileInfo[] ParseFromInstaller(string path, bool isUrl,string dotMCFolder,bool removeLocal = true)
        {
            List<MCFileInfo> lst = new List<MCFileInfo>();

            var root = JsonConvert.DeserializeObject<Root>(GetVersionContentFromInstaller(path,isUrl));
            foreach (var item in root.Libraries)
            {
                if (item.Downloads.Artifact != null)
                {
                    var native = item.Downloads.Artifact.Url.Replace("https://maven.minecraftforge.net/", LibrarySource);
                    var local = Path.Combine(dotMCFolder, "libraries", item.Downloads.Artifact.Path.Replace("/", "\\"));
                    var size = item.Downloads.Artifact.Size;
                    var name = item.Name;
                    var sha1 = item.Downloads.Artifact.Sha1;
                    if (removeLocal)
                    {
                        if (!File.Exists(local))
                        {
                            lst.Add(new(name, sha1, size, native, local));
                        }
                    }
                    else
                    {
                        lst.Add(new(name, sha1, size, native, local));
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
                                lst.Add(new(name, sha1, size, classnative, classlocal));
                            }
                        }
                        else
                        {
                            lst.Add(new(name, sha1, size, classnative, classlocal));
                        }
                    }
                }
            }
            return lst.ToArray();
        }
        public MCFileInfo[] ParseFromVersionJson(string contentOrPath, ParseType type, string dotMCFolder, string customVersionName, bool removeLocal)
        {
            List<MCFileInfo> res = new();
            Root root = null;
            if (type == ParseType.Json)
            {
                root = JsonConvert.DeserializeObject<Root>(contentOrPath);
            }
            else if (type == ParseType.FilePath)
            {
                root = JsonConvert.DeserializeObject<Root>(File.ReadAllText(contentOrPath));
            }
            else if (type == ParseType.NativeUrl)
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
                
            }
            return res.ToArray();
        }

        public static string CombineVersionJson(string mcJson, string forgeJson, ParseType type)
        {
            JObject mcJsonObject;
            JObject forgeJsonObject;
            if (type == ParseType.Json)
            {
                mcJsonObject = JObject.Parse(mcJson);
                forgeJsonObject = JObject.Parse(forgeJson);
            }
            else if (type == ParseType.FilePath)
            {
                mcJsonObject = JObject.Parse(File.ReadAllText(mcJson));
                forgeJsonObject = JObject.Parse(File.ReadAllText(forgeJson));
            }
            else
            {
                using (var clt = new WebClient())
                {
                    mcJsonObject = JObject.Parse(clt.DownloadString(mcJson));
                    forgeJsonObject = JObject.Parse(clt.DownloadString(forgeJson));
                }
            }
            var arr1 = forgeJsonObject["arguments"]["jvm"] as JArray;
            foreach (var item in arr1)
            {
                ((mcJsonObject["arguments"]["jvm"]) as JArray).Add(item);
            }
            var arr2 = forgeJsonObject["arguments"]["game"] as JArray;
            foreach (var item in arr2)
            {
                ((mcJsonObject["arguments"]["game"]) as JArray).Add(item);
            }
            var arr3 = forgeJsonObject["libraries"] as JArray;
            foreach (var item in arr3)
            {
                ((mcJsonObject["libraries"]) as JArray).Add(item);
            }
            mcJsonObject["id"] = forgeJsonObject["id"];
            mcJsonObject["time"] = forgeJsonObject["time"];
            mcJsonObject["releaseTime"] = forgeJsonObject["releaseTime"];
            mcJsonObject["type"] = forgeJsonObject["type"];
            mcJsonObject["mainClass"] = forgeJsonObject["mainClass"];
            mcJsonObject["inheritsFrom"] = forgeJsonObject["inheritsFrom"];
            return mcJsonObject.ToString();
        }
        public static string CombineInstallerProfileJson(string mcJson, string forgeJson, ParseType type)
        {
            JObject mcJsonObject;
            JObject forgeJsonObject;
            if (type == ParseType.Json)
            {
                mcJsonObject = JObject.Parse(mcJson);
                forgeJsonObject = JObject.Parse(forgeJson);
            }
            else if (type == ParseType.FilePath)
            {
                mcJsonObject = JObject.Parse(File.ReadAllText(mcJson));
                forgeJsonObject = JObject.Parse(File.ReadAllText(forgeJson));
            }
            else
            {
                using (var clt = new WebClient())
                {
                    mcJsonObject = JObject.Parse(clt.DownloadString(mcJson));
                    forgeJsonObject = JObject.Parse(clt.DownloadString(forgeJson));
                }
            }
            var arr3 = forgeJsonObject["libraries"] as JArray;
            foreach (var item in arr3)
            {
                ((mcJsonObject["libraries"]) as JArray).Add(item);
            }            
            return mcJsonObject.ToString();
        }
    }
}
