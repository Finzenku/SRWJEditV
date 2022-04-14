using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SRWJEditV.Models
{
    public class ObservablePair<T1, T2> : INotifyPropertyChanged
    {
        private T1 _item1;
        private T2 _item2;
        public T1 Item1
        {
            get => _item1;
            set
            {
                if (_item1 == null || !_item1.Equals(value))
                {
                    _item1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public T2 Item2 { get => _item2; 
            set
            {
                if (_item2 == null || !_item2.Equals(value))
                {
                    _item2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservablePair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
            //Stupid null warnings, go away; this shouldn't do anything
            _item1??= item1;
            _item2??= item2;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
