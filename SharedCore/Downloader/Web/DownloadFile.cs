using System;
#if NET6_0
using System.Buffers.Text;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
#if NET6_0
using System.Text.Json;
using System.Text.Json.Serialization;
#else 
using Newtonsoft.Json;
#endif
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Threading;

namespace CoreLaunching.Down.Web
{
    ///<summary>文件下载状态。</summary>
    public enum DownloadFileState
    {
        ///<summary>文件下载任务处于闲置状态。</summary>
        Idle,

        ///<summary>文件下载任务被取消了。</summary>
        Canceled,

        ///<summary>下载中。</summary>
        Downloading,

        ///<summary>下载失败。</summary>
        DownloadFailed,

        ///<summary>下载成功。</summary>
        DownloadSucessed,

    }

    public struct ContentRange
    {
        public long Offset;
        public long Length;
    }

    ///<summary>文件下载模块，一个实例对应一个下载任务。</summary>
    [Serializable]
    public class DownloadFile
    {
#region const

        private const int MaxRange = 1024 * 1024;

#endregion

#region events

        public event EventHandler<long> OnBytesAdd;
        public event EventHandler<string> OnDownloadFailed;
        public event EventHandler OnTaskCompleted;

#endregion

#region fields

        [NonSerialized]
        private CancellationTokenSource _cancelSource;

        ///<summary>文件当前下载的长度位置，如果要实现恢复下载，可以从此处进行设置。是临时数据。</summary>
        private long _tempDownloadingLengthForPausedLength;

        ///<summary>下载缓存。</summary>
        private byte[] _buffer;

#endregion

#region props

        ///<summary>下载状态。</summary>
        public DownloadFileState State
        {
            get; private set;
        }

        ///<summary>错误信息。</summary>
        public string ErrorInfo
        {
            get; private set;
        }

        ///<summary>文件下载和本地存放的来源。</summary>
        public DownloadURI Source
        {
            get; private set;
        }

        ///<summary>文件长度。</summary>
        public long ContentLength
        {
            get; private set;
        }

        ///<summary>下载计数器。</summary>
        public int DownloadCounter
        {
            get; private set;
        }

        ///<summary>文件长度是否正确。</summary>
        [JsonIgnore]
        public bool IsFileLengthCorrect => ContentLength == _tempDownloadingLengthForPausedLength;

#endregion

#region ctors

        public DownloadFile(DownloadURI source)
        {
            _cancelSource = new CancellationTokenSource();
            _tempDownloadingLengthForPausedLength = 0;
            Source = source;
            State = DownloadFileState.Idle;
            DownloadCounter = 0;
        }
#endregion

#region methods

        public async Task Download(bool isContinue = false)
        {
            try
            {
                var httpRequest = WebRequest.Create(Source.RemoteUri);
                httpRequest.Method = "GET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";

                httpRequest.Timeout = 30000; // 半分钟。
                var httpResponse = httpRequest.GetResponse();

                ContentLength = httpResponse.ContentLength;

                if (ContentLength > MaxRange)
                {
                    await DownloadCombineParts(ContentLength);
                }
                else
                {
                    await DownloadSingle(false);
                }

                OnTaskCompleted?.Invoke(this,new EventArgs());
            }
            catch (Exception)
            {

                throw;
            }
        }

        ///<summary>下载切片。</summary>
        public async Task DownloadCombineParts(long contentLength)
        {
            await DownloadCombineParts(contentLength, MaxRange); // 1MB区域
        }

