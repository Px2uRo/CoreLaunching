using CoreLaunching;
using CoreLaunching.Down.Helpers;
using CoreLaunching.Down.Web;

namespace NewTestCore
{
    public class Program
    {
        //static Downloader down = new Downloader();
        
        static  void Main(string[] args)
        {
            //List<DownloadURI> urls = new List<DownloadURI>();
            //urls.Add(new("https://bmclapi2.bangbang93.com/version/1.19.2/client", "I:\\WhatTheHell\\clt.jar"));
            //new DownloadQueue(urls.ToArray()).Download();
            var df = new DownloadFile(new("https://bmclapi2.bangbang93.com/version/1.19.2/client", "I:\\WhatTheHell\\clt.jar"));
            df.Download();
            df.WaitDownload();
            Console.WriteLine("三天之内杀了你");
            Console.ReadLine();
        }

        private static void Downloadfile_OnTaskCompleted(object? sender, EventArgs e)
        {
            var file = sender as DownloadFile;
            FanFileHelper.StartProcessAndSelectFile(file.Source.LocalPath);

        }

        static void OTC4(string[] args)
        {
            new Thread(() =>
            {
                //down.Download("https://download.visualstudio.microsoft.com/download/pr/35660869-0942-4c5d-8692-6e0d4040137a/4921a36b578d8358dac4c27598519832/dotnet-sdk-7.0.101-win-x64.exe", "I:\\NET70.exe", 32);
            }).Start();
            System.Timers.Timer t = new(1000);
            t.Elapsed += Timer_Elapsed;
            t.Start();
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Console.Clear();
            //Console.WriteLine(down.contentleng);
            //Console.WriteLine(down.Downloaded);
        }
        static void OTC1()
        {
            string FolderName = @"I:\isos\Testing\";
            DirectoryInfo directoryInfo = new DirectoryInfo(FolderName);
            directoryInfo.Create();
            ZipFile.Export(@"I:\isos\android-x86_64-9.0-r2.zip", @"I:\isos\Testing");
            Console.WriteLine();
        }
        static void HFTC1()
        {
            Launcher l = new Launcher();
            GameInfo g = new GameInfo();
            JVMInfo j = new JVMInfo();
            LauncherInfo li = new LauncherInfo();
            li.Version = new Version(0, 1);
            li.Name = "离职";
            j.JavaPath = "Java";
            j.ClassPath = "665336";
            g.GameVersion = "1sdahuidashui";
            g.GameDir = "233";


            g.FinalCommand = g.FormatGameArgs("gameDir ${game_directory}");
            j.FinnalCommand = j.FormatCommand("-666 ${classpath}", li);

            l.GameInfo = g;
            l.JVMInfo = j;
            l.LauncherInfo = li;

            l.JsonPath = @"D:\Gaming\Minecraft\My_Minecraft\.minecraft\versions\1.17.1\1.17.1.json";
            l.LibrariesFolderPath = @"D:\Gaming\Minecraft\My_Minecraft\.minecraft\libraries";
            l.GameJarPath = @"D:\Gaming\Minecraft\My_Minecraft\.minecraft\versions\1.17.1\1.17.1.jar";
            l.AutoParseJson();
        }
    }
}