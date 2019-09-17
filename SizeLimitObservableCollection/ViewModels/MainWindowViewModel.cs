using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Prism.Commands;

namespace SizeLimitObservableCollection.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            Limit = 5;
        }

        private string logString = "Test Data .... ";
        public string LogString
        {
            get { return logString; }
            set { SetProperty(ref logString, value); }
        }

        private int limit = 0;
        public int Limit
        {
            get { return limit; }
            set {
                LogCollection.Limit = value;
                SetProperty(ref limit, value); }
        }

        private DelegateCommand addFirst;
        public DelegateCommand AddFirst =>
            addFirst ?? (addFirst = new DelegateCommand(ExecuteAddFirst));
        void ExecuteAddFirst()
        {
            LogCollection.Insert(0, LogString);
        }

        private DelegateCommand addLast;
        public DelegateCommand AddLast =>
            addLast ?? (addLast = new DelegateCommand(ExecuteAddLast));
        void ExecuteAddLast()
        {
            LogCollection.Add(LogString);
        }

        private SizeLimitStringCollection logCollection = new SizeLimitStringCollection();
        public SizeLimitStringCollection LogCollection
        {
            get { return logCollection; }
            set { SetProperty(ref logCollection, value); }
        }


        public class SizeLimitStringCollection : ObservableCollection<string>
        {
            protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                return true;
            }

            protected virtual void Shrink(int index)
            {
                if (index != 0)
                {
                    while (Limit < Count) RemoveAt(0);
                }
                else
                {
                    while (Limit < Count) RemoveAt(Count - 1);
                }
            }

            private int limit = 10;
            public int Limit
            {
                get { return limit; }
                set
                {
                    if (value < 1) value = 1;
                    if (SetProperty(ref limit, value))
                    {
                        Shrink(Count);
                    }
                }
            }

            protected override void InsertItem(int index, string item)
            {
                item = DateTime.Now.ToString("hh:MM:ss ") + item;
                base.InsertItem(index, item);
                Shrink(index);
            }
        }
    }

    
}
