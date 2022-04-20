using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections;
using System.Collections.Generic;

namespace SRWJEditV.Views
{
    public partial class ByteCheckList : UserControl
    {
        public static readonly DirectProperty<ByteCheckList, IEnumerable> ItemsProperty =
            ItemsControl.ItemsProperty.AddOwner<ByteCheckList>(o => o.Items, (o, v) => o.Items = v);
        IEnumerable _items = new AvaloniaList<object>();
        public IEnumerable Items
        {
            get => _items;
            set { if (SetAndRaise(ItemsProperty, ref _items, value)) SetItemCtrl(); }

        }

        public static readonly DirectProperty<ByteCheckList, byte> ValueProperty =
            AvaloniaProperty.RegisterDirect<ByteCheckList, byte>(nameof(Value), o => o.Value, (o, v) => o.Value = v, defaultBindingMode:Avalonia.Data.BindingMode.TwoWay);
        byte _value = 0;
        public byte Value
        {
            get => _value;
            set
            {
                if (value > maximum)
                    value = maximum;
                if (SetAndRaise(ValueProperty, ref _value, value))
                    SetChecks();
            }
        }

        List<CheckBox> checks = new();
        StackPanel StackPnl = new();
        private byte maximum = 255;

        public ByteCheckList()
        {
            InitializeComponent();
            Content = StackPnl;

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetChecks()
        {
            for (int i = 0; i < checks.Count && i < 8; i++)
            {
                bool flag = ((Value >> i) & 1) == 1;
                checks[i].IsChecked = flag;
            }
        }

        private void SetItemCtrl()
        {
            if (Items is not null && StackPnl is not null)
            {
                checks.Clear();
                StackPnl.Children.Clear();
                int i = 0;
                maximum = 0;
                foreach (object o in Items)
                {
                    if (i > 7) break;
                    maximum += (byte)(1 << i);
                    CheckBox cb = new CheckBox() { Content = o, Margin = new Thickness(5, -3, 5, -3) };
                    cb.Click += Cb_Click;
                    checks.Add(cb);
                    i++;
                }
                StackPnl.Children.AddRange(checks);
            }
        }

        private void Cb_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            byte b = 0;
            for (int i = 0; i < checks.Count && i < 8; i++)
            {
                b += (byte)((checks[i].IsChecked is true ? 1 : 0) << i);
            }
            Value = b;
        }
    }
}
