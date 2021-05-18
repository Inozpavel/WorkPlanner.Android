using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkPlanner.Requests;
using WorkPlanner.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class RegisterPageViewModel : BaseViewModel
    {
        public RegisterPageViewModel()
        {
            Request = new RegisterUserRequest();
            RegisterCommand = new Command(ServerHelper.DecorateFailedConnectToServer(Register, OnConnectionFailed));
        }

        public ICommand RegisterCommand { get; }

        public RegisterUserRequest Request { get; }

        public event EventHandler<string> OnRegistrationFailed;

        public event EventHandler OnSendingDataStarted;

        public event EventHandler OnRegistrationSuccess;

        private async Task Register()
        {
            OnSendingDataStarted?.Invoke(this, EventArgs.Empty);
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                OnRegistrationFailed?.Invoke(this, AppResources.InternetIsMissing);
                return;
            }

            string data = await ServerHelper.SerializeAsync(Request);

            var result = await ServerHelper.GetClient().PostAsync(Settings.RegisterUrl,
                new StringContent(data, Encoding.UTF8, "application/json"));

            if (result.StatusCode == HttpStatusCode.OK)
            {
                OnRegistrationSuccess?.Invoke(this, EventArgs.Empty);
                return;
            }

            string errorBody = await result.Content.ReadAsStringAsync();
            OnRegistrationFailed?.Invoke(this, ServerHelper.GetErrorFromResponse(errorBody));
        }
    }
}