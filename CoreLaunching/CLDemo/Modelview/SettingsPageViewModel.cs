using CLDemo.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using System.IO;

namespace CLDemo.Modelview
{
    public class SettingsPageViewModel : VisibleViewModel
    {
        public class AddJavaCommand : ICommand
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                fileDialog.Multiselect = true;
                fileDialog.Title = "请选择Java";
                fileDialog.Filter = "Java窗口程序|Javaw.exe";
                fileDialog.ShowDialog();
                if (fileDialog.FileName != String.Empty)
                {
                    GeneralData.JavaPathInfos.Add(new JavaPathInfo(fileDialog.FileName));
                }
            }
        }
        public class SearchJavaInfos : ICommand
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                List<DirectoryInfo> alldirs = new List<DirectoryInfo>();
                var tmp = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)).GetDirectories();
                for (int i = 0; i < tmp.Length; i++)
                {
                    alldirs.Add(tmp[i]);
                }
                for (int i = 0; i < alldirs.Count; i++)
                {
                    DirectoryInfo[] tmp2 = new DirectoryInfo[0];
                    try
                    {
                        tmp2 = alldirs[i].GetDirectories();
                    }
                    catch
                    {

                    }
                    for (int j = 0; j < tmp2.Length; j++)
                    {
                        alldirs.Add(tmp2[j]);
                    }
                    tmp2 = null;
                }
                tmp = null;
                for (int i = 0; i < alldirs.Count; i++)
                {
                    FileInfo[] tmp2 = new FileInfo[0];
                    try
                    {
                        tmp2 = alldirs[i].GetFiles();
                    }
                    catch
                    {

                    }
                    for (int j = 0; j < tmp2.Length; j++)
                    {
                        if (tmp2[j].Name == "javaw.exe")
                        {
                            GeneralData.JavaPathInfos.Add(new JavaPathInfo(tmp2[j].FullName));
                        }
                    }
                    tmp2 = null;
                }
                alldirs.Clear();
            }
        }
        public IEnumerable<JavaPathInfo> JavaPathInfos { get; set; }
        public ICommand SearchJavaInfosCmd { get; set; }

        public ICommand AddCommand { get; private set; }
        public SettingsPageViewModel() : base(null)
        {
            AddCommand = new AddJavaCommand();
            //这一行其实是对通用数据的引用
            JavaPathInfos = Data.GeneralData.JavaPathInfos;
            SearchJavaInfosCmd = new SearchJavaInfos();
        }
    }
}