        ///<summary>下载多项。</summary>
        private async Task DownloadCombineParts(long contentLength, long partSize)
        {
            State = DownloadFileState.Downloading;
            var task = new Task(() =>
            {
            var buffer = new byte[contentLength];
            var partCount = contentLength / partSize; // 是否为整除，如果不是整除则补上余数段。
            partCount += contentLength % partSize == 0 ? 0 : 1;
            var hasRest = contentLength % partSize != 0;

            var ranges = new ContentRange[partCount];
            for (int i = 0; i < partCount; i++) { ranges[i] = new ContentRange() { Offset = i * partSize, Length = partSize }; }
            if (hasRest) ranges[partCount - 1].Length = contentLength - (partCount - 1) * partSize;

            var po = new ParallelOptions();
            po.CancellationToken = _cancelSource.Token;
            try
            {
                //foreach (var r in ranges)
                //{
                //    DownloadToBuffer(buffer, r.Offset, r.Length);
                //}
                Parallel.ForEach(ranges, po, r => DownloadToBuffer(buffer, r.Offset, r.Length));
                State = DownloadFileState.DownloadSucessed;
                OnTaskCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"下载失败{Source.RemoteUri}，{ex.Message}");
                State = DownloadFileState.DownloadFailed;
                ErrorInfo = ex.Message;
                OnDownloadFailed?.Invoke(this, ErrorInfo);
                OnTaskCompleted?.Invoke(this, EventArgs.Empty);
            }

            //Console.WriteLine(HttpHelper.GetBufferInfo(buffer, (int)ranges.Last().Offset));
                Directory.CreateDirectory(Path.GetDirectoryName(Source.LocalPath));
                using (var fs = new FileStream(Source.LocalPath, FileMode.Create))
                {
                    fs.Write(buffer,0,buffer.Length);
                }

                var nbuffer = File.ReadAllBytes(Source.LocalPath);
                //Console.WriteLine(HttpHelper.GetBufferInfo(nbuffer, (int)ranges.Last().Offset));
#if NET6_0
                Task.Delay(1000).ContinueWith((o) => { GC.Collect(); });
#else
                new Thread(() => { 
                Thread.Sleep(1000);
                    GC.Collect();
                }).Start();
#endif

            });
            task.Start();
            await task;

        }

        ///<summary>下载到缓存。</summary>
        private void DownloadToBuffer(byte[] buffer, long rangeStart, long rangeLen)
        {
            HttpHelper.GetContentByPart(buffer, Source.RemoteUri, rangeStart, rangeStart + rangeLen - 1);
        }

        ///<summary>下载任务。</summary>
        public Task DownloadSingle(bool isContinue = false)
        {
            State = DownloadFileState.Downloading;
            DownloadCounter++;
            var task = new Task(async () =>
            {
                try
                {
                    var httpRequest = WebRequest.Create(Source.RemoteUri) as HttpWebRequest;
                    httpRequest.Method = "GET";
                    httpRequest.ContentType = "application/x-www-form-urlencoded";
                    httpRequest.AddRange(0);
                    // 设置断点续传的信息。
                    if (isContinue)
                    {
                        httpRequest.Headers.Add("Range", $"{_tempDownloadingLengthForPausedLength}-");
                    }

                    httpRequest.Timeout = 30000; // 半分钟。
                    var httpResponse = httpRequest.GetResponse();

                    ContentLength = httpResponse.ContentLength;

                    using (var stream = httpResponse.GetResponseStream())
                    {
                        var parentRoot = Path.GetDirectoryName(Source.LocalPath);
                        Helpers.FanFileHelper.CreateFolder(parentRoot);
                        if (isContinue)
                        {
                            using (FileStream fs = new FileStream(Source.LocalPath, FileMode.Append))
                            {
                                fs.Seek(_tempDownloadingLengthForPausedLength, SeekOrigin.Begin);
#if NET6_0
                                await stream.CopyToAsync(fs, _cancelSource.Token);
#else
                                stream.CopyTo(fs);
#endif
                                _tempDownloadingLengthForPausedLength = fs.Length;
                            }
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(Source.LocalPath, FileMode.Create))
                            {
#if NET6_0
                                await stream.CopyToAsync(fs, _cancelSource.Token);
#else
                                stream.CopyTo(fs);
#endif
                                _tempDownloadingLengthForPausedLength = fs.Length;
                            }
                        }

                    }

                    State = DownloadFileState.DownloadSucessed;
                    OnTaskCompleted?.Invoke(this, EventArgs.Empty);


                }
                catch (TaskCanceledException cancelex)
                {
                    Debug.WriteLine($"下载取消");
                    State = DownloadFileState.Canceled;
                    OnDownloadFailed?.Invoke(this, ErrorInfo);
                    OnTaskCompleted?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {

                    Debug.WriteLine($"下载失败{Source.RemoteUri}，{ex.Message}");
                    State = DownloadFileState.DownloadFailed;
                    ErrorInfo = ex.Message;
                    OnDownloadFailed?.Invoke(this, ErrorInfo);
                    OnTaskCompleted?.Invoke(this, EventArgs.Empty);
                }

            });

            task.Start();


            return task;
        }

