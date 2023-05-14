using CoreLaunching.MicrosoftAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class FileDownloadProgress
    {
        public event EventHandler<Thread> Finished;
        private bool _isFinished;

        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                if (value)
                {
                    Finished.Invoke(this, thread);
                }
            }
        }

        public Thread thread;
        public MCFileInfo Info;

        public static FileDownloadProgress CreateSingle(MCFileInfo info, out Thread thread)
        {
            var res = new FileDownloadProgress();
            res.Info = info;
            if (info.Size < 2500000)
            {
                var thread1 = new Thread(() => {
                    using (var clt = new WebClient())
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(info.Local));
                            clt.DownloadFile(info.Url, info.Local);
                            res.IsFinished = true;
                        }
                        catch (Exception ex)
                        {
                            res.IsFinished = true;
                            Console.WriteLine(info.Url + ex.Message);
                        }
                    }
                });
                thread = thread1;
                //Console.WriteLine($"Created {thread1.GetHashCode()}");
                res.thread = thread1;
            }
            else
            {
                throw new ArgumentException($"{info.Size}>2500000!");
            }
            return res;
        }
    }
    public class FileDownloadProgressWithUpdate
    {
        public event EventHandler<Thread> Finished; 
        public event EventHandler<long> DownloadedUpdated;
        public void UpdateDownloaded(long e)
        {
            DownloadedUpdated?.Invoke(this, e);
        }

        private bool _isFinished;

        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                if (value)
                {
                    Finished.Invoke(this, thread);
                }
            }
        }

        public Thread thread;
        public MCFileInfo Info;

        public static FileDownloadProgressWithUpdate CreateSingle(MCFileInfo info)
        {
            var res = new FileDownloadProgressWithUpdate();
            res.Info = info;
            if (info.Size < 2500000)
            {
                var thread1 = new Thread(() => {

                        try
                        {
                            long lastL = 0;
                            var request = (HttpWebRequest)HttpWebRequest.Create(info.Url);
                            using (var response = request.GetResponse())
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(info.Local));
                                    using (var fstream = File.Create(info.Local))
                                    {
                                        var nbytes = new byte[4096];
                                        var nreadsize = stream.Read(nbytes, 0, 4096);
                                    res.UpdateDownloaded(nreadsize);
                                    while (nreadsize > 0)
                                        {
                                            fstream.Write(nbytes, 0, nreadsize);
                                            nreadsize = stream.Read(nbytes, 0, 4096);
                                            res.UpdateDownloaded(nreadsize);
                                        }
                                        res.Finish();
                                        res.IsFinished = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            res.IsFinished = true;
                            Console.WriteLine(info.Url + ex.Message);
                        }
                    
                });
                res.thread = thread1;
            }
            else
            {
                throw new ArgumentException($"{info.Size}>2500000!");
            }
            return res;
        }

        private void Finish()
        {
            this.Finished?.Invoke(this,thread);
        }

        public void Start()
        {
            thread.Start();
        }
    }
}
