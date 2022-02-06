using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLaunching.ObjectTemples;
using System.IO;
using System.Net;
using System.Threading;

namespace CoreLaunching
{
    public class MultiThreadDownloader
    {
        #region Arguments
        public string FinalLocalFolder;
        /// <summary>
        /// 网络文件地址
        /// </summary>
        public string webUrl;
        /// 线程数
        /// </summary>
        public int thread;
        #endregion

        #region 数组变量
        /// <summary>
        /// //线程结束布尔值数组
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
            #region 此类局部变量
            public MultiThreadDownloader multi;
            public int threadindex;//线程代号  
            public string filename;//文件名  
            public string strUrl;//接收文件的URL  
            public FileStream fs;
            public HttpWebRequest request;
            public System.IO.Stream ns;
            public byte[] nbytes;//接收缓冲区  
            public int nreadsize;//接收字节数  
            #endregion
            public HttpFile(MultiThreadDownloader multiarg,int threadsNum)
            {
                threadindex= threadsNum;
                multi = multiarg;
            }

            public void receive()//接收线程  
            {
                filename = multi.tmpFileNamew[threadindex];
                strUrl = multi.webUrl;
                ns = null;
                nbytes = new byte[512];
                nreadsize = 0;
                Console.WriteLine("线程" + threadindex.ToString() + "开始接收");
                fs = new FileStream(filename, FileMode.OpenOrCreate);
                bool pritfed = false;
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                    //接收的起始位置及接收的长度   
                    request.AddRange(multi.tmpFileSizeStartw[threadindex],
                    multi.tmpFileSizeStartw[threadindex] + multi.tmpFileSizew[threadindex]);
                    ns = request.GetResponse().GetResponseStream();//获得接收流  
                    nreadsize = ns.Read(nbytes, 0, 512);
                    while (nreadsize > 0)
                    {
                        fs.Write(nbytes, 0, nreadsize);
                        nreadsize = ns.Read(nbytes, 0, 512);
                        if (pritfed == false)
                        {
                            Console.WriteLine("线程" + threadindex.ToString() + "正在接收");
                            pritfed = true;
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
                multi.threadw[threadindex] = true;
            }
        }
        public void GoGoGo(string url, int threadNum, string localFolder)
        {
            localFolder = Path.Combine(localFolder, Path.GetFileName(url));
            FinalLocalFolder = localFolder;
            DateTime dt = DateTime.Now;//定义开始接收时间
            Console.WriteLine("开始时间={0}", dt);//输出信息
            webUrl = url;
            HttpWebRequest request;
            long filesize = 0;
            DirectoryInfo dir = new DirectoryInfo(localFolder + "tmp");
            dir.Create();

            #region 尝试对web进行请求。
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(webUrl);
                filesize = request.GetResponse().ContentLength;//取得目标文件的长度
                Console.WriteLine("文件大小：{0} kb", filesize);
                request.Abort();
            }
            catch (Exception er)
            {
                Console.WriteLine("发生错误，详情:\n{0}", er.Message);
            }

            #endregion

            // 确认线程数
            thread = threadNum;
            threadw = new bool[thread];
            tmpFileNamew = new string[thread];
            tmpFileSizeStartw = new int[thread];
            tmpFileSizew = new int[thread];

            #region 计算每个线程应该接收文件的大小
            int filethread = (int)filesize / thread;//平均分配  
            int filethreade = filethread + (int)filesize % thread;//剩余部分由最后一个线程完成  
            #region 为数组赋值
            for (int i = 0; i < thread; i++)
            {
                threadw[i] = false;//每个线程状态的初始值为假  
                tmpFileNamew[i] = Path.Combine(localFolder+"tmp", i.ToString() + ".tmp");//每个线程接收文件的临时文件名  
                if (i < thread - 1)
                {
                    tmpFileSizeStartw[i] = filethread * i;//每个线程接收文件的起始点  
                    tmpFileSizew[i] = filethread - 1;//每个线程接收文件的长度  
                }
                else
                {
                    tmpFileSizeStartw[i] = filethread * i;
                    tmpFileSizew[i] = filethreade - 1;
                }
            }
            #endregion
            #endregion

            #region 开始多线程
            Thread[] threadk = new Thread[thread];
            HttpFile[] httpfile = new HttpFile[thread];
            for (int j = 0; j < thread; j++)
            {
                httpfile[j] = new HttpFile(this,j);
                threadk[j] = new Thread(new ThreadStart(httpfile[j].receive));
                threadk[j].Start();
            }
            //启动合并各线程接收的文件线程  
            hbfile();
            dir.Delete(true);
            #endregion

        }

        void hbfile()
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
            fs = new FileStream(FinalLocalFolder, System.IO.FileMode.Create);
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
            Console.WriteLine("结束时间：{0}", DateTime.Now.ToString());
            Console.WriteLine("下载完成!!!");
        }
    }
}
