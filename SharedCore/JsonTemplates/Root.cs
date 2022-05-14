﻿using System;
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
    public class Root
    {
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
                    itm.Natives = new List<Natives>();
                    itm.Downloads = new Downloads();
                    var job = (JObject)item;
                    itm.Name = job["name"].ToString();
                    if (job["natives"] != null)
                    {
                        foreach (var nativeItem in (JObject)job["natives"])
                        {
                            Natives ntv = new Natives();
                            ntv.Platform = nativeItem.Key;
                            ntv.Id = nativeItem.Value.ToString();
                            itm.Natives.Add(ntv);
                            ntv = null;
                        }
                    }
                    if (job["downloads"] != null)
                    {
                        if (job["downloads"]["artifact"] != null)
                        {
                            itm.Downloads.Artifact = JsonConvert.DeserializeObject<Artifact>
                                (job["downloads"]["artifact"].ToString());
                        }
                        if (job["downloads"]["classifiersJObject"] != null)
                        {
                            var artifactWithPlatform = new ArtifactWithPlatform();
                            var classifiersJObject = job["downloads"]["classifiersJObject"];
                            if (classifiersJObject["javadoc"] != null)
                            {
                                var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["javadoc"].ToString());
                                artifactWithPlatform.Item = artifact;
                                artifact = null;
                                artifactWithPlatform.Platform = "javadoc";
                                itm.Downloads.ClassifiersJObject.Add(artifactWithPlatform);
                                artifactWithPlatform = new ArtifactWithPlatform();
                            }
                            if (classifiersJObject["sources"]!=null)
                            {
                                var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["sources"].ToString());
                                artifactWithPlatform.Item = artifact;
                                artifact = null;
                                artifactWithPlatform.Platform = "sources";
                                itm.Downloads.ClassifiersJObject.Add(artifactWithPlatform);
                                artifactWithPlatform = new ArtifactWithPlatform();
                            }
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
                                        itm.Downloads.ClassifiersJObject.Add(artifactWithPlatform);
                                        artifactWithPlatform = new ArtifactWithPlatform();
                                    }
                                    if (classifiersJObject["natives-windows-64"] != null&&System.Environment.Is64BitOperatingSystem==true)
                                    {
                                        var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["natives-windows-64"].ToString());
                                        artifactWithPlatform.Item = artifact;
                                        artifact = null;
                                        artifactWithPlatform.Platform = "natives-windows-64";
                                        itm.Downloads.ClassifiersJObject.Add(artifactWithPlatform);
                                        artifactWithPlatform = new ArtifactWithPlatform();
                                    }
                                    if (classifiersJObject["natives-windows-32"] != null&&System.Environment.Is64BitOperatingSystem == true)
                                    {
                                        var artifact = JsonConvert.DeserializeObject<Artifact>(classifiersJObject["natives-windows-32"].ToString());
                                        artifactWithPlatform.Item = artifact;
                                        artifact = null;
                                        artifactWithPlatform.Platform = "natives-windows-32";
                                        itm.Downloads.ClassifiersJObject.Add(artifactWithPlatform);
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
                    ret.Add(itm);
                    itm = null;
                }
                return ret;
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                
            }
        }

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
        public Game Game { get; set; }
        [JsonProperty("jvm")]
        public Jvm Jvm { get; set; }

    }

    public class RuleValuePair
    {
        public Rules Rules { get; set; }

        public Value Value { get; set; }
    }

    [JsonConverter(typeof(GameArrayConverter))]
    public class Game
    {
        public class GameArrayConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var result = (JArray)serializer.Deserialize(reader);

                var game = new Game();
                game.Array = new List<string>();
                game.RuleValuePairs = new List<RuleValuePair>();
                foreach (var arrayItem in result)
                {
                    if (arrayItem is JObject)
                    {
                        var pair = JsonConvert.DeserializeObject<RuleValuePair>(arrayItem.ToString());
                        game.RuleValuePairs.Add(pair);
                    }
                    else
                    {
                        game.Array.Add(arrayItem.Value<string>());
                    }
                }

                return game;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        public List<string> Array { get; set; }

        public List<RuleValuePair> RuleValuePairs { get; set; }

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


    [JsonConverter(typeof(JvmConverter))]
    public class Jvm
    {
        public class JvmConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var result = (JArray)serializer.Deserialize(reader);

                var jvm = new Jvm();
                jvm.Array = new List<string>();
                jvm.RuleValuePairs = new List<RuleValuePair>();
                foreach (var arrayItem in result)
                {
                    if (arrayItem is JObject)
                    {
                        var pair = JsonConvert.DeserializeObject<RuleValuePair>(arrayItem.ToString());
                        jvm.RuleValuePairs.Add(pair);
                    }
                    else
                    {
                        jvm.Array.Add(arrayItem.Value<string>());
                    }
                }

                return jvm;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        [JsonProperty("Array")]
        public List<string> Array { get; set; }
        public List<RuleValuePair> RuleValuePairs { get; set; }
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
        public string Size { get; set; }

        [JsonProperty("totalSize")]
        public string TotalSize { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class Downloads
    {
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

        [JsonProperty("classifiersJObject")]
        public List<ArtifactWithPlatform> ClassifiersJObject { get; set; }

        public Downloads()
        {
            Artifact = new Artifact();
            ClassifiersJObject = new List<ArtifactWithPlatform>();
        }
    }

    public class Client
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

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
        [JsonProperty("downloads")]
        public Downloads Downloads { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("natives")]
        public List<Natives> Natives { get; set; }

        [JsonProperty("rules")]
        public List<RuleValuePair> Rules { get; set; }

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
        public string Size { get; set; }

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