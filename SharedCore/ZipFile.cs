using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if NET4_0
using pkg= System.IO.Packaging;
#elif NET4_5_2||NET6_0
using comp = System.IO.Compression;
#endif
namespace CoreLaunching
{
    public class ZipFile
    {
#if NET4_0
        public static void Export(string ZipFilePath, string DirName)
        {
            
        }
        public static void Export(string ZipFilePath, string DirName,bool OverWrite)
        {
            
        }
#elif NET4_5_2
        public static void Export(string ZipFilePath, string DirName)
        {
            Export(ZipFilePath, DirName, false);
        }
        public static void Export(string ZipFilePath, string DirName,bool OverWirte)
        {
            var OpenStream = File.OpenRead(ZipFilePath);
            comp.ZipArchive zip = new comp.ZipArchive(OpenStream);
            foreach (var entry in zip.Entries)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(DirName, entry.FullName)));
                var ZipItemStream = File.Create(Path.Combine(DirName, entry.FullName));
                if (OverWirte == true&&File.Exists(Path.Combine(DirName, entry.FullName)))
                {
                    new FileInfo(Path.Combine(DirName, entry.FullName)).Delete();
                }
                var ZipEntryStream = entry.Open();
                ZipEntryStream.CopyTo(ZipItemStream);
                ZipEntryStream.Close();
                ZipItemStream.Close();
                ZipEntryStream = null;
                ZipEntryStream = null;
            }
        }
#elif NET6_0
        public static void Export(string ZipFilePath, string DirName)
        {
            comp.ZipFile.ExtractToDirectory(ZipFilePath, DirName);
        }
        public static void Export(string ZipFilePath, string DirName, bool OverWrite)
        {
            comp.ZipFile.ExtractToDirectory(DirName, ZipFilePath,OverWrite);
        }
#endif
    }
}
