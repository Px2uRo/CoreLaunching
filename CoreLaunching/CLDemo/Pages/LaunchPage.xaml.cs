using CLDemo.Modelview;
using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using CLDemo.Data;

namespace CLDemo.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : UserControl
    {
        public LaunchPage()
        {
            InitializeComponent();
            this.DataContext = Modelview.MainViewModel.LaunchageViewModel;
        }

        private void LegacyLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneralData.SelectedLegacyListIndex = LegacyLB.SelectedIndex;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Modelview.StaticVoids.RefreshGameInfoList(Data.GeneralData.DirInfos[Data.GeneralData.SelectedDirInfoListIndex].Path);
        }
    }
}
