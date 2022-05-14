using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace CoreLaunching
{
    public class ThreadDownloader
    {
        #region 这一部分是存信息的
        int[] TmpFileRangeStart;
        public int[] TmpFileSize { get; set; }
        string[] TmpFileNames;
        bool[] IsEveryOneOK;
        HttpWebRequest request;
        System.IO.FileStream stream;
        public string TargetUrl;
        int threadNum;
        public long DownloadedLeng { get; set; }
        public bool IsDone;
        public long ContentLen { get; set; }
        public string localName;
        public long[] EveryPartReceived { get; set; }
        public long BitPerSec { get; set; }
        public string tmpFolderPath;
        public ThreadDownloader(string url, int ThreadNum, string LocalName,string TmpFolderPath)
        {
            LocalPath.TmpFolderPath = TmpFolderPath;
            tmpFolderPath = TmpFolderPath;
            localName = LocalName;
            threadNum = ThreadNum;
            TargetUrl = url;
            TmpFileNames = new string[threadNum];
            TmpFileRangeStart = new int[threadNum];
            TmpFileSize = new int[threadNum];
            IsEveryOneOK = new bool[threadNum];
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                ContentLen = request.GetResponse().ContentLength;
            }
            catch (Exception ex)
            {

            }
            #region 计算每个线程应该接收文件的大小
            int Shang = (int)ContentLen / threadNum;//商
            int Yushu = Shang + (int)ContentLen % threadNum;//余数
            #region 为数组赋值
            for (int i = 0; i < threadNum; i++)
            {
                IsEveryOneOK[i] = false;//每个线程状态的初始值为假，方便最后合并时验证线程是否接受完毕
                TmpFileNames[i] = Path.Combine(TmpFolderPath, i.ToString() + ".tmp");
                if (i < threadNum - 1)
                {
                    TmpFileRangeStart[i] = Shang * i;//每个线程接收文件的起始点  
                    TmpFileSize[i] = Shang - 1;//每个线程接收文件的长度，减一是因为不把后面起点位置占用。
                }
                else
                {
                    TmpFileRangeStart[i] = Shang * i;
                    TmpFileSize[i] = Yushu - 1;
                }
            }
            #endregion
            #endregion
        }

        public ThreadDownloader(string url, int ThreadNum, string LocalName)
        {
            if (LocalPath.TmpFolderPath!=null)
            {
                tmpFolderPath = LocalPath.TmpFolderPath;
                localName = LocalName;
                threadNum = ThreadNum;
                TargetUrl = url;
                TmpFileNames = new string[threadNum];
                TmpFileRangeStart = new int[threadNum];
                TmpFileSize = new int[threadNum];
                IsEveryOneOK = new bool[threadNum];
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    ContentLen = request.GetResponse().ContentLength;
                }
                catch (Exception ex)
                {

                }
                #region 计算每个线程应该接收文件的大小
                int Shang = (int)ContentLen / threadNum;//商
                int Yushu = Shang + (int)ContentLen % threadNum;//余数
                #region 为数组赋值
                for (int i = 0; i < threadNum; i++)
                {
                    IsEveryOneOK[i] = false;//每个线程状态的初始值为假，方便最后合并时验证线程是否接受完毕
                    if (LocalPath.TmpFolderPath == null)
                    {
                        throw new NullReferenceException();
                    }
                    TmpFileNames[i] = Path.Combine(LocalPath.TmpFolderPath, i.ToString() + ".tmp");
                    if (i < threadNum - 1)
                    {
                        TmpFileRangeStart[i] = Shang * i;//每个线程接收文件的起始点  
                        TmpFileSize[i] = Shang - 1;//每个线程接收文件的长度，减一是因为不把后面起点位置占用。
                    }
                    else
                    {
                        TmpFileRangeStart[i] = Shang * i;
                        TmpFileSize[i] = Yushu - 1;
                    }
                }
                #endregion
                #endregion
            }
            else
            {
                throw new System.ArgumentNullException();
            }
        }
        public void Download()
        {
            new DirectoryInfo(LocalPath.TmpFolderPath).Create();
            IsDone = false;
            Thread[] threadk = new Thread[threadNum];//定义线程数组。
            ReciveOnePart[] httpfile = new ReciveOnePart[threadNum];//HttpFile 是一个类，此处定义类数组。
            for (int j = 0; j < threadNum; j++)//开始创建线程下载。
            {
                EveryPartReceived = new long[threadNum];
                httpfile[j] = new ReciveOnePart(this, j);
                threadk[j] = new Thread(new ThreadStart(httpfile[j].Receive));
                threadk[j].Start();
            }
            System.Timers.Timer t = new System.Timers.Timer(1000);
            long OldLen = 0;
            t.Elapsed += new System.Timers.ElapsedEventHandler(thevoid);
            void thevoid(object sender, System.Timers.ElapsedEventArgs e)
            {
                BitPerSec = DownloadedLeng - OldLen;
                OldLen = DownloadedLeng;
            }
            t.AutoReset = true;
            t.Start();
            Thread ComeAlong = new Thread(new ThreadStart(new ComeAlongWithEachOther(this).Hbfile));
            ComeAlong.Start();
        }
        #endregion
        public class ComeAlongWithEachOther
        {
            ThreadDownloader args;
            public ComeAlongWithEachOther(ThreadDownloader Args)
            {
                args = Args;
            }
            public void Hbfile()
            {
                bool hb;
                while (true)//等待  
                {
                    hb = true;
                    long TmpDownloaded = 0;
                    for (int j = 0; j < args.threadNum; j++)
                    {
                        TmpDownloaded += args.EveryPartReceived[j];
                    }
                    args.DownloadedLeng = TmpDownloaded;
                    TmpDownloaded = 0;
                    for (int i = 0; i < args.threadNum; i++)
                    {
                        if (args.IsEveryOneOK[i] == false)//有未结束线程，等待  
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
                byte[] bytes = new byte[2048];
                if (File.Exists(args.localName) != true)
                {
                    new DirectoryInfo(Path.GetDirectoryName( args.localName)).Create();
                    new FileInfo(args.localName).Create().Close();
                }
                while (true)
                {
                    try
                    {
                        fs = new FileStream(args.localName, System.IO.FileMode.OpenOrCreate);
                        break;  
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1000);
                    }
                }
                for (int k = 0; k < args.threadNum; k++)
                {
                    fstemp = new FileStream(args.TmpFileNames[k], System.IO.FileMode.Open);
                    while (true)
                    {
                        readfile = fstemp.Read(bytes, 0, 2048);
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
                args.IsDone = true;
                new DirectoryInfo(LocalPath.TmpFolderPath).Delete(true);
            }
        }

        class ReciveOnePart
        {
            Stream networkStream;
            FileStream fileStream;
            ThreadDownloader args;
            int TheIndex;
            public byte[] nbytes;//接收缓冲区  
            public int nreadsize;//接收字节数  
            public ReciveOnePart(ThreadDownloader args,int Index)
            {
                this.args = args;
                TheIndex = Index;
            }
            public void Receive()
            {
                networkStream = null;
                fileStream = new FileStream(args.TmpFileNames[TheIndex], FileMode.OpenOrCreate);
                nreadsize = 0;
                nbytes = new byte[2048];
                try
                {
                    var FromTheStart = args.TmpFileRangeStart[TheIndex];
                    var ThisPart = args.TmpFileSize[TheIndex];
                    var req = (HttpWebRequest)HttpWebRequest.Create(args.TargetUrl);
                    req.AddRange(FromTheStart,FromTheStart+ThisPart );
                    networkStream = req.GetResponse().GetResponseStream();  
                    nreadsize = networkStream.Read(nbytes, 0, 2048);
                    FileInfo downloadedTmpFile;
                    while (nreadsize > 0)
                    {
                        downloadedTmpFile = new FileInfo(args.TmpFileNames[TheIndex]);
                        fileStream.Write(nbytes, 0, nreadsize);
                        args.EveryPartReceived[TheIndex] = downloadedTmpFile.Length;
                        nreadsize = networkStream.Read(nbytes, 0, 2048);
                        downloadedTmpFile = null;
                    }
                    fileStream.Close();
                    networkStream.Close();
                    req = null;
                    args.IsEveryOneOK[TheIndex] = true;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }

    public static class LocalPath
    { 
        public static string TmpFolderPath { get; set; }
    }
}