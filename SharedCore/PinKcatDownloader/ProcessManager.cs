using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class ProcessManager
    {
        public event EventHandler Finished;
        int stepFinished = 0;
        private int _downloadedCount = 0;
        private long _downloadedSize = 0;
        public event EventHandler<int>? DownloadedCountUpdated;
        public event EventHandler<long>? DownloadedSizeUpdated;
        IEnumerable<MCFileInfo> Files;
        private ThreadState _state;
        string _temp = string.Empty;

        public ThreadState State => _state;
        public ProcessManager(IEnumerable<MCFileInfo> files)
        {
            Files= files;
        }

        public void Start(string temp)
        {
            _state = ThreadState.Running;
            var superSmall = Files.Where((x) => x.Size < 250000).ToArray();
            var small = Files.Where((x) => x.Size <= 2500000 && x.Size >= 250000).ToArray();
            var large = Files.Where((x) => x.Size > 2500000).ToArray();
            
            
            var promss = new SuperSmallProcessManager(superSmall);
            promss.OneFinished += Proms_OneFinished;
            promss.QueueEmpty += Promss_QueueEmpty;
            promss.OneFailed += Promss_OneFailed;
            new Thread(() => promss.DownloadSingle()).Start();


            var proms = new SingleThreadProcessManager(small);
            proms.OneFinished += Proms_OneFinished;
            proms.QueueEmpty += Promss_QueueEmpty;
            proms.OneFailed+= Promss_OneFailed;
            new Thread(() => proms.DownloadSingle()).Start();


            var prom = new MutilFileDownloaManager(large);
            Directory.CreateDirectory(temp);
            prom.OnePartFinsihed += Prom_OnePartFinsihed;
            prom.OneFinished += Proms_OneFinished;
            prom.QueueEmpty += Promss_QueueEmpty;
            prom.OneFailed+= Promss_OneFailed;
            new Thread(() => prom.Download(temp)).Start();
        }

        private void Promss_OneFailed(object? sender, MCFileFailedArgs e)
        {

        }

        private void Prom_OnePartFinsihed(object? sender, long e)
        {
            _downloadedSize+= e;
            DownloadedSizeUpdated?.Invoke(sender, _downloadedSize);
        }

        private void Promss_QueueEmpty(object? sender, EventArgs e)
        {
            stepFinished++;
            if(stepFinished== 3)
            {
                _state = ThreadState.Stopped;
                new DirectoryInfo(_temp).Delete();
                Finished?.Invoke(this,e);
            }
        }

        private void Proms_OneFinished(object? sender, MCFileInfo e)
        {
            _downloadedCount++;
            
            DownloadedCountUpdated?.Invoke(sender, _downloadedCount);
            if(sender is not MutilFileDownloaManager)
            {
                _downloadedSize += e.Size;
                DownloadedSizeUpdated.Invoke(sender, _downloadedSize);
            }
        }

        public void Pause()
        {
            if(_state==ThreadState.Running)
            {
                _state = ThreadState.StopRequested;
                _state = ThreadState.Stopped;
            }
        }
    }
    public class SingleThreadProcessManager:ObservableCollection<Thread>
    {
        public MCFileInfo[] Infos { get; set; }
        protected override void InsertItem(int index, Thread item)
        {
            var added = false;
            while (!added)
            {
                try
                {
                    
                    //Console.WriteLine($"Instered {item.GetHashCode()}");
                    //Console.WriteLine(item.ThreadState);
                    if(item.ThreadState == ThreadState.Unstarted)
                    {
                        index = Count;
                        base.InsertItem(index, item);
                        item.Start();
                        added = true;
                    }
                    else
                    {
                        return;
                    }
                }
                catch
                {

                }
            }
        }
        public int MaxNum = 64;
        public event EventHandler QueueEmpty;
        public void DownloadSingle()
        {
            Queue<MCFileInfo> queue = new();
            foreach (MCFileInfo info in Infos)
            {
                queue.Enqueue(info);
            }
           while (queue.Count > 0)
           {
                while (Count <= MaxNum)
                {
                    if (queue.Count > 0)
                    {
                        var proc = FileDownloadProgress.CreateSingle(queue.Dequeue(), out var thr);
                        proc.Finished += Proc_Finished;
                        Add(thr);
                    }
                    else
                    {
                        break;
                    }
                }
                Thread.Sleep(200);
           }
           while (Count > 0)
           {

           }
            QueueEmpty?.Invoke(this, new());
        }
        public event EventHandler<MCFileInfo> OneFinished;
        public event EventHandler<MCFileFailedArgs> OneFailed;
        public long AllLengthGetted { get; set; }
        private void Proc_Finished(object? sender, Thread e)
        {
            var data = sender as FileDownloadProgress;
            AllLengthGetted += data.Info.Size;
            Remove(e);
            OneFinished.Invoke(this,data.Info);
        }
        public SingleThreadProcessManager(MCFileInfo[] infos)
        {
            Infos= infos;
        }
    }
}
