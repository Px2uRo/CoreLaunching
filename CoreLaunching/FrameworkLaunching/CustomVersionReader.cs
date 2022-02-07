using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkLaunching
{
    /// <summary>
    /// 版本信息管理器。
    /// </summary>
    public partial class VersionMgr
    {
        /// <summary>
        /// 表示加载JSON
        /// </summary>
        /// <param name="path">文件路径</param>
        public void LoadJson(string path)
        {
            try//没有的时候不报错。
            {
                StreamReader file = File.OpenText(path);//打开文件备用。
                JsonTextReader reader = new JsonTextReader(file);//NewtonJson读取 file 文件。
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);//强制转换 一个抽象的 JSON 令牌 到一个 JSON 对象。
                var id = jsonObject["id"]; //读取 JSON 里面的 id 项目。
                Console.WriteLine(id);
                file.Close();
            }
            catch
            {

            }
        }
    }
}
