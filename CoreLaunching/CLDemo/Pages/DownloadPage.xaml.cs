using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    /// DownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadPage : UserControl
    {
        string DownloadJson = string.Empty;
        Data.VersionInJson DataInfos;
        int ReadyToDownloadIndex;
        CoreLaunching.MultiThreadDownloader md = new CoreLaunching.MultiThreadDownloader();
        public DownloadPage()
        {
            InitializeComponent();
            try
            {
                HttpClient client = new HttpClient();
                DownloadJson = client.GetStringAsync("https://launchermeta.mojang.com/mc/game/version_manifest.json").Result.ToString();
            }
            catch
            {
                MessageBox.Show("你没有联网吗？我没法获取到下载服务器的信息，您会在下载页面迷失方向。");
            }
            DataInfos = JsonConvert.DeserializeObject<Data.VersionInJson>(DownloadJson);
            LastesRealse.Content = "最新正式版：" + DataInfos.Latest.Release;
            LastesSnapsort.Content ="最新快照版" + DataInfos.Latest.Snapshot;
            DownloadLB.ItemsSource = DataInfos.Versions;
        }

        private void DownLoad(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name=="LastesRealse")
            {
                for (int i = 0; i < DataInfos.Versions.Count; i++)
                {
                    if (DataInfos.Versions[i].Id == DataInfos.Latest.Release)
                    {
                        ReadyToDownloadIndex = i;
                        SetNewGame.Visibility = Visibility.Visible;
                        VersionLB.Visibility = Visibility.Hidden;
                        NewDictory.Text = DataInfos.Latest.Release;
                        break;
                    }
                }
            }
            else if ((sender as Button).Name == "LastesSnapsort")
            {
                for (int i = 0; i < DataInfos.Versions.Count; i++)
                {
                    if (DataInfos.Versions[i].Id == DataInfos.Latest.Snapshot)
                    {
                        ReadyToDownloadIndex = i;
                        SetNewGame.Visibility = Visibility.Visible;
                        VersionLB.Visibility = Visibility.Hidden;
                        NewDictory.Text = DataInfos.Latest.Snapshot;
                        break;
                    }
                }
            }
            else if ((sender as Button).Name == "DownLoadBtn")
            {
                var path = System.IO.Path.Combine(Data.GeneralData.DirInfos[Data.GeneralData.SelectedDirInfoListIndex].Path,"versions", NewDictory.Text);
                if(Directory.Exists(path))
                {
                    MessageBox.Show("（此处以后会用到Rules的）你不能使用已经存在的目录名");
                }
                else
                {
                    Directory.CreateDirectory(path);
                    md.GoGoGo(DataInfos.Versions[ReadyToDownloadIndex].Url,8,path);
                    FileInfo renamefi = new FileInfo(System.IO.Path.Combine(path, System.IO.Path.GetFileName(DataInfos.Versions[ReadyToDownloadIndex].Url)));
                    renamefi.MoveTo(System.IO.Path.Combine(path, NewDictory.Text + ".json"), true);
                    Modelview.StaticVoids.RefreshGameInfoList(Data.GeneralData.DirInfos[Data.GeneralData.SelectedDirInfoListIndex].Path);
                    SetNewGame.Visibility = Visibility.Hidden;
                    VersionLB.Visibility = Visibility.Visible;
                }
            }
        }
        private void DownloadLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReadyToDownloadIndex = (sender as ListBox).SelectedIndex;
            SetNewGame.Visibility = Visibility.Visible;
            VersionLB.Visibility = Visibility.Hidden;
            NewDictory.Text = DataInfos.Versions[ReadyToDownloadIndex].Id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetNewGame.Visibility = Visibility.Hidden;
            VersionLB.Visibility =  Visibility.Visible;
        }
    }
}
