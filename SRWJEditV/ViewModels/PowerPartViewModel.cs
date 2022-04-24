using SRWJData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRWJEditV.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using SRWJData.DataHandlers;

namespace SRWJEditV.ViewModels
{
    [EditorViewModel]
    public class PowerPartViewModel : ViewModelBase
    {
        private List<PowerPartModel> parts;
        private Dictionary<int, int> indexedNamePointers;
        private SortedDictionary<int, string> partPointers;

        [Reactive] public ObservableCollection<string> PartNames { get; set; }
        [Reactive] public ObservableCollection<KeyValuePair<int, string>> ObservablePointers { get; set; }
        [Reactive] public int SelectedNamePointer { get; set; }
        [Reactive] public int SelectedInfo1Pointer { get; set; }
        [Reactive] public int SelectedInfo2Pointer { get; set; }
        [Reactive] public int SelectedInfo3Pointer { get; set; }
        [Reactive] public int SelectedInfo4Pointer { get; set; }
        [Reactive] public int SelectedIndex { get; set; }
        [Reactive] public PowerPartModel SelectedPart { get; set; }
        [Reactive] public string SelectedPartName { get; set; }
        [Reactive] public string SelectedPartInfo1 { get; set; }
        [Reactive] public string SelectedPartInfo2 { get; set; }
        [Reactive] public string SelectedPartInfo3 { get; set; }
        [Reactive] public string SelectedPartInfo4 { get; set; }
        [Reactive] public byte SelectedCategory { get; set; }

        public PowerPartViewModel()
        {
            parts = new();
            partPointers = new();
            indexedNamePointers = new();
            ObservablePointers = new();
            PartNames = new();
            SelectedIndex = 0;
        }

        public PowerPartViewModel(IModelHandler modelHandler) : this()
        {
            parts = modelHandler.GetList<PowerPartModel>();
            partPointers = modelHandler.GetPointerDictionary<PowerPartModel>();
            SetPartNames();
            ResetFakePointers();

            this.WhenAnyValue(x => x.SelectedIndex).Subscribe(x => 
            { 
                if (x>=0)
                {
                    SelectedPart = parts[x];
                    SelectedPartName = partPointers[SelectedPart.NamePointer];
                    SelectedPartInfo1 = partPointers[SelectedPart.Info1Pointer];
                    SelectedPartInfo2 = partPointers[SelectedPart.Info2Pointer];
                    SelectedPartInfo3 = partPointers[SelectedPart.Info3Pointer];
                    SelectedPartInfo4 = partPointers[SelectedPart.Info4Pointer];
                    SelectedCategory = SelectedPart.Category < 3 ? SelectedPart.Category : (byte)3;
                }
            });
            this.WhenAnyValue(x => x.SelectedCategory).Subscribe(x =>
            {
                if (x > 0)
                    SelectedPart.Category = x < 3 ? x : (byte)12;
            });
        }

        private string GetListName(PowerPartModel ppm) => $"{ppm.Index:X2} {partPointers[ppm.NamePointer]}";
        private string GetListName(int index, string name) => $"{index:X2} {name}";
        private void UpdateFakePointers()
        {
            int np = SelectedNamePointer, ip1 = SelectedInfo1Pointer, ip2 = SelectedInfo2Pointer, ip3 = SelectedInfo3Pointer, ip4 = SelectedInfo4Pointer;
            var changed = ObservablePointers.Select((value, index) => new { value, index })
                                .Where(pair => pair.value.Value != partPointers[pair.value.Key])
                                .Select(pair => pair.index)
                                .ToList();
            foreach (int i in changed)
            {
                ObservablePointers[i] = new KeyValuePair<int, string>(ObservablePointers[i].Key, partPointers[ObservablePointers[i].Key]);
            }
            SelectedNamePointer = np;
            SelectedInfo1Pointer = ip1;
            SelectedInfo2Pointer = ip2;
            SelectedInfo3Pointer = ip3;
            SelectedInfo4Pointer = ip4;
        }

        private void UpdatePartList()
        {
            int oldIndex = SelectedIndex;
            bool update = false;
            foreach (KeyValuePair<int, int> kvp in indexedNamePointers)
            {
                string part = GetListName(parts[kvp.Key].Index, partPointers[kvp.Value]);
                string display = PartNames[kvp.Key];
                if (display != part)
                {
                    PartNames[kvp.Key] = part;
                    update = true;
                }
            }
            if (update)
            {
                UpdateFakePointers();
                this.RaisePropertyChanged(nameof(PartNames));
                SelectedIndex = oldIndex;
            }
        }

        private void ResetFakePointers()
        {
            ObservablePointers.Clear();
            foreach (KeyValuePair<int, string> kvp in partPointers)
            {
                ObservablePointers.Add(kvp);
            }
        }

        private void SetPartNames()
        {
            int oldIndex = SelectedIndex;
            PartNames.Clear();
            indexedNamePointers.Clear();
            for (int i = 0; i < parts.Count; i++)
            {
                PartNames.Add(GetListName(parts[i]));
                indexedNamePointers.Add(parts[i].Index, parts[i].NamePointer);
            }
            SelectedIndex = oldIndex;
        }

    }
}
