using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CLDemo.Data
{
    public class GameInfo
    {
        public string Id { get; set; }
        public string JsonPath { get; set; }
        public string JarPath { get; set; }
        public GameInfo(string JsonPath)
        {
            this.JsonPath = JsonPath;
            if (File.Exists(JsonPath))
            {
                JarPath = JsonPath.Replace(".json", ".jar");
                Id = System.IO.Path.GetFileNameWithoutExtension(JsonPath);
            }
            else
            {
                this.Id = "解析失败了哦，Json 文件不存在";
                this.JarPath = "解析失败了哦，Jar 文件不存在";
                this.JsonPath = null;
            }
        }
    }
}
