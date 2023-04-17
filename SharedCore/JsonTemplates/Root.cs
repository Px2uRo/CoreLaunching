using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreLaunching.JsonTemplates
{

    [Serializable]
    public class UnSpportException : Exception
    {
        public UnSpportException() { }
        public UnSpportException(string message) : base(message) { }
        public UnSpportException(string message, Exception inner) : base(message, inner) { }
        protected UnSpportException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class LibrariesArrayItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<LibrariesArrayItem> ret = new List<LibrariesArrayItem>();
            foreach (var item in JToken.ReadFrom(reader))
            {
                LibrariesArrayItem itm = new LibrariesArrayItem();
                var job = (JObject)item;
                var Allow = false;
                if (job["rules"] != null)
                {
                    try
                    {
                        foreach (var rule in job["rules"])
                        {
                            if (rule["action"].ToString() == "allow")
                            {
                                if (rule["os"] == null)
                                {
                                    Allow = true;
                                }
                                else
                                {
                                    if (rule["os"].HasValues)
                                    {
                                        if (rule["os"]["name"] != null)
                                        {
                                            if (rule["os"]["name"].ToString() == "windows" && Environment.OSVersion.Platform == PlatformID.Win32NT)
                                            {
                                                Allow = true;
                                            }
                                            else
                                            {
                                                Allow = false;
                                            }
                                        }
                                        if (rule["os"]["version"] != null)
                                        {
                                            if (rule["os"]["version"].ToString() == "^10\\." && Environment.OSVersion.Version.Major >= 10)
                                            {
                                                Allow = true;
                                            }
                                            else
                                            {
                                                Allow = false;
                                            }
                                        }
                                        if (rule["os"]["arch"] != null)
                                        {
                                            if (rule["os"]["arch"].ToString() == "x86" && Environment.Is64BitOperatingSystem == false)
                                            {
                                                Allow = true;
                                            }
                                            else
                                            {
                                                Allow = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (rule["action"].ToString() == "disallow")
                            {
                                if (rule["os"] == null)
                                {
                                    Allow = false;
                                }
                                else
                                {
                                    if (rule["os"].HasValues)
                                    {
                                        if (rule["os"]["name"] != null)
                                        {
                                            if (rule["os"]["name"].ToString() == "windows" && Environment.OSVersion.Platform == PlatformID.Win32NT)
                                            {
                                                Allow = false;
                                            }
                                            else
                                            {
                                                Allow = true;
                                            }
                                        }
                                        if (rule["os"]["version"] != null)
                                        {
                                            if (rule["os"]["version"].ToString() == "^10\\" && Environment.OSVersion.Version.Major >= 10)
                                            {
                                                Allow = false;
                                            }
                                            else
                                            {
                                                Allow = true;
                                            }
                                        }
                                        if (rule["os"]["arch"] != null)
                                        {
                                            if (rule["os"]["arch"].ToString() == "x86" && Environment.Is64BitOperatingSystem == false)
                                            {
                                                Allow = false;
                                            }
                                            else
                                            {
                                                Allow = true;
                                            }
                                        }
                                        else
                                        {
                                            //todo 提上 CL 日程？或者 MEFL 日程？
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Allow = false;
                    }
                }
                else
                {
                    Allow = true;
                }
                if (Allow)
                {
                    itm.Natives = new List<Natives>();
                    itm.Downloads = new Downloads();
                    itm.Name = job["name"].ToString();
                    if (job["natives"] != null)
                    {
                        foreach (var nativeItem in (JObject)job["natives"])
                        {
                            Natives ntv = new Natives();
                            ntv.Platform = nativeItem.Key;
                            ntv.Id = nativeItem.Value.ToString();
                            itm.Natives.Add(ntv);
                        }
                    }
                    if (job["downloads"] != null)
                    {
                        if (job["downloads"]["artifact"] != null)
                        {
                            if (Allow)
                            {
                                itm.Downloads.Artifact = JsonConvert.DeserializeObject<Artifact>(job["downloads"]["artifact"].ToString());
                            }
                        }
                        if (job["downloads"]["classifiers"] != null)
                        {
                            var artifactWithPlatform = new ArtifactWithPlatform();
                            var classifiersJObject = job["downloads"]["classifiers"];
                            //if (classifiersJObject["javadoc"] != null)
                            //{
                            //    var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["javadoc"].ToString());
                            //    artifactWithPlatform.Item = artifact;
                            //    artifact = null;
                            //    artifactWithPlatform.Platform = "javadoc";
                            //    itm.Downloads.Classifiers.Add(artifactWithPlatform);
                            //    artifactWithPlatform = new ArtifactWithPlatform();
                            //}
                            //if (classifiersJObject["sources"] != null)
                            //{
                            //    var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["sources"].ToString());
                            //    artifactWithPlatform.Item = artifact;
                            //    artifact = null;
                            //    artifactWithPlatform.Platform = "sources";
                            //    itm.Downloads.Classifiers.Add(artifactWithPlatform);
                            //    artifactWithPlatform = new ArtifactWithPlatform();
                            //}
                            switch (System.Environment.OSVersion.Platform)
                            {
                                case PlatformID.Win32S:
                                    throw new UnSpportException("不支持 Win32S 操作系统");
                                    break;
                                case PlatformID.Win32Windows:
                                    throw new UnSpportException("不支持 Win32Windows 操作系统");
                                    break;
                                case PlatformID.Win32NT:
                                    if (classifiersJObject["natives-windows"] != null)
                                    {
                                        var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["natives-windows"].ToString());
                                        artifactWithPlatform.Item = artifact;
                                        artifact = null;
                                        artifactWithPlatform.Platform = "natives-windows";
                                        itm.Downloads.Classifiers.Add(artifactWithPlatform);
                                        artifactWithPlatform = new ArtifactWithPlatform();
                                    }
                                    if (classifiersJObject["natives-windows-64"] != null && System.Environment.Is64BitOperatingSystem == true)
                                    {
                                        var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["natives-windows-64"].ToString());
                                        artifactWithPlatform.Item = artifact;
                                        artifact = null;
                                        artifactWithPlatform.Platform = "natives-windows-64";
                                        itm.Downloads.Classifiers.Add(artifactWithPlatform);
                                        artifactWithPlatform = new ArtifactWithPlatform();
                                    }
                                    if (classifiersJObject["natives-windows-32"] != null && System.Environment.Is64BitOperatingSystem == false)
                                    {
                                        var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["natives-windows-32"].ToString());
                                        artifactWithPlatform.Item = artifact;
                                        artifact = null;
                                        artifactWithPlatform.Platform = "natives-windows-32";
                                        itm.Downloads.Classifiers.Add(artifactWithPlatform);
                                        artifactWithPlatform = new ArtifactWithPlatform();
                                    }
                                    break;
                                case PlatformID.WinCE:
                                    throw new UnSpportException("不支持 WinCE 操作系统");
                                    break;
                                case PlatformID.Unix:
                                    throw new UnSpportException("不支持 Unix 操作系统");
                                    break;
                                case PlatformID.Xbox:
                                    throw new UnSpportException("不支持 Xbox 操作系统");
                                    break;
                                case PlatformID.MacOSX:
                                    throw new UnSpportException("不支持 MacOSX 操作系统");
                                    break;
#if NET6_0
                                case PlatformID.Other:
                                    throw new UnSpportException("不支持 Other 操作系统");
                                    break;
#endif
                            }

                        }
                    }
                    else if (itm.Name.Contains("optifine"))
                    {
                        var pathr = itm.Name.Replace(":", "/");
                        var name = string.Empty;
                        if (pathr.Contains("launchwrapper-of"))
                        {
                            name = Path.GetFileName(pathr) + ".jar";
                            name = $"launchwrapper-of-{name}";
                        }
                        else if (itm.Name.Contains("OptiFine:"))
                        {
                            name = (itm.Name.Replace(":", "-") + ".jar").Replace("optifine-", string.Empty); ;
                        }
                        itm.Downloads.Artifact = new();
                        itm.Downloads.Artifact.Path = $"{pathr}/{name}";
                    }
                    ret.Add(itm);
                }
            }
            return ret;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }

    public class Root
    {
        [JsonProperty("inheritsFrom")]
        public string InheritsFrom { get; set; }
        [JsonProperty("arguments")]
        public Arguments Arguments { get; set; }

        public AssetIndex AssetIndex { get; set; }

        public string Assets { get; set; }

        public int ComplianceLevel { get; set; }

        public Downloads Downloads { get; set; }
        public string Id { get; set; }
        public JavaVersion JavaVersion { get; set; }
        [JsonProperty("libraries")]
        [JsonConverter(typeof(LibrariesArrayItemConverter))]
        public List<LibrariesArrayItem> Libraries { get; set; }


        public Logging Logging { get; set; }
        public string MainClass { get; set; }
        public int MinimumLauncherVersion { get; set; }
        public string ReleaseTime { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        [JsonProperty("minecraftArguments")]
        public string MinecraftArguments { get; set; }
    }

    public class Arguments
    {
        [JsonProperty("game")]
        public JToken Game { get; set; }
        [JsonConverter(typeof(ArgsWithRulesConverter))]
        [JsonProperty("jvm")]
        public List<String> Jvm { get; set; }

    }
    
    public class ArgsWithRulesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            List<String> res = new List<string>();
                var jT = JToken.ReadFrom(reader);
                foreach (var item in jT)
                {
                    if (item.HasValues)
                    {
                        var Allow = false;
                        try
                        {
                            foreach (var rule in item["rules"])
                            {
                                if (rule["action"].ToString() == "allow")
                                {
                                    if (rule["os"] == null)
                                    {
                                        Allow = true;
                                    }
                                    else
                                    {
                                        if (rule["os"].HasValues)
                                        {
                                            if (rule["os"]["name"] != null)
                                            {
                                                if (rule["os"]["name"].ToString() == "windows" && Environment.OSVersion.Platform == PlatformID.Win32NT)
                                                {
                                                    Allow = true;
                                                }
                                                else
                                                {
                                                    Allow = false;
                                                }
                                            }
                                            if (rule["os"]["version"] != null)
                                            {
                                                if (rule["os"]["version"].ToString() == "^10\\." && Environment.OSVersion.Version.Major >= 10)
                                                {
                                                    Allow = true;
                                                }
                                                else
                                                {
                                                    Allow = false;
                                                }
                                            }
                                            if (rule["os"]["arch"] != null)
                                            {
                                                if (rule["os"]["arch"].ToString() == "x86" && Environment.Is64BitOperatingSystem == false)
                                                {
                                                    Allow = true;
                                                }
                                                else
                                                {
                                                    Allow = false;
                                                }
                                            }
                                            else
                                            {
                                                
                                            }
                                        }
                                    }
                                }
                                else if (rule["action"].ToString() == "disallow")
                                {
                                    if (rule["os"] == null)
                                    {
                                        Allow = false;
                                    }
                                    else
                                    {
                                        if (rule["os"].HasValues)
                                        {
                                            if (rule["os"]["name"] != null)
                                            {
                                                if (rule["os"]["name"].ToString() == "windows" && Environment.OSVersion.Platform == PlatformID.Win32NT)
                                                {
                                                    Allow = false;
                                                }
                                                else
                                                {
                                                    Allow = true;
                                                }
                                            }
                                            if (rule["os"]["version"] != null)
                                            {
                                                if (rule["os"]["version"].ToString() == "^10\\" && Environment.OSVersion.Version.Major >= 10)
                                                {
                                                    Allow = false;
                                                }
                                                else
                                                {
                                                    Allow = true;
                                                }
                                            }
                                            if (rule["os"]["arch"] != null)
                                            {
                                                if (rule["os"]["arch"].ToString() == "x86" && Environment.Is64BitOperatingSystem == false)
                                                {
                                                    Allow = false;
                                                }
                                                else
                                                {
                                                    Allow = true;
                                                }
                                            }
                                            else
                                            {
                                                //todo 提上 CL 日程？或者 MEFL 日程？
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Allow = false;
                        }
                        if (Allow)
                        {
                            if (item["value"].HasValues)
                            {
                                foreach (var vls in item["value"])
                                {
                                    res.Add(vls.ToString());
                                }
                            }
                            else
                            {
                                res.Add(item["value"].ToString());
                            }
                        }
                    }
                    else
                    {
                        res.Add(item.ToString());
                    }
                }
            //reader.Close();
            return res;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class RuleValuePair
    {
        public Rules Rules { get; set; }

        public Value Value { get; set; }
    }

    [JsonConverter(typeof(RulesArrayConverter))]
    public class Rules
    {
        public class RulesArrayConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var result = (JArray)serializer.Deserialize(reader);
                var rules = new Rules();
                var actionFeaturesPairJsonObject = result.First as JObject;// 原型
                var actionFeaturesInstance = JsonConvert.DeserializeObject<ActionFeaturesOSGroup>(actionFeaturesPairJsonObject.ToString());
                rules.ActionFeaturesOSGroup = actionFeaturesInstance;
                return rules;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        public ActionFeaturesOSGroup ActionFeaturesOSGroup { get; set; }
    }

    public class ActionFeaturesOSGroup
    {

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("features")]
        public Features Features { get; set; }

        [JsonProperty("os")]
        public Os Os { get; set; }

    }


    public class Features
    {
        [JsonProperty("is_demo_user")]
        public bool Is_demo_user { get; set; }

        [JsonProperty("has_custom_resolution")]
        public bool Has_custom_resolution { get; set; }

    }


    [JsonConverter(typeof(ValueConverter))]
    public class Value
    {
        public class ValueConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var result = serializer.Deserialize(reader);// as JArray;
                var value = new Value();
                value.Array = new List<string>();

                if (result is string)
                    value.Array.Add(result as string);
                else if (result is JArray)
                {
                    foreach (var item in (result as JArray))
                    {
                        if (item is JValue)
                            value.Array.Add((item as JValue).Value<string>());
                    }
                }

                return value;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        public List<string> Array { get; set; }

    }

    public class Os
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("arch")]
        public string Arch { get; set; }

    }

    public class AssetIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("totalSize")]
        public string TotalSize { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class Downloads
    {
        public override string ToString()
        {
            if (Client == null)
            {
                if (!(Artifact.Url == null))
                {
                    return Artifact.Url;
                }
                else
                {
                    return base.ToString();
                }
            }
            else
            {
                return base.ToString();
            }
        }
        [JsonProperty("client")]
        public Client Client { get; set; }

        [JsonProperty("client_mappings")]
        public Client_mappings Client_mappings { get; set; }

        [JsonProperty("server")]
        public Server Server { get; set; }

        [JsonProperty("server_mappings")]
        public Server_mappings Server_mappings { get; set; }

        [JsonProperty("artifact")]
        public Artifact Artifact { get; set; }

        [JsonProperty("classifiers")]
        public List<ArtifactWithPlatform> Classifiers { get; set; }

        public Downloads()
        {
            Classifiers = new List<ArtifactWithPlatform>();
        }
    }

    public class Client
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("argument")]
        public string Argument { get; set; }

        [JsonProperty("file")]
        public File File { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

    }

    public class Client_mappings
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class Server
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class Server_mappings
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class JavaVersion
    {
        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("majorVersion")]
        public int MajorVersion { get; set; }

    }
    public class LibrariesArrayItem
    {
        public override string ToString()
        {
            if (Name != null)
            {
                return $"{Name}";
            }
            else
            {
                return base.ToString();
            }
        }
        [JsonProperty("downloads")]
        public Downloads Downloads { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("natives")]
        public List<Natives> Natives { get; set; }

        //[JsonProperty("rules")]
        //public JToken Rules { get; set; }

        [JsonProperty("extract")]
        public Extract Extract { get; set; }
    }

    public class Artifact
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class ArtifactWithPlatform
    {
        public Artifact Item { get; set; }
        public string Platform { get; set; }
    }
    public class Natives
    {
        public string Platform { get; set; }
        public string Id { get; set; }
    }

    public class Extract
    {
        [JsonProperty("exclude")]
        public Exclude Exclude { get; set; }

    }


    [JsonConverter(typeof(ExcludeArrayConverter))]
    public class Exclude
    {

        public class ExcludeArrayConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var result = (JArray)serializer.Deserialize(reader);
                var exc = new Exclude();
                exc.Array = new List<string>();
                foreach (var item in result)
                {
                    exc.Array.Add(item.Value<string>());
                }
                return exc;

            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        [JsonProperty("Array")]
        public List<string> Array { get; set; }

    }


    public class Logging
    {
        [JsonProperty("client")]
        public Client Client { get; set; }

    }

    public class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }
}
