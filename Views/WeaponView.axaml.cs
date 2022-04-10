using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SRWJEditV.Views
{
    public partial class WeaponView : Window
    {
        public WeaponView()
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
