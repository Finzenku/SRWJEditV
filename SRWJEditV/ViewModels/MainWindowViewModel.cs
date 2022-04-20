using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SRWJData.DataHandlers;
using SRWJData.Utilities;
using NP.Avalonia.Visuals.ThemingAndL10N;
using Avalonia;
using NP.Avalonia.Visuals.Behaviors;
using NP.ViewModelInterfaces.ThemingAndL10N;

namespace SRWJEditV.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand OpenFile { get; private set; }
        public ICommand SaveFile { get; private set; }
        public Interaction<ViewModelBase, Unit?> EditorInteraction { get; private set; }
        public Interaction<Unit, string> OpenFileDialog { get; private set; }
        [Reactive] public bool fileLoaded { get; set; }
        [Reactive] List<MenuItem> EditorMenuItems { get; set; }
        public List<MenuItem> LanguageItems { get; private set; }
        ThemeLoader _languageLoader;

        public MainWindowViewModel()
        {
            EditorInteraction = new Interaction<ViewModelBase, Unit?>();
            OpenFileDialog = new Interaction<Unit, string>();
            EditorMenuItems = new List<MenuItem>();
            LanguageItems = new List<MenuItem>();
            _languageLoader = Application.Current!.Resources.GetThemeLoader("LanguageLoader")!;
            _languageLoader.SelectedThemeChangedEvent += ThemeChanged;
            CreateLanguageMenuItems();

            OpenFile = ReactiveCommand.Create(async () =>
            {
                var fileLoc = await OpenFileDialog.Handle(new Unit());
                if (fileLoc is not null)
                {
                    DataHandlers.UseROMHandler(fileLoc);
                    fileLoaded = true;
                }
                else fileLoaded = false;
            });
            SaveFile = ReactiveCommand.Create(() =>
            {
                ((ROMModelHandler?)DataHandlers.GetModelHandler())?.SaveData();
            });
        }

        public MainWindowViewModel(List<Type> viewModels) : this()
        {
            foreach (Type t in viewModels)
            {
                string header = t.Name.Replace("ViewModel", "");
                _languageLoader.TryGetResource(header, out object? o);
                MenuItem menu = new() { Name=header, Header = o??header };
                var ctorWithHandler = TFactory.CreateConstructor(t, typeof(IModelHandler));
                var ctorNoHandler = TFactory.CreateConstructor(t);
                menu.Command = ReactiveCommand.Create(async () =>
                {
                    var handler = DataHandlers.GetModelHandler();
                    var result = handler is not null ? await HandlePlugin((ViewModelBase)ctorWithHandler(handler)) : await HandlePlugin((ViewModelBase)ctorNoHandler());
                });
                EditorMenuItems.Add(menu);
            }
        }

        private async Task<Unit?> HandlePlugin(ViewModelBase plugin)
        {
            return await EditorInteraction.Handle(plugin);
        }

        private void ThemeChanged(IThemeLoader obj)
        {
            foreach(MenuItem mi in EditorMenuItems)
            {
                _languageLoader.TryGetResource(mi.Name!, out object? o);
                mi.Header = o??mi.Header;
            }
        }

        private void CreateLanguageMenuItems()
        {
            foreach (ThemeInfo info in _languageLoader.Themes)
            {
                MenuItem menuItem = new() { Header = info.Id };
                menuItem.Click += (s, e) => _languageLoader.SelectedThemeId = menuItem.Header;
                LanguageItems.Add(menuItem);
            }
        }
    }
}
