using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.Down.Web;
public static class HttpHelper
{

    public const long MaxPartSize = 10;

    public static void GetContent(byte[] buffer, string url)
    {
        var len = GetLength(url, out var response);
        response.Dispose();
    }

    ///<summary>断点续传。</summary>
    public static void GetContentByPart(byte[] buffer, string url, long startPoint, long endPoint)
    {
        var httpRequest = WebRequest.Create(url) as HttpWebRequest;
        httpRequest.Method = "GET";
        httpRequest.ContentType = "application/x-www-form-urlencoded";
        //httpRequest.Headers.Add("Range", $"bytes={startPoint}-{endPoint}");
        httpRequest.AddRange(startPoint, endPoint);
        httpRequest.Timeout = 30000; // 半分钟。

        using (var httpResponse = httpRequest.GetResponse())
        {
            using (var stream = httpResponse.GetResponseStream())
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var temp = ms.ToArray();
                    //stream.Read(buffer, (int)startPoint, (int)(endPoint - startPoint + 1));
                    //stream.Read(temp, 0, (int)(endPoint - startPoint + 1));
                    //Console.WriteLine($"{startPoint}-{endPoint}");
                    //Console.WriteLine(GetBufferInfo(temp));

                    Buffer.BlockCopy(temp, 0, buffer, (int)startPoint, temp.Length);
                    //GC.SuppressFinalize(temp);
                    //temp = null;
                }
            }
        }
        GC.Collect();
    }



    ///<summary>获得资源的长度。</summary>
    public static long GetLength(string url, out WebResponse response)
    {
        var httpRequest = WebRequest.Create(url);
        httpRequest.Method = "GET";
        httpRequest.ContentType = "application/x-www-form-urlencoded";

        httpRequest.Timeout = 30000; // 半分钟。
        var httpResponse = httpRequest.GetResponse();
        response = httpResponse;
        return httpResponse.ContentLength;
    }

    public static long GetLength(string url)
    {
        var len = GetLength(url, out var response);
        response.Dispose();
        return len;
    }
}
