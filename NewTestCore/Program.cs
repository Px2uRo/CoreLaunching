using CoreLaunching;

namespace NewTestCore
{
    public class Program
    {
        static void Main(string[] args)
        {
            HFTC1();
                int i = 0;
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