using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FrameworkLaunching
{
    /// <summary>
    /// 这是下载Minecraft游戏文件使用的一个类。
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// C# 普通下载
        /// </summary>
        /// <param name="url">设置下载文件源</param>
        /// <param name="path">下载文件到哪里</param>
        public void NormalDownload(string url,string path)
        {
            var fileName = Path.GetFileName(url);
            var fullPath = path + fileName;
            var request = WebRequest.Create(url) as HttpWebRequest; //设置参数
            var response = request.GetResponse() as HttpWebResponse; //发送请求并获取相应回应数据
            var responseStream = response.GetResponseStream(); //直到request.GetResponse()程序才开始向目标网页发送Post请求
            var stream = new FileStream(fullPath, FileMode.Create); //创建本地文件写入流
            var bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
        }
    }
}
