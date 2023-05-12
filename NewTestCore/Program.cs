using CoreLaunching;
using CoreLaunching.Accounts;
using CoreLaunching.Forge;
using CoreLaunching.JsonTemplates;
using CoreLaunching.MicrosoftAuth;
using CoreLaunching.ModFinder;
using CoreLaunching.PinKcatDownloader;
using Newtonsoft.Json;
using System;
using System.Net;

namespace NewTestCore
{
    public class Program
    {
        //static Downloader down = new Downloader();


        static void Main(string[] args)
        {
            //var result =  McModFinder.FromName("JEI");
            //var resultLinks = McModLinkResultItem.GetDownloadInfoFromPage("https://www.mcmod.cn/class/459.html");
            //var upa = new UnitedPassportAccount() { EmailAddress= "3245813053@qq.com",
            //    Password= "qweasdzxc.",
            //    ServerID = "9b56c301b0ca11eb9d5300163e095b49" };
            var upa = UnitedPassportAccount.GetAccountWithToken("9b56c301b0ca11eb9d5300163e095b49", "3245813053@qq.com", "qweasdzxc.");
            if(!upa.SingUp(out var errorInfo))
            {
                Console.WriteLine(errorInfo);
            }
        }

        static void OTC5()
        {
            var parser = new ForgeParser();
            var lst = parser.ParseFromInstaller(@"C:\Users\Lenovo\Downloads\forge-1.19.3-44.0.1-installer\install_profile.json", false,
                @"I:\Xiong's\MEFLCollection\MEFLAva\MEFL\bin\Debug\net7.0\.minecraft");
            var downloader = new ProcessManager(lst);
            downloader.DownloadedSizeUpdated += Downloader_DownloadedSizeUpdated;
            downloader.Start("I:\\Xiong's\\MEFLCollection\\MEFLAva\\MEFL\\bin\\Debug\\net7.0\\.minecraft\\CoreLaucnchingTmp");
            while (downloader.State != ThreadState.Stopped)
            {

            }
            var installer = new ForgeInstaller();
            installer.Output += Pr1_OutputDataReceived;
            installer.InstallClient(
                @"C:\Program Files\Microsoft\jdk-17.0.2.8-hotspot\bin\java.exe",
                "I:\\Xiong's\\MEFLCollection\\MEFLAva\\MEFL\\bin\\Debug\\net7.0\\.minecraft\\libraries",
                @"I:\Xiong's\MEFLCollection\MEFLAva\MEFL\bin\Debug\net7.0\.minecraft\versions\1.19.3_forge_44.0.1\1.19.3_forge_44.0.1.jar",
          @"I:\Xiong's\MEFLCollection\MEFLAva\MEFL\bin\Debug\net7.0\.minecraft\versions\1.19.3_forge_44.0.1\1.19.3_forge_44.0.1.json",
@"C:\Users\Lenovo\Downloads\forge-1.19.3-44.0.1-installer.jar", CoreLaunching.PinKcatDownloader.ParseType.FilePath
                );
        }

        private static void Downloader_DownloadedSizeUpdated(object? sender, long e)
        {
            Console.Clear();
            Console.WriteLine(e.ToString());
        }

        private static void Pr1_OutputDataReceived(object sender,string e)
        {
            Console.WriteLine(e);
        }

        static void OTC5(string[] args)
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
            j.FinnalCommand = j.FormatCommand("-cp ${classpath}", li);

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