﻿using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;
using SRWJEditV.Attributes;
using SRWJData.Extensions;
using SRWJData.DataHandlers;
using SRWJData.Models;

namespace SRWJEditV.ViewModels
{
    [EditorViewModel]
    public class WeaponViewModel : ViewModelBase
    {
        private List<WeaponModel> Weapons { get; }
        private Dictionary<int, int> IndexedNamePointers;
        private SortedDictionary<int, string> WeaponPointers;
        private List<int> PointerKeys;

        [Reactive] public ObservableCollection<string> WeaponNames { get; set; }
        [Reactive] public ObservableCollection<KeyValuePair<int, string>> ObservablePointers { get; set; }
        [Reactive] public int SelectedPointer1 { get; set; }
        [Reactive] public int SelectedPointer2 { get; set; }
        [Reactive] public WeaponModel SelectedWeapon { get; set; }
        [Reactive] public int SelectedIndex { get; set; }
        [Reactive] public string WeaponName1 { get; set; }
        [Reactive] public string WeaponName2 { get; set; }
        [Reactive] public int MaxName1 { get; set; }
        [Reactive] public int MaxName2 { get; set; }
        [Reactive] public byte BulletTypeIndex { get; set; }
        [Reactive] public int WeaponTypeIndex { get; set; }

        public WeaponViewModel()
        {
            Weapons = new();
            WeaponNames = new();
            WeaponPointers = new();
            ObservablePointers = new();
            IndexedNamePointers = new();
            SelectedWeapon = new();
            PointerKeys = new();
            SelectedIndex = 0;
            WeaponName1 =string.Empty;
            WeaponName2=string.Empty;
            BulletTypeIndex = 0;
            WeaponTypeIndex = -1;
            this.WhenAnyValue(x => x.WeaponTypeIndex).Subscribe(_ => { if (WeaponTypeIndex != -1) WeaponTypeIndex = -1; });
        }


        public WeaponViewModel(IModelHandler modelHandler) : this()
        {
            Weapons = modelHandler.GetList<WeaponModel>();            
            WeaponPointers = modelHandler.GetPointerDictionary<WeaponModel>();
            PointerKeys = WeaponPointers.Keys.ToList();
            ResetWeaponList();
            ResetFakePointers();

            //When the SelectedIndex changes, update the SelectedWeapon
            this.WhenAnyValue(x => x.SelectedIndex).Subscribe(x => { if (x >= 0) SelectedWeapon = Weapons[x]; });
            //When the SellectedWeapon changes, update the WeaponName
            this.WhenAnyValue(x => x.SelectedWeapon).Subscribe(x =>
            {
                WeaponName1 = WeaponPointers[x.NamePointer1];
                MaxName1 = WeaponName1.Length + WeaponPointers.GetBytesRemaining(x.NamePointer1);
                WeaponName2 = WeaponPointers[x.NamePointer2];
                MaxName2 = WeaponName1.Length + WeaponPointers.GetBytesRemaining(x.NamePointer2);
                BulletTypeIndex = x.BulletFlag > 7 ? (byte)8 : x.BulletFlag;
            });

            //When the WeaponName changes, update the Dictionary, SelectedPointer, and the WeaponList
            this.WhenAnyValue(x => x.WeaponName1).Subscribe(x =>
            {
                WeaponName1 = WeaponPointers.UpdateString(SelectedWeapon.NamePointer1, x);
                SelectedPointer1 = PointerKeys.IndexOf(SelectedWeapon.NamePointer1);
                MaxName1 = WeaponName1.Length + WeaponPointers.GetBytesRemaining(SelectedWeapon.NamePointer1);
                if (SelectedWeapon.NamePointer1 == SelectedWeapon.NamePointer2)
                    WeaponName2 = WeaponName1;
                UpdateWeaponList();
            });
            this.WhenAnyValue(x => x.WeaponName2).Subscribe(x =>
            {
                WeaponName2 = WeaponPointers.UpdateString(SelectedWeapon.NamePointer2, x);
                SelectedPointer2 = PointerKeys.IndexOf(SelectedWeapon.NamePointer2);
                MaxName2 = WeaponName2.Length + WeaponPointers.GetBytesRemaining(SelectedWeapon.NamePointer2);
                if (SelectedWeapon.NamePointer2 == SelectedWeapon.NamePointer1)
                    WeaponName1 = WeaponName2;
                UpdateWeaponList();
            });

            //When the SelectedPointer changes, update the displayed WeaponName
            this.WhenAnyValue(x => x.SelectedPointer1).Subscribe(x =>
            {
                if (x >= 0 && SelectedIndex >= 0)
                {
                    SelectedWeapon.NamePointer1 = PointerKeys[x];
                    IndexedNamePointers[SelectedIndex] = SelectedWeapon.NamePointer1;
                    WeaponName1 = WeaponPointers[SelectedWeapon.NamePointer1];                    
                }
            });
            this.WhenAnyValue(x => x.SelectedPointer2).Subscribe(x =>
            {
                if (x >= 0)
                { 
                    SelectedWeapon.NamePointer2 = PointerKeys[x];
                    WeaponName2 = WeaponPointers[SelectedWeapon.NamePointer2];
                }
            });

            this.WhenAnyValue(x => x.BulletTypeIndex).Subscribe(x =>
            {
                if (x == 8 && SelectedWeapon.BulletFlag != 255) 
                    SelectedWeapon.BulletFlag=255;
                if (x < 8 && x >= 0 && SelectedWeapon.BulletFlag != x) 
                    SelectedWeapon.BulletFlag=x;
            });
        }

