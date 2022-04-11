using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using SRWJEditV.Utilities;
using SRWJEditV.ViewModels;
using System.Reactive;
using System.Threading.Tasks;

namespace SRWJEditV.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.FindControl<MenuItem>("Exit").Command = ReactiveCommand.Create(() => Close());
            this.FindControl<MenuItem>("Save").Command = ReactiveCommand.Create(() => ModelHandler.GetInstance()?.SaveData());
            this.WhenActivated(d => d(ViewModel!.EditorInteraction.RegisterHandler(DoShowPlugin)));
            this.WhenActivated(d => d(ViewModel!.OpenFileDialog.RegisterHandler(OpenFile)));
        }
        private async Task DoShowPlugin(InteractionContext<ViewModelBase, Unit?> interaction)
        {
            IControl control = new ViewLocator().Build(interaction.Input);
            if (control is Window win)
            {
                win.DataContext = interaction.Input;
                var result = await win.ShowDialog<Unit?>(this);
                interaction.SetOutput(result);
            }
        }
        private async Task OpenFile(InteractionContext<Unit, string> interaction)
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "SRWJ ROM File", Extensions= { "gba" } });
            dlg.Title = "Open File";
            var result = await dlg.ShowAsync(this);
            if (result is not null)
                interaction.SetOutput(result[0]);
        }

    }
}
