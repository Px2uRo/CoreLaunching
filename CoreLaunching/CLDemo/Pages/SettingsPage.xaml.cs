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

namespace CLDemo.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = Modelview.MainViewModel.SettingsPageViewModel;
            (this.DataContext as Modelview.SettingsPageViewModel).SearchJavaInfosCmd.Execute(null);
        }

        private void JavaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.GeneralData.SelectedJavaPathListIndex = (sender as ListBox).SelectedIndex;
        }
    }
}
