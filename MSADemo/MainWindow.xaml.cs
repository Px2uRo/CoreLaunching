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
           TB.Text = MSauth.MSAuth(wbv.Source.ToString());
        }
    }
}
