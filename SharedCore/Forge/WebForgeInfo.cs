using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLaunching.Forge
{
    public class WebForgeInfo
    {
        public override string ToString()
        {
            return $"{Version} {Mcversion} {Modified}";
        }
        [JsonProperty("_id")]
        public string ID { get; set; }
        [JsonProperty("__v")]
        public int V { get; set; }
        [JsonProperty("branch")]
        public string Branch { get; set; }
        [JsonProperty("build")]
        public string Build { get; set; }
        [JsonProperty("files")]
        public ForgeFile[] Files { get; set; }
        [JsonProperty("mcversion")]
        public string Mcversion { get; set; }
        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
        [JsonProperty("version")]
        public Version Version { get; set; }
    }

    public class ForgeFile
    {
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        public override string ToString()
        {
            return $"{Format}:{Category}";
        }
    }

}
