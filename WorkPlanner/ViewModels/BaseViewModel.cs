using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkPlanner.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public event EventHandler ConnectionFailed;

        protected void OnConnectionFailed() => ConnectionFailed?.Invoke(this, EventArgs.Empty);

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}