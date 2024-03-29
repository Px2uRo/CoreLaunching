﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using zip = Ionic.Zip;

namespace CoreLaunching
{
    public class ZipFile
    {
        public static void ExtractToDirectory(string TargetZipFile,string localFolderInfo,bool OverWrite)
        {
            zip.ZipFile zipFile = new zip.ZipFile(TargetZipFile);
            if (OverWrite == true)
            {
                zipFile.ExtractAll(localFolderInfo, zip.ExtractExistingFileAction.OverwriteSilently);
            }
            else if(OverWrite == false)
            {
                zipFile.ExtractAll(localFolderInfo, zip.ExtractExistingFileAction.DoNotOverwrite);
            }
        }
    }
}
