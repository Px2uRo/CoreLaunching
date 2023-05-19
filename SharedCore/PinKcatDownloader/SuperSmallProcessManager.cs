using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class SuperSmallProcessManager : ObservableCollection<Thread>
    {
        public MCFileInfo[] Infos { get; set; }
        private void InsertMe(int index, Thread item,long size)
        {
            var added = false;
            while (!added)
            {
                try
                {

                    //Console.WriteLine($"Instered {item.GetHashCode()}");
                    //Console.WriteLine(item.ThreadState);
                    if (item.ThreadState == ThreadState.Unstarted)
                    {
                        index = Count;
                        base.InsertItem(index, item);
                        item.Start();
                        added = true;
                        RunningBytes += size;
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
        public long MaxBytes = 2500000;
        private long RunningBytes = 0;
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
                while (RunningBytes < MaxBytes)
                {
                    if (queue.Count >= 32)
                    {
                        var proc = FileDownloadProgress.CreateSingle(queue.Dequeue(), out var thr);
                        proc.Finished += Proc_Finished;
                        InsertMe(Count-1,thr,proc.Info.Size);
                        //Thread.Sleep(200);
                    }
                    else if (queue.Count < 32&&queue.Count>0)
                    {
                        var qu = queue.Dequeue();
                        if (waiting)
                        {
                            var proc = FileDownloadProgress.CreateSingle(qu, out var thr);
                            proc.Finished += Proc_Finished;
                            InsertMe(Count - 1, thr, proc.Info.Size);
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
                while (Count > 0)
                {
                    Thread.Sleep(100);
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
            Remove(e);
            RunningBytes -= data.Info.Size;
            OneFinished.Invoke(this, data.Info);
        }
        public SuperSmallProcessManager(MCFileInfo[] infos)
        {
            Infos = infos;
        }
    }

    public class MCFileFailedArgs:EventArgs
    {
        private MCFileInfo _info;

        public MCFileInfo Info => _info;
        private Exception _ex;

        public Exception Exception => _ex;

        public MCFileFailedArgs(MCFileInfo info,Exception ex)
        {
            _info=info;
            _ex=ex;
        }
    }
}
