using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CoreLaunching.Forge
{
    public static class BMCLForgeHelper
    {
        private static string[] _knownSupportedVersions = null;

        public static string[] KnownSupportedVersions { get 
            { 
                if(_knownSupportedVersions == null)
                {
                    _knownSupportedVersions = GetSupporttedVersions();
                }
                return _knownSupportedVersions;
            } }

        public static string[] GetSupporttedVersions()
        {
            using (var clt = new WebClient())
            {
                return JsonConvert.DeserializeObject<string[]>(clt.DownloadString("https://bmclapi2.bangbang93.com/forge/minecraft"));
            }
        }
        private static Dictionary<string, WebForgeInfo[]> _knownUrlsByVersion = new Dictionary<string, WebForgeInfo[]>();
        public static WebForgeInfo[] GetKnownUrlsFromMcVersion(string mcVersion)
        {
            if (!_knownUrlsByVersion.Keys.Contains(mcVersion))
            {
                _knownUrlsByVersion.Add(mcVersion, GetDownloadUrlsFromMcVersion(mcVersion));
            }
            return _knownUrlsByVersion[mcVersion];
        }
        private static WebForgeInfo[] GetDownloadUrlsFromMcVersion(string mcVersion) 
        {
            using (var clt = new WebClient())
            {
                return JsonConvert.DeserializeObject<WebForgeInfo[]>(clt.DownloadString($"https://bmclapi2.bangbang93.com/forge/minecraft/{mcVersion}"));
            }
        }

        public static string GetDownloadUrlFromBuild(string build)
        {
            var url = $"https://bmclapi2.bangbang93.com/forge/download/{build}";
            #region Redirect
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";
            req.AllowAutoRedirect = false;
            using (WebResponse response = req.GetResponse())
            {
                url = "https://download.mcbbs.net" + response.Headers["Location"];
            }
            return url;
            #endregion
        }
        public static string GetDownloadUrlFromParameters(string mcversion,string version,string category,string format,string branch = "")
        {
            if (string.IsNullOrEmpty(branch))
            {
                return $"https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{mcversion}-{version}/forge-{mcversion}-{version}-{category}.{format}";
            }
            else
            {
                return $"https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{mcversion}-{version}-{branch}/forge-{mcversion}-{version}-{category}.{format}";
            }
        }
    }
}
