using System.ComponentModel;
using System.Runtime.CompilerServices;

using Tanji.Services;

namespace Tanji.Helpers
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public static IMaster Master => App.Master;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        protected void RaiseOnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            OnPropertyChanged(
                new PropertyChangedEventArgs(propertyName));
        }

        public bool AlwaysTrue(object obj) => true;
        public bool AlwaysFalse(object obj) => false;
    }
}