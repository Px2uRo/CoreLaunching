using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreLaunching.ObjectTemplates
{
    public class HashObjectCoverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var assetObjects = new AssetObject();
            assetObjects.SizeHashPairList = new List<SizeHashPair>();
            foreach (var item in jObject)
            {
                var hash = item.Value["hash"].ToString();
                var size = item.Value["size"].ToObject<long>();
                assetObjects.SizeHashPairList.Add(new SizeHashPair()
                {
                    Hash = hash,
                    Size = size
                }); 
            }
            return assetObjects;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
        }
    }
    [JsonConverter(typeof(HashObjectCoverter))]
    public class AssetObject
    {
        public List<SizeHashPair> SizeHashPairList { get; set; }
    }
    public class SizeHashPair
    {
        public long Size { get; set; }
        public string Hash { get; set; }
    }
    public class AssetIndexRoot
    {
        [JsonProperty("objects")]
        public AssetObject AssetObject { get; set; }
    }
}
