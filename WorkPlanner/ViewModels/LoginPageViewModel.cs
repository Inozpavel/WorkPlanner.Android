using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class LoginPageViewModel : INotifyPropertyChanged
    {
        private readonly INavigation _navigation;

        private string _password;


        public LoginPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            LoginCommand = new Command(SendLoginData);
            OpenRegisterPageCommand = new Command(async () => await OpenRegisterPage());
        }

        public LoginPageViewModel()
        {
        }

        public ICommand LoginCommand { get; }

        public ICommand OpenRegisterPageCommand { get; }

        public string Login { get; set; }

        public bool ShouldRememberUser { get; set; }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task OpenRegisterPage() => await _navigation.PushAsync(new RegisterPage());

        private void SendLoginData()
        {
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}