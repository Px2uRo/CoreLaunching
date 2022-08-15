using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreLaunching.JsonTemplates
{
    public class AssetsObject
    {
        [JsonConverter(typeof(NameHashSizeConvert))]
        [JsonProperty("objects")]
        public List<NameHashSize> Objects;
    }

    public class NameHashSizeConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            List<NameHashSize> list = new List<NameHashSize>();
            foreach (var item in (JObject)(JToken.ReadFrom(reader)))
            {
                NameHashSize nhs = new NameHashSize();
                nhs.Name = item.Key;
                nhs.Hash = (string)item.Value["hash"];
                nhs.Size = (int)item.Value["size"];
                list.Add(nhs);
            }
            return list;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {

        }
    }

    public class NameHashSize:Object
    {
        public string Name;
        public string Hash;
        public int Size;
        public override string ToString()
        {
            return Name;
        }
    }
}