        private void UpdateWeaponList()
        {
            int oldIndex = SelectedIndex;
            bool update = false;
            foreach (KeyValuePair<int, int> kvp in IndexedNamePointers)
            {
                string wep = GetListName(Weapons[kvp.Key].Index, WeaponPointers[kvp.Value]);
                string display = WeaponNames[kvp.Key];
                if (display != wep)
                {
                    WeaponNames[kvp.Key] = wep;
                    update = true;
                }
            }
            if (update)
            {
                UpdateFakePointers();
                this.RaisePropertyChanged(nameof(WeaponNames));
                SelectedIndex = oldIndex;
            }
        }
        private void ResetWeaponList()
        {
            int oldIndex = SelectedIndex;
            WeaponNames.Clear();
            IndexedNamePointers.Clear();
            for (int i = 0; i < Weapons.Count; i++)
            {
                WeaponNames.Add(GetListName(Weapons[i].Index, WeaponPointers[Weapons[i].NamePointer1]));
                IndexedNamePointers.Add(Weapons[i].Index, Weapons[i].NamePointer1);
            }
            SelectedIndex = oldIndex;
        }

        private void UpdateFakePointers()
        {
            int oldIndex1 = SelectedPointer1, oldIndex2 = SelectedPointer2;
            var changed = ObservablePointers.Select((value, index) => new { value, index })
                                .Where(pair => pair.value.Value != WeaponPointers[pair.value.Key])
                                .Select(pair => pair.index)
                                .ToList();
            foreach (int i in changed)
            {
                ObservablePointers[i] = new KeyValuePair<int, string>(ObservablePointers[i].Key, WeaponPointers[ObservablePointers[i].Key]);
            }
            SelectedPointer1 = oldIndex1;
            SelectedPointer2 = oldIndex2;
        }

        private void ResetFakePointers()
        {
            ObservablePointers.Clear();
            foreach (KeyValuePair<int, string> kvp in WeaponPointers)
            {
                ObservablePointers.Add(kvp);
            }
        }
               
        private string GetListName(int index, string name)
        {
            return $"{Weapons[index].Index:X4}: {name}";
        }


    }
}
