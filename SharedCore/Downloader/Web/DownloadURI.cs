using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.Down.Web;

///<summary>下载URI对象。</summary>
public class DownloadURI
{
    public readonly string RemoteUri; // 远程URI
    public readonly string LocalPath; // 本地路径

    public DownloadURI(string remoteUri, string localPath)
    {
        RemoteUri = remoteUri;
        LocalPath = localPath;
    }

    public override string ToString()
    {
        return $"remote: {RemoteUri}, local: {LocalPath}";
    }
}
