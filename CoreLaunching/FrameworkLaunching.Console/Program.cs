using System;
using Cons = System.Console;
using cl = CoreLaunching;
using System.IO;

namespace CoreLaunching.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dir = new DirectoryInfo(@"..\.minecraft");
            dir.Create();

            string front = @"..\.minecraft\";
            string verDir = @"version\";

            cl.DirectorySystem dirS = new DirectorySystem();
            dirS.DirSys(front+verDir);

            cl.VersionMgr mgr = new VersionMgr();
            try
            {
                mgr.LoadJson(front + verDir + @"Test.json");

            }
            catch
            {

            }
            Cons.Read();
        }
    }
}
