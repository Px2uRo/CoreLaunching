using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CLDemo.Data
{
    public class VersionInJson
    {
        [JsonProperty("latest")]
        public Latest Latest { get; set; }
        [JsonProperty("versions")]
        public ObservableCollection<Versions> Versions { get; set; }
    }
    public class Latest
    {
        [JsonProperty("release")]
        public string Release { get; set; }
        [JsonProperty("snapshot")]
        public string Snapshot { get; set; }
    }
    public class Versions
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }
    }
}
