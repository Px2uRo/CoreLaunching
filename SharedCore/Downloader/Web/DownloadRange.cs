using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.Down.Web;
public class DownloadRange
{
    public long Start;

    public long End;

    public bool IsUseRange;

    public readonly string RemoteUri; // 远程URI
    public readonly string LocalPath; // 本地路径
}