        public void WaitDownload()
        {
            while (State == DownloadFileState.Downloading)
            {
                Thread.Sleep(250);
            }
        }

        ///<summary>将文本内容进行序列化。</summary>
        public T ToObject<T>()
        {
            if (File.Exists(Source.LocalPath) == false)
                return default(T);

#if NET6_0
            using var fileStream = new FileStream(Source.LocalPath, FileMode.Open);
            var obj = JsonSerializer.Deserialize<T>(fileStream);
#else
            var strm = File.OpenText(Source.LocalPath);
            var obj = JsonSerializer.Deserialize<T>(new JsonTextReader(strm));
#endif
            return obj;
        }

        ///<summary>取消任务。会保存上一次的下载数据信息。</summary>
        public void Cancel()
        {
            _cancelSource.Cancel();
            State = DownloadFileState.Canceled;
        }

        ///<summary>保存文件状态。</summary>
        public void SaveTempCache(string cacheDirPath)
        {
            // 本地文件会保存成这种样子。  
            var guid = Source.LocalPath;
            guid = guid.Replace(':', '_');
            guid = guid.Replace('/', '_');
            guid = guid.Replace('\\', '_');
            guid = guid.Replace(' ', '_');
            guid = guid.Replace('.', '_');
            guid = guid.ToUpper();
            guid = guid + ".cache";

            var path = Path.Combine(cacheDirPath, guid);
            var file = new FileInfo(path);
            using var stream = file.OpenWrite();
            JsonSerializer.Serialize(stream, this);
        }

        ///<summary>载入当前文件的长度。</summary>
        private long LoadCurrentFileLength()
        {
            var file = new FileInfo(Source.LocalPath);
            var len = file.Length;
            _tempDownloadingLengthForPausedLength = len;
            return file.Length;
        }

#endregion

#region overrides
        public override string ToString()
        {
            return $"[DownloadFile] remote uri: {Source.RemoteUri}, local uri: {Source.LocalPath}, content len: {ContentLength}";
        }
#endregion

#region static

        ///<summary>读取下载信息。</summary>
        public static DownloadFile LoadTempCache(string path)
        {
            DownloadFile downloadFile = null;
            using (var stream = File.OpenRead(path))
            {
                downloadFile = JsonSerializer.Deserialize<DownloadFile>(stream);
            }
            downloadFile.LoadCurrentFileLength();
            return downloadFile;
        }

        ///<summary>读取缓存文件夹的所有数据。</summary>
        public static DownloadFile[] LoadTempCaches(string path, out string[] invalidPaths)
        {
            var validList = new List<DownloadFile>();
            var invalidList = new List<string>();
            foreach (var file in Directory.GetFiles(path, "*.cache"))
            {
                var downloadFile = LoadTempCache(file);
                if (downloadFile != null)
                    validList.Add(downloadFile);
                else
                    invalidList.Add(file);
            }
            invalidPaths = invalidList.ToArray();
            return validList.ToArray();
        }

#if DEBUG
        public static DownloadFile LoadTest()
        {
            var targetPath = Path.Combine(Environment.CurrentDirectory, "test.exe");
            return new DownloadFile(new DownloadURI("https://download.visualstudio.microsoft.com/download/pr/dcf6b6e2-824d-4cae-9f05-1b81b4ccbace/dd620dd4b95bb3534d0ebf53babc968b/dotnet-sdk-7.0.200-win-x64.exe", targetPath));
        }
#endif


#endregion

    }
}
