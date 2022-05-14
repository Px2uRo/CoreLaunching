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
using CoreLaunching;
using CLDemo.Pages;

namespace CLDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LaunchPage lp = new LaunchPage();
        DownloadPage dp = new DownloadPage();
        SettingsPage sp = new SettingsPage();
        MiscPage mp = new MiscPage();
        public MainWindow()
        {
            CoreLaunching.LocalPath.TmpFolderPath =System.IO.Path.Combine( Environment.CurrentDirectory,"TmpFolder");
            InitializeComponent();
            UserPage.Child = lp;
        }

        private void LaunchPageButton_Click(object sender, RoutedEventArgs e)
        {
            UserPage.Child = lp;
        }

        private void DownloadPageButton_Click(object sender, RoutedEventArgs e)
        {
            UserPage.Child = dp;
        }

        private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            UserPage.Child = sp;
        }

        private void MiscPageButton_Click(object sender, RoutedEventArgs e)
        {
            UserPage.Child = mp;
        }
    }
}
