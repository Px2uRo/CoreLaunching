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
using CLDemo.Data;
using CLDemo.Modelview;

namespace CLDemo.Pages
{
    /// <summary>
    /// AddLegacyUser.xaml 的交互逻辑
    /// </summary>
    public partial class AddLegacyUser : UserControl
    {
        public AddLegacyUser()
        {
            InitializeComponent();
        }

        private void TextBlock_Error(object sender, ValidationErrorEventArgs e)
        {
            var errors = Validation.GetErrors(NewLegacyName);
            var errorlist = from error in errors select error.ErrorContent;
            errorTextBox.Text = string.Join(',', errorlist);
        }
    }
}
