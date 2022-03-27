using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows;

namespace CLDemo.Modelview
{
    public class GameDirMgrViewModel:VisibleViewModel
    {
        #region Properties
        public ObservableCollection<Data.DirInfo> DirInfos { get; set; }
        public ObservableCollection<Data.GameInfo> GameInfos { get; set; }
        public ICommand AddANewDirCmd { get; set; }
        public ICommand RefreshCmd { get; set; }
        #endregion

        #region Command
        public class AddANewDirCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                var tar = string.Empty;
                if(parameter is string)
                {
                    tar = (parameter as string);
                }
                else
                {
                    dlg.ShowDialog();
                    tar = dlg.SelectedPath;
                }
                if (tar != string.Empty)
                {
                    Data.GeneralData.DirInfos.Add(new Data.DirInfo(tar, "自定义路径"));
                    if (Directory.Exists(Path.Combine(tar, "versions")) != true)
                    {
                        Directory.CreateDirectory(Path.Combine(tar, "versions"));
                    }
                    DirectoryInfo[] directories = new DirectoryInfo(Path.Combine(tar, "versions")).GetDirectories();
                    Data.GeneralData.GameInfos.Clear();
                    Data.GeneralData.SelectedGameListIndex = 0;
                    for (int i = 0; i < directories.Length; i++)
                    {
                        var bb = Path.Combine(directories[i].FullName, directories[i].Name + ".json");
                        Data.GeneralData.GameInfos.Add(new Data.GameInfo(bb));
                    }
                    if((parameter is string)!=true)
                    {
                        for (int i = 0; i < Data.GeneralData.DirInfos.Count; i++)
                        {
                            if (Data.GeneralData.DirInfos[i].Path == tar)
                            {
                                Data.GeneralData.SelectedDirInfoListIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }
        public class RefreshDirListBoxCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Modelview.StaticVoids.RefreshGameInfoList(parameter);
            }
        }

        #endregion
        public GameDirMgrViewModel(LaunchPageViewModel prt):base(prt)
        {
            DirInfos = Data.GeneralData.DirInfos;
            AddANewDirCmd = new AddANewDirCommand();
            GameInfos = Data.GeneralData.GameInfos;
            RefreshCmd = new RefreshDirListBoxCommand();
        }
    }
}
