using Avalonia.Controls;
using SRWJEditV.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SRWJEditV.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand OpenFile { get; private set; }
        public Interaction<ViewModelBase, Unit?> EditorInteraction { get; private set; }
        public Interaction<Unit, string> OpenFileDialog { get; private set; }
        [Reactive] public bool fileLoaded { get; set; }
        [Reactive] List<MenuItem> EditorMenuItems { get; set; }

        public MainWindowViewModel()
        {
            EditorInteraction = new Interaction<ViewModelBase, Unit?>();
            OpenFileDialog = new Interaction<Unit, string>();
            EditorMenuItems = new List<MenuItem>();

            OpenFile = ReactiveCommand.Create(async () =>
            {
                var fileLoc = await OpenFileDialog.Handle(new Unit());
                if (fileLoc is not null)
                {
                    ModelHandler.SetFilePath(fileLoc);
                    fileLoaded = true;
                }
                else fileLoaded = false;
            });
        }

        public MainWindowViewModel(List<Type> viewModels) : this()
        {
            foreach (Type t in viewModels)
            {
                MenuItem menu = new() { Header = t.Name.Replace("ViewModel", "") };
                var ctorWithHandler = TFactory.CreateConstructor(t, typeof(ModelHandler));
                var ctorNoHandler = TFactory.CreateConstructor(t);
                menu.Command = ReactiveCommand.Create(async () =>
                {
                    var handler = ModelHandler.GetInstance();
                    var result = handler is not null ? await HandlePlugin((ViewModelBase)ctorWithHandler(handler)) : await HandlePlugin((ViewModelBase)ctorNoHandler());
                });
                EditorMenuItems.Add(menu);
            }
        }

        private async Task<Unit?> HandlePlugin(ViewModelBase plugin)
        {
            return await EditorInteraction.Handle(plugin);
        }
    }
}
