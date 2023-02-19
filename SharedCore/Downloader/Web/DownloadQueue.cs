using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET4_5_2 || NET4_0
using System.Threading;
#endif

namespace CoreLaunching.Down.Web;

///<summary>下载队列。</summary>
public class DownloadQueue
{
    private Semaphore _semaphore = new(16, 64);
    // TODO 准备列表。
    public Queue<DownloadFile> ReadyList = new();
    // TODO 失败列表。
    public Queue<DownloadFile> FailedList = new();
    // TODO 完成列表。
    public Queue<DownloadFile> SuccessList = new();
#region ctor
    public DownloadQueue(string cachePath) : this(DownloadFile.LoadTempCaches(cachePath, out var inv))
    {

    }

    public DownloadQueue(IEnumerable<DownloadURI> urls) : this(GetDownloadFiles(urls))
    {

    }

    public DownloadQueue(DownloadFile[] files)
    {
        foreach (var file in files)
        {
            file.OnTaskCompleted += File_OnTaskCompleted;
            if (file.State == DownloadFileState.Idle || file.State == DownloadFileState.Canceled)
            {
                ReadyList.Enqueue(file);
            }
            else if (file.State == DownloadFileState.Downloading)
            {
                ReadyList.Enqueue(file);
            }
            else if (file.State == DownloadFileState.DownloadFailed)
            {
                FailedList.Enqueue(file);
            }
            else
            {
                SuccessList.Enqueue(file);
            }
        }
    }


#endregion
#region methods

    private void File_OnTaskCompleted(object? sender, EventArgs e)
    {
        var temp = sender as DownloadFile;
        if (temp.State == DownloadFileState.DownloadSucessed)
        {
            SuccessList.Enqueue(temp);
        }
        else
        {
            FailedList.Enqueue(temp);
        }
        _semaphore.Release(1);
    }

    private static DownloadFile[] GetDownloadFiles(IEnumerable<DownloadURI> urls)
    {
        List<DownloadFile> res = new();
        foreach (var url in urls)
        {
            res.Add(new DownloadFile(url));
        }
        return res.ToArray();   
    }
    public void Download()
    {
        while (ReadyList.Count!=0)
        {
            _semaphore.WaitOne();
            var file = ReadyList.Dequeue();
            file.Download();
        }
    }
#endregion
}
