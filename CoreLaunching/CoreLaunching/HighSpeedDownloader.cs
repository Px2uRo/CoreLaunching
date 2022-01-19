using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace CoreLaunching
{
        /// <summary>
        /// Manage Thread Files
        /// </summary>
    public class SuperDownloader
    {
        public static bool[] threadEnd; //每个线程结束标志  
        public static string[] fileNameEveryThread;//每个线程接收文件的文件名  
        public static int[] fileStartEveryThread;//每个线程接收文件的起始位置  
        public static int[] fileSizeEveryThread;//每个线程接收文件的大小  
        public static string targetUrl;//接受文件的URL  
        public static bool isHeBingLe;//文件合并标志  
        public static int thread;//进程数
        public class HttpFile
        {
            public int currectThreadNum;//Threade Number
            public string fileName { get; set; }//FileName
            public string streamUrl;//Url
            public FileStream fs;
            public byte[] nbytes;//HuanChongBytes
            public HttpFile(int thread)//构造方法  
            {
                currectThreadNum = thread;
            }
            public void receive(string url)
            {
                fileName = SuperDownloader.fileNameEveryThread[currectThreadNum];
                streamUrl = url;
                nbytes = new byte[512];
                Console.WriteLine("Thread {0} is ready to recive", currectThreadNum.ToString());
                fs = new FileStream(fileName, System.IO.FileMode.Create);
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.AddRange(SuperDownloader.fileStartEveryThread[currectThreadNum],
                    SuperDownloader.fileStartEveryThread[currectThreadNum]+SuperDownloader.fileSizeEveryThread[currectThreadNum]);
                    System.IO.Stream stream;
                    stream = request.GetResponse().GetResponseStream();
                    int nreadsize = stream.Read(nbytes, 0, 512);
                    while(nreadsize > 0)
                    {
                        fs.Write(nbytes, 0, nreadsize);
                        nreadsize = stream.Read(nbytes, 0, 512);
                        Console.WriteLine("Thread {0} is Now reciving", currectThreadNum.ToString());
                    }
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    fs.Close();
                }

                Console.WriteLine("Thread {0} isalready recived!", currectThreadNum.ToString());
            }
        }
    }
}
