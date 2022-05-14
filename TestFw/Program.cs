using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CoreLaunching;

namespace TestFw
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OTC();
        }
        /// <summary>
        /// NET4.5.2解压测试
        /// </summary>
        [Obsolete(null,true)]
        static void OTC()
        {
            string FolderName = @"I:\isos\Testing\";
            DirectoryInfo directoryInfo = new DirectoryInfo(FolderName);
            directoryInfo.Create();
            ZipFile.Export(@"C:\Users\Lenovo\Videos\Passport.zip", @"C:\Users\Lenovo\Videos\");
            Console.WriteLine();
        }
    }
}
