using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLaunching.JsonTemplates;
using System.IO;
using System.Net;
using System.Threading;

namespace CoreLaunching
{
    [Obsolete("别用，会带来不幸",true)]
    public class MultiThreadDownloader
    {
        #region Arguments
        public string FinalLocalFolder;
        /// <summary>
        /// 网络文件地址
        /// </summary>
        public string webUrl;
        /// <summary>
        /// 线程数
        /// </summary>
        public int thread;
        #endregion

        #region 数组变量
        /// <summary>
        /// 线程结束布尔值数组
        /// </summary>
        bool[] threadw;
        /// <summary>
        /// 临时文件名数组
        /// </summary>
        string[] tmpFileNamew;
        /// <summary>
        /// 临时文件开始数组
        /// </summary>
        int[] tmpFileSizeStartw;
        /// <summary>
        /// 临时文件大小数组
        /// </summary>
        int[] tmpFileSizew;
        #endregion

        class HttpFile
        {
            #region 此类变量
            public MultiThreadDownloader arg;
            public int threadindex;//线程代号  
            public string filename;//文件名  
            public string strUrl;//接收文件的URL  
            public FileStream fs;
            public HttpWebRequest request;
            public System.IO.Stream ns;
            public byte[] nbytes;//接收缓冲区  
            public int nreadsize;//接收字节数  
            #endregion
            public HttpFile(MultiThreadDownloader multiarg,int threadsNum)//构造函数
            {
                threadindex= threadsNum;
                arg = multiarg;
            }

            public void Receive()//接收线程  
            {
                filename = arg.tmpFileNamew[threadindex];
                strUrl = arg.webUrl;
                ns = null;
                nbytes = new byte[512];
                nreadsize = 0;
                Console.WriteLine("线程" + threadindex.ToString() + "开始接收");
                fs = new FileStream(filename, FileMode.OpenOrCreate);
                bool logged = false;
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                    //接收的起始位置及接收的长度   
                    request.AddRange(arg.tmpFileSizeStartw[threadindex],
                    arg.tmpFileSizeStartw[threadindex] + arg.tmpFileSizew[threadindex]);
                    ns = request.GetResponse().GetResponseStream();//获得接收流  
                    nreadsize = ns.Read(nbytes, 0, 512);
                    while (nreadsize > 0)
                    {
                        fs.Write(nbytes, 0, nreadsize);
                        nreadsize = ns.Read(nbytes, 0, 512);
                        if (logged == false)
                        {
                            Console.WriteLine("线程" + threadindex.ToString() + "正在接收");
                            logged = true;
                        }
                    }
                    fs.Close();
                    ns.Close();
                }
                catch (Exception er)
                {
                    Console.WriteLine(er.Message);
                    fs.Close();
                }
                Console.WriteLine("进程" + threadindex.ToString() + "接收完毕!");
                arg.threadw[threadindex] = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="threadNum"></param>
        /// <param name="localFolder"></param>
        public void GoGoGo(Uri url, int threadNum, string localFolder)
        {
            GoGoGo(url.ToString(), threadNum, localFolder);
        }

        public void GoGoGo(string url, int threadNum, string localFolder)
        {
            #region 设置一堆参数
            localFolder = Path.Combine(localFolder, Path.GetFileName(url));
            FinalLocalFolder = localFolder;//这个是其他成员会用到的参数，请定义在类下面而不是成员下面。
            webUrl = url;//同上
            HttpWebRequest request;
            long filesize = 0;
            #endregion

            DateTime dt = DateTime.Now;
            Console.WriteLine("开始时间={0}", dt);//打印一行日志
            DirectoryInfo dir = new DirectoryInfo(localFolder + "tmp");
            dir.Create();//创建临时文件文件夹。

            #region 尝试对web进行请求。
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(webUrl);
                filesize = request.GetResponse().ContentLength;//取得目标文件的长度
                Console.WriteLine("正在下载 {1} 到 {2}，文件大小：{0} kb", filesize,webUrl,dir);
                request.Abort();
            }
            catch (Exception er)
            {
                Console.WriteLine("发生错误，详情:\n{0}", er.Message);
            }

            #endregion

            // 确认线程数组的数量，这些都是定义在类下面的。
            thread = threadNum;
            threadw = new bool[thread];
            tmpFileNamew = new string[thread];
            tmpFileSizeStartw = new int[thread];
            tmpFileSizew = new int[thread];

            #region 计算每个线程应该接收文件的大小
            int Shang = (int)filesize / thread;//商
            int Yushu = Shang + (int)filesize % thread;//余数
            #region 为数组赋值
            for (int i = 0; i < thread; i++)
            {
                threadw[i] = false;//每个线程状态的初始值为假，方便最后合并时验证线程是否接受完毕
                tmpFileNamew[i] = Path.Combine(localFolder+"tmp", i.ToString() + ".tmp");//每个线程接收文件的临时文件名  
                if (i < thread - 1)
                {
                    tmpFileSizeStartw[i] = Shang * i;//每个线程接收文件的起始点  
                    tmpFileSizew[i] = Shang - 1;//每个线程接收文件的长度，减一是因为不把后面起点位置占用。
                }
                else
                {
                    tmpFileSizeStartw[i] = Shang * i;
                    tmpFileSizew[i] = Yushu - 1;
                }
            }
            #endregion
            #endregion

            #region 开始多线程
            Thread[] threadk = new Thread[thread];//定义线程数组。
            HttpFile[] httpfile = new HttpFile[thread];//HttpFile 是一个类，此处定义类数组。
            for (int j = 0; j < thread; j++)//开始创建线程下载。
            {
                httpfile[j] = new HttpFile(this,j);
                threadk[j] = new Thread(new ThreadStart(httpfile[j].Receive));
                threadk[j].Start();
            }
            Hbfile();//合并线程接收的文件  
            dir.Delete(true);//强制删除临时文件夹
            #endregion

        }

        void Hbfile()
        {
            bool hb;
            while (true)//等待  
            {
                hb = true;
                for (int i = 0; i < thread; i++)
                {
                    if (threadw[i] == false)//有未结束线程，等待  
                    {
                        hb = false;
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (hb == true)//所有线程均已结束，停止等待，  
                {
                    break;
                }
            }
            FileStream fs;//开始合并  
            FileStream fstemp;
            int readfile;
            byte[] bytes = new byte[512];
            fs = new FileStream(FinalLocalFolder, System.IO.FileMode.OpenOrCreate);
            for (int k = 0; k < thread; k++)
            {
                fstemp = new FileStream(tmpFileNamew[k], System.IO.FileMode.Open);
                while (true)
                {
                    readfile = fstemp.Read(bytes, 0, 512);
                    if (readfile > 0)
                    {
                        fs.Write(bytes, 0, readfile);
                    }
                    else
                    {
                        break;
                    }
                }
                fstemp.Close();
            }
            fs.Close();
        }
    }
}
