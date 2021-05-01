using System.ComponentModel;
using System.Runtime.CompilerServices;
using WorkPlanner.Requests;

namespace WorkPlanner.ViewModels
{
    public class RegisterPageViewModel : INotifyPropertyChanged
    {
        public RegisterPageViewModel() => Request = new RegisterUserRequest();

        public RegisterUserRequest Request { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}