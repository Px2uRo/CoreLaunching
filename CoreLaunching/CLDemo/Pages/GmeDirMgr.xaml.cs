using CLDemo.Modelview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLDemo.Pages
{
    /// <summary>
    /// GmeDirMgr.xaml 的交互逻辑
    /// </summary>
    public partial class GmeDirMgr : UserControl
    {
        public GmeDirMgr()
        {
            InitializeComponent();
        }

        private void CB_Initialized(object sender, EventArgs e)
        {
            string CLDemoDir = System.Environment.CurrentDirectory;
            var mcpath = System.IO.Path.Combine(CLDemoDir, ".minecraft");
            if (System.IO.Directory.Exists(mcpath) != true)
            {
                System.IO.Directory.CreateDirectory(mcpath);
            }
            Data.GeneralData.DirInfos.Add(new Data.DirInfo(mcpath, "当前文件夹"));
            (sender as ComboBox).SelectedIndex = 0;
            Data.GeneralData.SelectedDirInfoListIndex = (sender as ComboBox).SelectedIndex;
        }

        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.GeneralData.SelectedDirInfoListIndex = (sender as ComboBox).SelectedIndex;
            (this.DataContext as GameDirMgrViewModel).RefreshCmd.Execute
                ((((sender as ComboBox).ItemsSource) as ObservableCollection<Data.DirInfo>)
                [Data.GeneralData.SelectedDirInfoListIndex].Path);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.GeneralData.SelectedGameListIndex = (sender as ListBox).SelectedIndex;
        }
    }
}
