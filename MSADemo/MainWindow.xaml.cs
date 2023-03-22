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
using CoreLaunching.Auth;
using CoreLaunching.JsonTemplates;
using Microsoft.Web.WebView2.Wpf;

namespace MSADemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MSAuther MSauth = new MSAuther();
        public MainWindow()
        {
            InitializeComponent();
            wbv.Source = MSauth.LogInUrl;
            wbv.SourceChanged += Wbv_SourceChanged;
        }

        private void Wbv_SourceChanged(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
           PlayerInfo plr = MSauth.MSAuth("" +
"https://login.live.com/oauth20_desktop.srf?code=M.R3_BAY.2.0c08f0d9-43bd-bb7e-1710-8279d86251cf&lc=2052"
);
            TB.Text = "你的uuid:" + plr.Id;
        }
    }
}
