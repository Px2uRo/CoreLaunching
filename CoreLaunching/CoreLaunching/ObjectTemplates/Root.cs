using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreLaunching.ObjectTemplates
{
	class Root
	{
		[JsonProperty("arguments")]
		public Arguments arguments { get; set; }

		public AssetIndex assetIndex { get; set; }

		public string assets { get; set; }

		public int complianceLevel { get; set; }

		public Downloads downloads { get; set; }
		public string id { get; set; }
		public JavaVersion javaVersion { get; set; }
		public Libraries libraries { get; set; }
		public Logging logging { get; set; }
		public string mainClass { get; set; }
		public int minimumLauncherVersion { get; set; }
		public string releaseTime { get; set; }
		public string time { get; set; }
		public string type { get; set; }

		public string minecraftArguments { get; set; }
	}

	class Arguments
	{
		[JsonProperty("game")]
		public Game game { get; set; }
		[JsonProperty("jvm")]
		public Jvm jvm { get; set; }

	}

	class RuleValuePair
	{
		public Rules rules { get; set; }

		public Value value { get; set; }
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
		public string action { get; set; }

		[JsonProperty("features")]
		public Features features { get; set; }

		[JsonProperty("os")]
		public Os os { get; set; }

	}


	class Features
	{
		[JsonProperty("is_demo_user")]
		public bool is_demo_user { get; set; }

		[JsonProperty("has_custom_resolution")]
		public bool has_custom_resolution { get; set; }

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
		public string name { get; set; }

		[JsonProperty("version")]
		public string version { get; set; }

		[JsonProperty("arch")]
		public string arch { get; set; }

	}

	class AssetIndex
	{
		[JsonProperty("id")]
		public string id { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("totalSize")]
		public string totalSize { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Downloads
	{
		[JsonProperty("client")]
		public Client client { get; set; }

		[JsonProperty("client_mappings")]
		public Client_mappings client_mappings { get; set; }

		[JsonProperty("server")]
		public Server server { get; set; }

		[JsonProperty("server_mappings")]
		public Server_mappings server_mappings { get; set; }

		[JsonProperty("artifact")]
		public Artifact artifact { get; set; }

		[JsonProperty("classifiers")]
		public Classifiers classifiers { get; set; }

	}

	class Client
	{
		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

		[JsonProperty("argument")]
		public string argument { get; set; }

		[JsonProperty("file")]
		public File file { get; set; }

		[JsonProperty("type")]
		public string type { get; set; }

	}

	class Client_mappings
	{
		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Server
	{
		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Server_mappings
	{
		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class JavaVersion
	{
		[JsonProperty("component")]
		public string component { get; set; }

		[JsonProperty("majorVersion")]
		public string majorVersion { get; set; }

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
		public Downloads downloads { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("natives")]
		public Natives natives { get; set; }

		[JsonProperty("rules")]
		public Rules rules { get; set; }

		[JsonProperty("extract")]
		public Extract extract { get; set; }
	}

	class Artifact
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Classifiers
	{
		[JsonProperty("javadoc")]
		public Javadoc javadoc { get; set; }

		[JsonProperty("natives-linux")]
		public Natives_linux natives_linux { get; set; }

		[JsonProperty("natives-macos")]
		public Natives_macos natives_macos { get; set; }

		[JsonProperty("natives-windows")]
		public Natives_windows natives_windows { get; set; }

		[JsonProperty("sources")]
		public Sources sources { get; set; }

		[JsonProperty("natives-osx")]
		public Natives_osx natives_osx { get; set; }

	}

	class Javadoc
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Natives_linux
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Natives_macos
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Natives_windows
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Sources
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Natives
	{
		[JsonProperty("osx")]
		public string osx { get; set; }

		[JsonProperty("linux")]
		public string linux { get; set; }

		[JsonProperty("windows")]
		public string windows { get; set; }

	}

	class Extract
	{
		[JsonProperty("exclude")]
		public Exclude exclude { get; set; }

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



	class Natives_osx
	{
		[JsonProperty("path")]
		public string path { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}

	class Logging
	{
		[JsonProperty("client")]
		public Client client { get; set; }

	}

	class File
	{
		[JsonProperty("id")]
		public string id { get; set; }

		[JsonProperty("sha1")]
		public string sha1 { get; set; }

		[JsonProperty("size")]
		public string size { get; set; }

		[JsonProperty("url")]
		public string url { get; set; }

	}





}
