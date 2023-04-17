using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CoreLaunching.JsonTemplates
{

    public class InstallProfile
    {
        [JsonProperty("spec")]
        public int Spec { get; set; }
        [JsonProperty("profile")]
        public string Profile { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("path")]
        public object Path { get; set; }
        [JsonProperty("minecraft")]
        public string Minecraft { get; set; }
        [JsonProperty("serverJarPath")]
        public string ServerJarPath { get; set; }
        [JsonConverter(typeof(InstallProfileDataConverter))]
        [JsonProperty("data")]
        public Dictionary<string,ClientAndServerPair> Data { get; set; }
        [JsonProperty("processors")]
        public Processor[] Processors { get; set; }
        [JsonProperty("libraries")]
        [JsonConverter(typeof(LibrariesArrayItemConverter))]
        public List<LibrariesArrayItem> Libraries { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("json")]
        public string Json { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("mirrorList")]
        public string MirrorList { get; set; }
        [JsonProperty("welcome")]
        public string Welcome { get; set; }
    }

    internal class InstallProfileDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var diction = new Dictionary<string, ClientAndServerPair>();
            var jOb = JObject.ReadFrom(reader);
            foreach (JProperty item in jOb)
            {

                diction.Add($"{{{item.Name}}}",JsonConvert.DeserializeObject<ClientAndServerPair>(item.Value.ToString()));
            }
            return diction;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class ClientAndServerPair
    {
        [JsonProperty("client")]
        public string Client { get; set; }
        [JsonProperty("server")]
        public string Server { get; set; }
        public ClientAndServerPair()
        {

        }
        public ClientAndServerPair(string client, string server)
        {
            Client = client;
            Server = server;
        }
    }

    public class Processor
    {
        [JsonProperty("sides")]
        public string[] Sides { get; set; }
        [JsonProperty("jar")]
        public string Jar { get; set; }
        [JsonProperty("classpath")]
        public string[] ClassPath { get; set; }
        [JsonProperty("args")]
        public string[] Args { get; set; }
        [JsonProperty("outputs")]
        public Outputs Outputs { get; set; }
        public bool IsForClient { 
            get {
                if (Sides == null)
                {
                    return true;
                }
                else
                {
                    return Sides.Contains("client");
                }
            } 
        }

        public bool IsForServer
        {
            get
            {
                if (Sides == null)
                {
                    return true;
                }
                else
                {
                    return Sides.Contains("server");
                }
            }
        }

        public Process GetProcess(string javaExePath,string librariesPath, Dictionary<string, ClientAndServerPair> data)
        {
            var process = new Process();
            var inf = new ProcessStartInfo();
            inf.WorkingDirectory = librariesPath;
            inf.FileName = javaExePath;
            inf.Arguments = GetArgs(librariesPath, data);
            inf.RedirectStandardOutput= true;
            process.StartInfo= inf;
            return process;
        }

        private string GetArgs(string librariesPath, Dictionary<string, ClientAndServerPair> data)
        {
            var args = "-cp ";
            var cps = Jar.GetLibraryFileName(librariesPath)+";";
            foreach (var item in ClassPath)
            {
                var fileP = item.GetLibraryFileName(librariesPath);
                cps += fileP + ";";
            }
            cps.Remove(cps.Length-2,1);
            if (cps.Contains(" ")) { cps = $"\"{cps}\""; }
            args+= cps;
            args += (" " + GetMainClassFromPackName(librariesPath) + " ");
            var MainCArgs = "";
            foreach (var item in Args)
            {
                if (item.Contains("@"))
                {
                    var nP = item;
                    nP = nP.Replace("[", "").Replace("]","");
                    nP = nP.GetLibraryFileName(librariesPath);
                    if (nP.Contains(" ")) {nP=$"\"{nP}\""; }
                    MainCArgs += nP + " ";
                }
                else
                {
                    MainCArgs += item + " ";
                }
            }
            foreach (var item in data)
            {
                if (MainCArgs.Contains(item.Key))
                {
                    if (item.Value.Client.Contains("@"))
                    {
                        var nP = item.Value.Client;
                        nP = nP.Replace("[", "").Replace("]", "");
                        nP = nP.GetLibraryFileName(librariesPath);
                        if (nP.Contains(" ")) { nP = $"\"{nP}\""; }
                        MainCArgs = MainCArgs.Replace(item.Key,nP);
                    }
                    else
                    {
                        MainCArgs = MainCArgs.Replace(item.Key, item.Value.Client);
                    }
                }
            }
            args += MainCArgs;

            return args;
        }
        /// <summary>
        /// 代码来自 TT702，谢谢 TT702.
        /// </summary>
        /// <returns></returns>
        private string GetMainClassFromPackName(string librariesPath)
        {
            var path = Jar.GetLibraryFileName(librariesPath);
            Directory.CreateDirectory(Path.GetFileName(path));
            using (var zip = System.IO.Compression.ZipFile.OpenRead(path))
            {
                var mainfest = zip.GetEntry("META-INF/MANIFEST.MF");
                var stream = new StreamReader(mainfest.Open());
                var currentLine = String.Empty;

                while ((currentLine = stream.ReadLine()) != null)
                {
                    if (currentLine.Contains("Main-Class:"))
                    {
                        stream.Close();
                        return $"{currentLine.Replace("Main-Class:", "").Trim()} ";
                    }
                }
                stream.Close();
                return "";
            }
        }

    }

    public class Outputs
    {
        public string MC_SLIM { get; set; }
        public string MC_EXTRA { get; set; }
    }
}
