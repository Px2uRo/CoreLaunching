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
        public void DownloadSingle()
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
                    if (queue.Count > 0)
                    {
                        var proc = FileDownloadProgress.CreateSingle(queue.Dequeue(), out var thr);
                        proc.Finished += Proc_Finished;
                        InsertMe(Count-1,thr,proc.Info.Size);
                    }
                    else
                    {
                        QueueEmpty?.Invoke(this,new());
                        break;
                    }
                }
                Thread.Sleep(200);
            }
        }
        public event EventHandler<MCFileInfo> OneFinished;
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
}
