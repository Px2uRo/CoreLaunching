using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using zip = Ionic.Zip;

namespace FrameworkLaunching
{
    public class ZipFile
    {
        public static void ExtractToDirectory(string TargetZipFile,string localFolderInfo)
        {
            zip.ZipFile zipFile = new zip.ZipFile(TargetZipFile);
            zipFile.ExtractAll(localFolderInfo, zip.ExtractExistingFileAction.OverwriteSilently);
        }
    }
}
