using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class MCFileInfo
    {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("sha1")]
            public string Sha1 { get; set; }

            [JsonProperty("size")]
            public long Size { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonIgnore]
            public string Local { get; set; }

        public override string ToString()
        {
            return $"{Id},{Size},{Url},{Local}";
        }
        public MCFileInfo(string id, string sha1, long size, string url, string loacl)
        {
            Id = id;
            Sha1 = sha1;
            Size = size;
            Url = url;
            Local = loacl;
        }
        public MCFileInfo(NameHashSize item,string AssetsSource,string dotMCFolder) :this(item.Name, item.Hash, item.Size, $"{AssetsSource}/{item.Hash.Substring(0, 2)}/{item.Hash}", Path.Combine(dotMCFolder, "assets", "objects", item.Hash.Substring(0, 2), item.Hash)) 
        { 
            
        }
    }
}
