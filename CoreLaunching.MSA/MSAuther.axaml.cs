using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CoreLaunching.MSA
{
    public partial class MSAuther : Window
    {
        public MSAuther()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
