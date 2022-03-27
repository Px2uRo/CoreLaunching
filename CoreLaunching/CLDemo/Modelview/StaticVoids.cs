using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDemo.Modelview
{
    public static class StaticVoids
    {
        public static void RefreshGameInfoList(object? parameter)
        {
            string Parameter = parameter as string;
            Data.GeneralData.GameInfos.Clear();
            if (Directory.Exists(Path.Combine(Parameter, "versions")) != true)
            {
                Directory.CreateDirectory(Path.Combine(Parameter, "versions"));
            }
            DirectoryInfo[] directories = new DirectoryInfo(Path.Combine(Parameter, "versions")).GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                var bb = Path.Combine(directories[i].FullName, directories[i].Name + ".json");
                Data.GeneralData.GameInfos.Add(new Data.GameInfo(bb));
            }
        }
    }
}