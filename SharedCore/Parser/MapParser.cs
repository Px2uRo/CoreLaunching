using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace CoreLaunching.Parser
{
    public class MapsParser
    {
        public static MapInfo[] Parse(string saves)
        {
            var rel = new List<MapInfo>();
            for (int i = 0; i < Directory.GetDirectories(saves).Count(); i++)
            {
                var save = Directory.GetDirectories(saves)[i];
                rel.Add(new MapInfo(save));
            }
            return rel.ToArray();
        }
    }
    public class MapInfo:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RasieEvent(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public const string IconName = "icon.png";
        public string Folder { get; set; }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; RasieEvent(nameof(Name)); }
        }

        public MapInfo(string folder)
        {
            Folder = folder;
            Name = Path.GetFileName(folder);
            OpenFolder = new OpenFolderCommand(folder);
        }
        public ICommand OpenFolder { get; set; }
    }

    public class OpenFolderCommand : ICommand
    {
        private string _folder;
        public OpenFolderCommand(string folder)
        {
            _folder= folder;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "explorer";
            proc.StartInfo.Arguments = _folder;
            proc.Start();
        }
    }
}
