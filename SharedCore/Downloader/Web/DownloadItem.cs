using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.Down.Web;


public class DownloadItem
{
    private DownloadFile _downloadFile;

    public DownloadItem(DownloadURI uri)
    {
        _downloadFile = new DownloadFile(uri);
    }




}
