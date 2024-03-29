﻿using System;
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
        public long DownloadedSize =>_downloadedSize;
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

        public void Start(string temp,bool waiting = false)
        {
            _temp = temp;
            _state = ThreadState.Running;
            var superSmall = Files.Where((x) => x.Size < 250000).ToArray();
            var small = Files.Where((x) => x.Size <= 2500000 && x.Size >= 250000).ToArray();
            var large = Files.Where((x) => x.Size > 2500000).ToArray();

            if (superSmall.Length > 0)
            {
                var promss = new SuperSmallProcessManager(superSmall);
                promss.OneFinished += Proms_OneFinished;
                promss.QueueEmpty += Promss_QueueEmpty;
                promss.OneFailed += Promss_OneFailed;
                new Thread(() => promss.DownloadSingle(waiting)).Start();
            }
            else
            {
                stepFinished++;
            }

            if(small.Length > 0)
            {
                var proms = new SingleThreadProcessManager(small);
                proms.OneFinished += Proms_OneFinished;
                proms.QueueEmpty += Promss_QueueEmpty;
                proms.OneFailed += Promss_OneFailed;
                new Thread(() => proms.DownloadSingle(waiting)).Start();
            }
            else
            {
                stepFinished++;
            }

            if(large.Length > 0)
            {
                var prom = new MutilFileDownloaManager(large);
                Directory.CreateDirectory(temp);
                prom.OneFinished += Prom_OnePartFinsihed;
                prom.QueueEmpty += Promss_QueueEmpty;
                prom.OneFailed += Promss_OneFailed;
                new Thread(() => prom.Download(temp, waiting)).Start();
            }
            else
            {
                stepFinished++;
            }
        }
        public List<MCFileInfo> Remains = new();
        private void Prom_OnePartFinsihed(object? sender, MCFileInfo e)
        {
            _downloadedCount++;
            DownloadedCountUpdated?.Invoke(sender, _downloadedCount);
            if (sender is MutilFileDownloaManager)
            {
                _downloadedSize += e.Size;
                DownloadedSizeUpdated?.Invoke(sender, _downloadedSize);
            }
        }

        private void Promss_OneFailed(object? sender, MCFileFailedArgs e)
        {

        }

        private void Promss_QueueEmpty(object? sender, EventArgs e)
        {
            stepFinished++;
            if (sender is IDownloadRemain rm)
            {
                Remains.AddRange(rm.GetRemain());
            }
            if (stepFinished== 3)
            {
                _state = ThreadState.Stopped;
                //new DirectoryInfo(_temp).Delete(true);
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
                DownloadedSizeUpdated?.Invoke(sender, _downloadedSize);
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
    public class SingleThreadProcessManager:ObservableCollection<FileDownloadProgress>,IDownloadRemain
    {
        public MCFileInfo[] Infos { get; set; }
        protected override void InsertItem(int index, FileDownloadProgress item)
        {
            var added = false;
            while (!added)
            {
                try
                {
                    
                    //Console.WriteLine($"Instered {item.GetHashCode()}");
                    //Console.WriteLine(item.ThreadState);
                    if(item.thread.ThreadState == ThreadState.Unstarted)
                    {
                        index = Count;
                        base.InsertItem(index, item);
                        item.thread.Start();
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
        public void DownloadSingle(bool waiting = false)
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
                    if (queue.Count >= 8)
                    {
                        var proc = FileDownloadProgress.CreateSingle(queue.Dequeue(), out var thr);
                        proc.Finished += Proc_Finished;
                        Add(proc);
                    }
                    else if(queue.Count<8&&queue.Count>0)
                    {
                        var qu = queue.Dequeue();
                        if (waiting)
                        {
                            var proc = FileDownloadProgress.CreateSingle(qu, out var thr);
                            proc.Finished += Proc_Finished;
                            Add(proc);
                        }
                        else
                        {
                            _remain.Add(qu);
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                Thread.Sleep(200);
           }
            if (waiting)
            {
                while (Count>0)
                {
                    Thread.Sleep(100);
                }
            }
            else
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] !=null)
                    {

                    }
                    this[i].BreakNow = true;
                    _remain.Add(this[i].Info);
                }
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
            Remove(data);
            OneFinished.Invoke(this,data.Info);
        }

        List<MCFileInfo> _remain = new List<MCFileInfo>();
        public IEnumerable<MCFileInfo> GetRemain()
        {
            return _remain;
        }

        public SingleThreadProcessManager(MCFileInfo[] infos)
        {
            Infos= infos;
        }
    }
}
