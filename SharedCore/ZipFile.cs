using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

#if NET4_0
using pkg= System.IO.Packaging;
#elif NET4_5_2 || NET6_0
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
        public static string[] GetItems(string zipFilePath)
        {
            var lst = new List<string>();
            using (var str = File.OpenRead(zipFilePath))
            {
                using (var zipf = new comp.ZipArchive(str))
                {
                    foreach (var entry in zipf.Entries)
                    {
                        lst.Add(entry.FullName);
                    }
                }
            }
            return lst.ToArray();
        }
        public static string[] GetItems(Stream stream)
        {
            var lst = new List<string>();
                using (var zipf = new comp.ZipArchive(stream))
                {
                    foreach (var entry in zipf.Entries)
                    {
                        lst.Add(entry.FullName);
                    }
            }
            return lst.ToArray();
        }
        public static byte[] GetSubFileData(string subFileName, string zipFilePath)
        {
            var fs = File.Open(zipFilePath,FileMode.Open);
            using (var zipf = new comp.ZipArchive(fs))
            {
                var itm = zipf.Entries.Where((x) => x.Name == subFileName).ToArray()[0];
                using (var stm = itm.Open())
                {
                    using (var ms = new MemoryStream())
                    {
                        stm.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }
        public static byte[] GetSubFileData(string subFileName, Stream stream)
        {
            using (var zipf = new comp.ZipArchive(stream))
            {
                var itm = zipf.Entries.Where((x) => x.Name == subFileName).ToArray()[0];
                using (var stm = itm.Open())
                {
                    using (var ms = new MemoryStream())
                    {
                        stm.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }
        public static void Export(string zipFilePath, string dirName)
        {
            Export(zipFilePath, dirName, false);
        }
        public static void Export(string zipFilePath, string DirName, bool overrite)
        {
            using (var str = File.OpenRead(zipFilePath))
            {
                using (var zipf = new comp.ZipArchive(str))
                {
                    foreach (var entry in zipf.Entries)
                    {
                        if (entry.Name.EndsWith(".dll"))
                        {
                            if(!overrite&&File.Exists(Path.Combine(DirName, entry.Name)))
                            {
                                return;
                            }
                            var path = Path.Combine(DirName, entry.Name);
                            entry.ExtractToFile(path, overrite);
                        }
                    }
                }
                str.Close();
            }
        }
#endif
    }
}
