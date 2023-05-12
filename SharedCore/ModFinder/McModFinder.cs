using CoreLaunching.JsonTemplates;
using CoreLaunching.PinKcatDownloader;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CoreLaunching.ModFinder
{
    public static class McModFinder
    {
        public static IEnumerable<McModWebResultItem> FromName(string name,string page = "1")
        {
            var ru = $"https://search.mcmod.cn/s?key={name}&site=&filter=1&mold=0&page={page}";
            var req = (HttpWebRequest)(HttpWebRequest.Create(ru));
            req.Timeout = 30000;
            req.KeepAlive = true;
            req.Method = "GET";
            req.Credentials = CredentialCache.DefaultCredentials;
            using (var resp = (System.Net.HttpWebResponse)req.GetResponse())
            {
                using (var restrm = resp.GetResponseStream())
                {
                    
                        var strR = new StreamReader(restrm);
                        var html = strR.ReadToEnd();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var nodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[4]/div[2]");
                        if (nodes==null)
                        {
                            nodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[4]/div[1]");
                        }
                        if (nodes.Count > 0)
                        {
                            return McModWebResultItem.ParseManyFromHtmlNode(nodes[0].ChildNodes.ToArray());
                        }
                        else
                        {
                            return null;
                        }
                    
                }
            }
        }

    }

    public class McModWebResultItem
    {
        public string Page { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime RecordTime { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static List<McModWebResultItem> ParseManyFromHtmlNode(IEnumerable<HtmlNode> nodes)
        {
            var result = new List<McModWebResultItem>();
            foreach (var node in nodes)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var resultItem = new McModWebResultItem();
                resultItem.Name = doc.DocumentNode.SelectNodes("/div[1]")[0].InnerText;
                resultItem.Page = doc.DocumentNode.SelectNodes("/div[1]/a[1]")[0].Attributes["href"].Value;
                resultItem.Description = doc.DocumentNode.SelectNodes("/div[2]")[0].InnerText;
                resultItem.RecordTime = DateTime.Parse(doc.DocumentNode.SelectNodes("/div[3]/span[2]")[0].InnerText.Replace("快照时间：", ""));
                resultItem.Source = doc.DocumentNode.SelectNodes("/div[3]/span[3]")[0].InnerText.Replace("来自：", "");
                result.Add(resultItem);
            }
            return result;
        }
    }
    public class SourceNameUrlPair
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public class McModLinkResultItem
    {
        public Dictionary<string, IEnumerable<string>> SupportedVersions { get; set; }

        public IEnumerable<SourceNameUrlPair> Sources { get; set; }
        public static McModLinkResultItem GetDownloadInfoFromPage(string page)
        {
            var req = (HttpWebRequest)(HttpWebRequest.Create(page));
            req.Timeout = 30000;
            req.KeepAlive = true;
            req.Method = "GET";
            req.Credentials = CredentialCache.DefaultCredentials;
            using (var resp = (System.Net.HttpWebResponse)req.GetResponse())
            {
                using (var restrm = resp.GetResponseStream())
                {

                    var strR = new StreamReader(restrm);
                    var html = strR.ReadToEnd();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var sourcesNodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[1]/div[2]/div[2]/div[1]/div[2]/div[2]/div[1]/ul[1]/div[1]/div[2]/ul[1]");
                    var versionNodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[1]/div[2]/div[2]/div[1]/div[2]/div[2]/div[1]/ul[1]/li[9]/ul[1]");
                    var result = new McModLinkResultItem();
                    var supported = new Dictionary<string, IEnumerable<string>>();
                    var snpl = new List<SourceNameUrlPair>();
                    foreach (var source in sourcesNodes[0].ChildNodes)
                    {
                        var snp = new SourceNameUrlPair();
                        snp.Name = source.InnerText;
                        snp.Url = source.ChildNodes[0].Attributes["href"].Value.Replace("//","https://");
                        snpl.Add(snp);
                    }
                    result.Sources= snpl;
                    foreach (var loader in versionNodes[0].ChildNodes)
                    {
                        for (int i = 0; i < loader.ChildNodes.Count; i++)
                        {
                            var version = loader.ChildNodes[0];
                            if (version.InnerText.Contains("Forge"))
                            {
                                supported.Add("Forge", new List<string>());
                                for (int j = 1; j < loader.ChildNodes.Count; j++)
                                {
                                    (supported["Forge"] as List<string>).Add(loader.ChildNodes[j].InnerText);
                                }
                                break;
                            }
                            else if (version.InnerText.Contains("Fabric"))
                            {
                                supported.Add("Fabric", new List<string>());
                                for (int j = 1; j < loader.ChildNodes.Count; j++)
                                {
                                    (supported["Fabric"] as List<string>).Add(loader.ChildNodes[j].InnerText);
                                }
                                break;
                            }
                        }
                    }
                    result.SupportedVersions = supported;
                    return result;
                }
            }
        }
    }
}
