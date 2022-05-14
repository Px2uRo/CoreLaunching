using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CoreLaunching
{
    #region JSON 文档转 JsonObject
    public static class FileToJObject 
    {
        /// <summary>
        /// Json 转 JObject
        /// </summary>
        /// <param name="TargetFile">目标文件</param>
        /// <returns>JObject</returns>
        public static JObject LoadJson(string TargetFile)
        {
            StreamReader loader = File.OpenText(TargetFile);
            JsonTextReader reader = new JsonTextReader(loader);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            loader.Close();
            reader.Close();
            loader = null;
            reader = null;
            return jsonObject;
        }
    }
    #endregion
}