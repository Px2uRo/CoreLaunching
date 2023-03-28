using CoreLaunching.MicrosoftAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            set { _isFinished = value;
                if (value) {
                    Finished.Invoke(this, thread);
                }
            }
        }

        public Thread thread;
        public MCFileInfo Info;

        public static FileDownloadProgress CreateSingle(MCFileInfo info,out Thread thread)
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
                            clt.DownloadFile(info.Url,info.Local);
                            res.IsFinished = true;
                        }
                        catch (Exception ex)
                        {
                            res.IsFinished = true;
                            Console.WriteLine(info.Url+ex.Message);
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
}
