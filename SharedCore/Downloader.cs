using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace CoreLaunching
{
    internal class WebDownHelper
    {
        internal long StartIndex;
        internal long Length;

    }
    public class Downloader : IDisposable
    {
        #region EVENT
        public delegate void SpeedChangedEH(object sender);
        public event SpeedChangedEH SpeedChanged;
        #endregion
        public int Downloaded { 
            get {
                var res = 0;
                for (int i = 0; i < HelperPool.Length; i++)
                {
                    res += HelperPool[i].bytesRcved;
                }
                return res;
            } }

        public System.Timers.Timer Tim = new(1000);
        #region Dispose

        private bool IsDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                IsDisposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Downloader()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
        public FileHelper[] HelperPool;
        int[] startC;
        int[] lengC;
        int ThreadNums;
        public long contentleng = 0;


        public static void DownloadFile(string Native, string Local)
        {
            var down = new Downloader();
            down.Download(Native, Local);
            down.Dispose();
        }
        public static void DownloadFile(string Native, string Local, int ThreadNum)
        {
            var down = new Downloader();
            down.Download(Native, Local, ThreadNum);
            down.Dispose();
        }

        public void Download(string Native, string Local)
        {
            var http = HttpWebRequest.Create(Native);
            using (var resp = (HttpWebResponse)http.GetResponse())
            {
                contentleng = resp.ContentLength;
            }
            GC.SuppressFinalize(http);
            var DecidedThreadNum = 1;
            if(contentleng <= 300000)
            {
                Download(Native, Local, 1);
                return;
            }
            else if(contentleng >= 15000000)
            {
                Download(Native, Local, 50);
                return;
            }
            else
            {
                DecidedThreadNum = (int)(contentleng / 300000 +1);
                Download(Native, Local, DecidedThreadNum);
                return;
            }
        }
        public void Download(string native, string Local, int ThreadNum)
        {
            #region Download
            startC = new int[ThreadNum];
            lengC = new int[ThreadNum];
            ThreadNums = ThreadNum;
            HelperPool = new FileHelper[ThreadNum];
            long startIndex = 0;
            long length = 0;
            var http = HttpWebRequest.Create(native);
            using (var resp = (HttpWebResponse)http.GetResponse())
            {
                contentleng = resp.ContentLength;
            }
            if (!File.Exists(Local))
            {
                if (!Directory.Exists(Local.Replace(Path.GetFileName(Local), "")))
                {
                    Directory.CreateDirectory(Local.Replace(Path.GetFileName(Local), ""));
                }
            }
            int Shang = (int)(contentleng / ThreadNum);
            int Yushu = (int)contentleng - (Shang * ThreadNum);
            for (int i = 0; i < HelperPool.Length; i++)
            {
                if (i < ThreadNum - 1)
                {
                    startIndex = Shang * i;
                    length = Shang;
                }
                else
                {
                    startIndex = Shang * i;
                    length = Shang + Yushu;
                }
                startC[i] = (int)startIndex;
                lengC[i] = (int)length;
                HelperPool[i] = new();
            }
            for (int i = 0; i < HelperPool.Length; i++)
            {
                if (i < HelperPool.Length)
                {
                    HelperPool[i].Start(native, Local, startC[i], lengC[i]);
                    HelperPool[i].Dispose();
                }
            }
            Tim.AutoReset = true;
            Tim.Elapsed += Tim_Elapsed;
            this.Tim.Start();
            #endregion
            #region Combine
            var fc = new FileCombiner();
            fc.Combine(Local, HelperPool);
            while(fc.working)
            {
                Thread.Sleep(1000);
            }
            fc.Dispose();
            #endregion
        }

        private void Tim_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            SpeedChanged.Invoke(this);
        }
    }
    #region HelperClasses
    public class FileHelper : IDisposable
    {
        public bool pause = false;
        public bool IsDone = false;
        private byte[] nbytes = new byte[512];
        private string _tempFilePath;
        public int bytesRcved;
        public string TempFilePath => _tempFilePath;


        internal void Start(string native,string local, long startIndex, long length)
        {
            Console.WriteLine(startIndex);
            new Thread(() =>
            {
                var baseRequest = (HttpWebRequest)HttpWebRequest.Create(native);
                baseRequest.Method = "GET";
                baseRequest.AddRange(startIndex, startIndex + length -1);

                var localp = local.Replace(Path.GetFileName(local), $"{Path.GetFileName(local)}-Temps\\{Path.GetFileName(local)}-{startIndex}.tmp");
                if (!Directory.Exists(Path.GetDirectoryName(localp)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localp));
                }
                _tempFilePath = localp;
                Thread.Sleep(100);
                if (File.Exists(localp))
                {
                    File.Delete(localp);
                }
                using (var fs = File.Create(localp))
                {
                    using (var resp = baseRequest.GetResponse())
                    {
                        using (var data = resp.GetResponseStream())
                        {
                            var nreadsize = data.Read(nbytes, 0, 512);
                            while (nreadsize > 0)
                            {
                                while (pause)
                                {
                                    Thread.Sleep(100);
                                }
                                if (!pause)
                                {
                                    fs.Write(nbytes, 0, nreadsize);
                                    bytesRcved += 512;
                                    nreadsize = data.Read(nbytes, 0, 512);
                                }
                            }
                            bytesRcved = (int)length;
                            data.Close();
                            data.Dispose();
                        }
                        resp.Close();
                        resp.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
                IsDone = true;
            }).Start();
        }

        #region Dispose
        public bool IsDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                IsDisposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FileHelper()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal class FileCombiner : IDisposable
    {
        #region Dispose
        private bool disposedValue;
        internal bool working = true;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FileCombiner()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        internal void Combine(string local, FileHelper[] helperPool)
        {
            new Thread(() =>
            {
                bool hb;
                while (true)//等待  
                {
                    hb = true;
                    for (int i = 0; i < helperPool.Length; i++)
                    {
                        if (helperPool[i].IsDone == false)//有未结束线程，等待  
                        {
                            hb = false;
                            Thread.Sleep(500);
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
                fs = new FileStream(local, System.IO.FileMode.OpenOrCreate);
                for (int k = 0; k < helperPool.Length; k++)
                {
                    fstemp = new FileStream(helperPool[k].TempFilePath, System.IO.FileMode.Open);
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
                var folderpath = helperPool[0].TempFilePath.Replace(Path.GetFileName(helperPool[0].TempFilePath), string.Empty);
                new DirectoryInfo(folderpath).Delete(true);
                working = false;
            }).Start();
        }
    }

    //public class DownloaderArgs : ProgressChangedEventArgs,IDisposable
    //{
    //    private long _BytesReceived;
    //    private long _TotalBytesToReceive;
    //    private int _ProgressPercentage;
    //    private bool disposedValue;

    //    public long BytesReceived => _BytesReceived;
    //    public long TotalBytesToReceive => _TotalBytesToReceive;
    //    public new long ProgressPercentage => _ProgressPercentage;

    //    public DownloaderArgs(long bytesReceived, long totalBytesToReceive, int progressPercentage, object? userState) : base(progressPercentage, userState)
    //    {
    //        _BytesReceived = bytesReceived;
    //        _TotalBytesToReceive = totalBytesToReceive;
    //        _ProgressPercentage = progressPercentage;
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                _BytesReceived = 0;
    //                _TotalBytesToReceive = 0;
    //                _ProgressPercentage = 0;
    //                GC.SuppressFinalize(_BytesReceived);
    //                GC.SuppressFinalize(_TotalBytesToReceive);
    //                GC.SuppressFinalize(_ProgressPercentage);
    //            }

    //            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
    //            // TODO: 将大型字段设置为 null
    //            disposedValue = true;
    //        }
    //    }

    //    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    //    // ~DownloaderArgs()
    //    // {
    //    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //    //     Dispose(disposing: false);
    //    // }

    //    public void Dispose()
    //    {
    //        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //        Dispose(disposing: true);
    //        GC.SuppressFinalize(this);
    //    }
    //}
    #endregion


}
