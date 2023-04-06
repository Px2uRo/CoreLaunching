using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.PinKcatDownloader
{
    public class MutilFileDownloaManager:ObservableCollection<MutilFileDownloadProcess>
    {
        public int MaxNum = 12;
        protected override void InsertItem(int index, MutilFileDownloadProcess item)
        {
            var added = false;
            while (!added)
            {
                try
                {
                    index = Count;
                    base.InsertItem(index, item);
                    added = true;
                    foreach (var req in item.Requsets)
                    {
                        if (req.Thread.ThreadState == ThreadState.Unstarted)
                        {
                            req.Thread.Start();
                        }
                    }
                    if(item.CombineThread.ThreadState == ThreadState.Unstarted) 
                    { 
                        item.CombineThread.Start();
                    }
                }
                catch
                {

                }
            }
        }
        public MCFileInfo[] Infos { get; set; }
        public MutilFileDownloaManager(MCFileInfo[] infos)
        {
            Infos= infos;
        }
        public event EventHandler QueueEmpty;
        public event EventHandler ManagerEmpty;
        public void Download(string tempRoot)
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
                        var proc = MutilFileDownloadProcess.Create(queue.Dequeue(),tempRoot);
                        proc.CombineFinished += Proc_Finished;
                        Add(proc);
                    }
                    else
                    {
                        QueueEmpty?.Invoke(this, new());
                        break;
                    }
                }
                Thread.Sleep(500);
            }
            while (Count > 0)
            {

            }
            ManagerEmpty?.Invoke(this, new());
        }

        public EventHandler<MCFileInfo> OneFinished;
        private void Proc_Finished(object? sender, MCFileInfo e)
        {
            var proc = sender as MutilFileDownloadProcess;
            Remove(proc);
            OneFinished.Invoke(this,e);
        }
    }
}
