using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using template = CoreLaunching.ObjectTemples;

namespace CoreLaunching
{
    public class VersionManifestReader
    {
        public List<template.versions> verInfo;
        public template.latest latestInfo;
        public void Load(string Target)
        {
            StreamReader loader = File.OpenText(Target);
            JsonTextReader reader = new JsonTextReader(loader);//NewtonJson读取文件。
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
            JToken versionToken = jsonObject["versions"];
            JToken latestToken = jsonObject["latest"];
            verInfo = JsonConvert.DeserializeObject<List<template.versions>>(versionToken.ToString());
            latestInfo = JsonConvert.DeserializeObject<template.latest>(latestToken.ToString());
        }
    }
}
