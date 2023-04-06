using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
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
        public event EventHandler ManagerEmpty;
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
                        QueueEmpty?.Invoke(this, new());
                        break;
                    }
                }
                Thread.Sleep(200);
           }
           while (Count > 0)
           {

           }
           ManagerEmpty?.Invoke(this, new());
        }
        public event EventHandler<MCFileInfo> OneFinished;
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
