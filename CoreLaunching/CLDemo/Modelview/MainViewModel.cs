using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDemo.Modelview
{
    public static class MainViewModel
    {
        static public LaunchPageViewModel LaunchageViewModel { get; set; }
        static public SettingsPageViewModel SettingsPageViewModel { get; set; }
        static public GameDirMgrViewModel GameDirMgrViewModel { get; set; }
        static MainViewModel()
        {
            LaunchageViewModel = new LaunchPageViewModel();
            SettingsPageViewModel = new SettingsPageViewModel();
        }
    }
}
