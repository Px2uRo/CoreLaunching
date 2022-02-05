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

    class HttpFile
    {
        #region 程序变量
        public bool[] threadw; //每个线程结束标志  
        public string[] filenamew;//每个线程接收文件的文件名  
        public int[] filestartw;//每个线程接收文件的起始位置  
        public int[] filesizew;//每个线程接收文件的大小  
        public string strurl;//接受文件的URL  
        public bool hb;//文件合并标志  
        public int thread;//进程数
        public HttpFile(int thread)//构造方法  
        {
            threadh = thread;
        }
        #endregion
        public int threadh;//线程代号  
        public string filename { get; set; }//文件名  
        public string strUrl { get; set; }//接收文件的URL  
        public FileStream fs { get; set; }
        public HttpWebRequest request { get; set; }
        public System.IO.Stream ns { get; set; }
        public byte[] nbytes { get; set; }//接收缓冲区  
        public int nreadsize { get; set; }//接收字节数  
        public string FinalSaveFilePath { get; set; }

        void receive()//接收线程  
        {
            filename = filenamew[threadh];
            ns = null;
            nbytes = new byte[512];
            nreadsize = 0;
            Console.WriteLine("线程" + threadh.ToString() + "开始接收");
            fs = new FileStream(filename, System.IO.FileMode.Create);
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                //接收的起始位置及接收的长度   
                request.AddRange(filestartw[threadh],
                filestartw[threadh] + filesizew[threadh]);
                ns = request.GetResponse().GetResponseStream();//获得接收流  
                nreadsize = ns.Read(nbytes, 0, 512);
                while (nreadsize > 0)
                {
                    fs.Write(nbytes, 0, nreadsize);
                    nreadsize = ns.Read(nbytes, 0, 512);
                    Console.WriteLine("线程" + threadh.ToString() + "正在接收");
                }
                fs.Close();
                ns.Close();
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                fs.Close();
            }
            Console.WriteLine("进程" + threadh.ToString() + "接收完毕!");
            threadw[threadh] = true;
        }

        public void Start(string url,int threadNumber)
        {
            DateTime dt = DateTime.Now;//开始接收时间  
            Console.WriteLine("开始时间={0}", dt);
            strurl = url;
            HttpWebRequest request;
            long filesize = 0;
            try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(strurl);
                    filesize = request.GetResponse().ContentLength;//取得目标文件的长度  
                    request.Abort();
                }
            catch (Exception er)
                {
                    Console.WriteLine(er.Message);
                }
            // 接收线程数  
            thread = threadNumber;
                //根据线程数初始化数组  
                threadw = new bool[thread];
                filenamew = new string[thread];
                filestartw = new int[thread];
                filesizew = new int[thread];

                //计算每个线程应该接收文件的大小  
                int filethread = (int)filesize / thread;//平均分配  
                int filethreade = filethread + (int)filesize % thread;//剩余部分由最后一个线程完成  
                                                                      //为数组赋值  
                for (int i = 0; i < thread; i++)
                {
                    threadw[i] = false;//每个线程状态的初始值为假  
                    filenamew[i] = i.ToString() + ".tmp";//每个线程接收文件的临时文件名  
                    if (i < thread - 1)
                    {
                        filestartw[i] = filethread * i;//每个线程接收文件的起始点  
                        filesizew[i] = filethread - 1;//每个线程接收文件的长度  
                    }
                    else
                    {
                        filestartw[i] = filethread * i;
                        filesizew[i] = filethreade - 1;
                    }
                }
                //定义线程数组，启动接收线程  
                Thread[] threadk = new Thread[thread];
                HttpFile[] httpfile = new HttpFile[thread];
                for (int j = 0; j < thread; j++)
                {
                    httpfile[j] = new HttpFile(j);
                    threadk[j] = new Thread(new ThreadStart(httpfile[j].receive));
                    threadk[j].Start();
                }
                //启动合并各线程接收的文件线程  
                Thread hbth = new Thread(new ThreadStart(hbfile));
                hbth.Start();
        }

        public void hbfile()
        {
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
            fs = new FileStream(FinalSaveFilePath, System.IO.FileMode.Create);
            for (int k = 0; k < thread; k++)
            {
                fstemp = new FileStream(filenamew[k], System.IO.FileMode.Open);
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
            DateTime dt = DateTime.Now;
            Console.WriteLine("接受完毕，时间为{0}",dt.ToString());//结束时间  
        }
    }

    public class HighSpeedDownloader
    {
        public void Start(string url,string localFolder,int threadsNum)
        {
            HttpFile hf = new HttpFile(threadsNum);
            hf.thread = threadsNum;
            hf.strurl = url;
            hf.FinalSaveFilePath=Path.Combine(localFolder,Path.GetFileName(hf.strurl));
            hf.Start(url, threadsNum);
        }
    }
}
