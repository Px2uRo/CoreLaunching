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
	class Root
	{
		[JsonProperty("arguments")]
		public Arguments Arguments { get; set; }

		public AssetIndex AssetIndex { get; set; }

		public string Assets { get; set; }

		public int ComplianceLevel { get; set; }

		public Downloads Downloads { get; set; }
		public string Id { get; set; }
		public JavaVersion JavaVersion { get; set; }
		public Libraries Libraries { get; set; }
		public Logging Logging { get; set; }
		public string MainClass { get; set; }
		public int MinimumLauncherVersion { get; set; }
		public string ReleaseTime { get; set; }
		public string Time { get; set; }
		public string Type { get; set; }
		[JsonProperty("minecraftArguments")]
		public string MinecraftArguments { get; set; }
	}

	class Arguments
	{
		[JsonProperty("game")]
		public Game Game { get; set; }
		[JsonProperty("jvm")]
		public Jvm Jvm { get; set; }

	}

	class RuleValuePair
	{
		public Rules Rules { get; set; }

		public Value Value { get; set; }
	}

	[JsonConverter(typeof(GameArrayConverter))]
	class Game
	{
		class GameArrayConverter : JsonConverter
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
	class Rules
	{
		class RulesArrayConverter : JsonConverter
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

	class ActionFeaturesOSGroup
	{

		[JsonProperty("action")]
		public string Action { get; set; }

		[JsonProperty("features")]
		public Features Features { get; set; }

		[JsonProperty("os")]
		public Os Os { get; set; }

	}


	class Features
	{
		[JsonProperty("is_demo_user")]
		public bool Is_demo_user { get; set; }

		[JsonProperty("has_custom_resolution")]
		public bool Has_custom_resolution { get; set; }

	}


	[JsonConverter(typeof(ValueConverter))]
	class Value
	{
		class ValueConverter : JsonConverter
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
	class Jvm
	{
        class JvmConverter : JsonConverter
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

	class Os
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("arch")]
		public string Arch { get; set; }

	}

	class AssetIndex
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

	class Downloads
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

		[JsonProperty("classifiers")]
		public Classifiers Classifiers { get; set; }

	}

	class Client
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

	class Client_mappings
	{
		[JsonProperty("sha1")]
		public string Sha1 { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

	}

	class Server
	{
		[JsonProperty("sha1")]
		public string Sha1 { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

	}

	class Server_mappings
	{
		[JsonProperty("sha1")]
		public string Sha1 { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

	}

	class JavaVersion
	{
		[JsonProperty("component")]
		public string Component { get; set; }

		[JsonProperty("majorVersion")]
		public int MajorVersion { get; set; }

	}

	[JsonConverter(typeof(LibrariesArrayConverter))]
	class Libraries
	{

		class LibrariesArrayConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return true;
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var result = (JArray)serializer.Deserialize(reader);
				var lib = new Libraries();
				lib.Array = new List<LibrariesArrayItem>();

				foreach (var item in result)
				{
					if (item is JObject)
					{
						var arrayItem = JsonConvert.DeserializeObject<LibrariesArrayItem>(item.ToString());
						lib.Array.Add(arrayItem);
					}

				}
				return lib;

			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}

		public List<LibrariesArrayItem> Array { get; set; }


	}

	class LibrariesArrayItem
	{
		[JsonProperty("downloads")]
		public Downloads Downloads { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("natives")]
		public Natives Natives { get; set; }

		[JsonProperty("rules")]
		public List<RuleValuePair> Rules { get; set; }

		[JsonProperty("extract")]
		public Extract Extract { get; set; }
	}

	class Artifact
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

	class Classifiers
	{
		[JsonProperty("javadoc")]
		public Javadoc Javadoc { get; set; }

		[JsonProperty("natives-linux")]
		public Natives_linux Natives_linux { get; set; }

		[JsonProperty("natives-macos")]
		public Natives_macos Natives_macos { get; set; }

		[JsonProperty("natives-windows")]
		public Natives_windows Natives_windows { get; set; }

		[JsonProperty("sources")]
		public Sources Sources { get; set; }

		[JsonProperty("natives-osx")]
		public Natives_osx Natives_osx { get; set; }

	}

	class Javadoc
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

	class Natives_linux
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

	class Natives_macos
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

	class Natives_windows
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



	class Natives_osx
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


	class Sources
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

	class Natives
	{
		[JsonProperty("osx")]
		public string Osx { get; set; }

		[JsonProperty("linux")]
		public string Linux { get; set; }

		[JsonProperty("windows")]
		public string Windows { get; set; }

	}

	class Extract
	{
		[JsonProperty("exclude")]
		public Exclude Exclude { get; set; }

	}


	[JsonConverter(typeof(ExcludeArrayConverter))]
	class Exclude
	{

		class ExcludeArrayConverter : JsonConverter
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


	class Logging
	{
		[JsonProperty("client")]
		public Client Client { get; set; }

	}

	class File
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
