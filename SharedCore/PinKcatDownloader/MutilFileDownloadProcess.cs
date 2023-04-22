using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class RequestWithRange
    {
        public bool IsOK = false;
        public string NativeUrl { get; set; }
        public string LocalTempPath { get; set; }
        public long From { get; set; }
        public long To { get; set; }
        private Thread _thread = null;

        public Thread Thread
        {
            get { 
                if(_thread == null)
                {
                    _thread=CreateThread();
                }
                return _thread; 
            }
        }
        public event EventHandler<long> OnePartFinished;
        private Thread CreateThread()
        {
            return new Thread(() => {
                try
                {
                    var request = (HttpWebRequest)HttpWebRequest.Create(NativeUrl);
                request.AddRange(From, To);
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LocalTempPath));
                        
                            using (var fstream = File.Create(LocalTempPath))
                            {
                                stream.CopyTo(fstream);
                                OnePartFinished?.Invoke(this,fstream.Length);
                            }
                        }
                    }
                    IsOK = true;
                }
                catch (Exception ex)
                {
                    IsOK = true;
                }
            });
        }

        public RequestWithRange(string nativeUrl, string localTempPath, long from, long to)
        {
            NativeUrl = nativeUrl;
            LocalTempPath = localTempPath;
            From = from;
            To = to;
        }
    }
    public class MutilFileDownloadProcess
    {
        public event EventHandler<RequestWithRange[]> WebFinished;
        public event EventHandler<MCFileInfo> CombineFinished; 
        public event EventHandler<long> OnePartFinished;
        private bool _isFinished;

        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                if (value)
                {
                    WebFinished.Invoke(this, Requsets);
                }
            }
        }
        private Thread CreateCombineThread()
        {
            return new Thread(() =>
            {
                bool CanCombine = false;
                while (!CanCombine)
                {
                    CanCombine = true;
                    for (int i = 0; i < Requsets.Length; i++)
                    {
                        if (Requsets[i].IsOK == false)
                        {
                            CanCombine = false;
                            break;
                        }
                    }
                    Thread.Sleep(2000);
                }
                WebFinished?.Invoke(this, Requsets);
                Directory.CreateDirectory(Path.GetDirectoryName(Info.Local));
                using (var finalfs = File.Create(Info.Local))
                {
                    foreach (var item in this.Requsets)
                    {
                        using (var tempFs = File.OpenRead(item.LocalTempPath))
                        {
                            tempFs.CopyTo(finalfs);
                        }
                        new FileInfo(item.LocalTempPath).Delete();
                    }
                }
                CombineFinished.Invoke(this,Info);
            });
        }
        private Thread _combineThread;

        public Thread CombineThread
        {
            get {
                if (_combineThread == null)
                {
                    _combineThread = CreateCombineThread();
                }
                return _combineThread; }
        }

        public RequestWithRange[] Requsets { get; set; }
        public MCFileInfo Info;

        public static MutilFileDownloadProcess Create(MCFileInfo info,string tempRoot)
        {
            var res = new MutilFileDownloadProcess();
            res.Info = info;
            long chushu = 2500000;
            if (info.Size > 2500000)
            {
                var tempF = Path.Combine(tempRoot,Path.GetFileNameWithoutExtension(info.Local));
                int shang = (int)info.Size / (int)chushu;
                var yushu = info.Size % chushu;
                List<RequestWithRange> requsets = new List<RequestWithRange>();
                for (int i = 0; i < shang; i++)
                {
                    var tempP = Path.Combine(tempF,$"{Path.GetFileNameWithoutExtension(info.Local)}part{i}.tmp");
                    RequestWithRange req = new(info.Url, tempP, chushu * i, chushu * i + chushu - 1);
                    requsets.Add(req);
                }
                var tempL = Path.Combine(tempF, $"{Path.GetFileNameWithoutExtension(info.Local)}part{shang}.tmp");
                requsets.Add(new(info.Url,tempL,chushu*shang,info.Size-1));
                res.Requsets = requsets.ToArray();
            }
            else
            {
                throw new ArgumentException($"{info.Size}<=2500000!");
            }
            return res;
        }
    }
}
