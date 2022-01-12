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
            cl.Launcher launcher = new cl.Launcher();
            cl.Launcher.JavaPath = @"""C:\Program Files\Java\jre1.8.0_291\bin\java.exe""";
            launcher.SetMemory(256, 1536);
            launcher.SetLauncherInfo("CoreLaunching", "0.1");
            cl.Launcher.OtherArguments = "-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";
            cl.Launcher.PlayerName = "FEN114514";
            cl.Launcher.GameVersion = "1.17.10";
            cl.Launcher.GameDir = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\versions\1.7.10\";
            cl.Launcher.assetIndex = "1.7";
            cl.Launcher.uuid = "17d03005e4ae445689edf74b53bb386b";
            cl.Launcher.accessToken = @"eyJhbGciOiJIUzI1NiJ9.eyJ4dWlkIjoiMjUzNTQ2NjEwNTMzOTE4OSIsImFnZyI6IkFkdWx0Iiwic3ViIjoiZWEwY2RkNDMtN2U1Yy00MzFkLWIxZTMtZTJkMDRiOGRhNzVmIiwibmJmIjoxNjQxOTE5NjY5LCJhdXRoIjoiWEJPWCIsInJvbGVzIjpbXSwiaXNzIjoiYXV0aGVudGljYXRpb24iLCJleHAiOjE2NDIwMDYwNjksImlhdCI6MTY0MTkxOTY2OSwicGxhdGZvcm0iOiJVTktOT1dOIiwieXVpZCI6IjUyNzczMjEzNzAyY2VkZDc3MGMzMzRlYmZhMjc0MDZlIn0.uWI-w99z_2NJ_Y3u-DLt21uJwl8KJ-EuDudIIJ93RUY";
            cl.Launcher.userProperties = "{}";
            cl.Launcher.userType = "Mojang";
            
            cl.Launcher.TargetJSON = @"C:\Users\Lenovo\AppData\Roaming\.minecraft\versions\1.7.10\1.7.10.json";

            launcher.Launch();
        }
    }
}
 