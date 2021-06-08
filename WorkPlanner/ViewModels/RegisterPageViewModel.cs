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

        private async Task Register()
        {
            MessagingCenter.Send(this, Messages.ActionStarted);
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(AppResources.InternetIsMissing, Messages.RegistrationFail);
                return;
            }

            string data = await ServerHelper.SerializeAsync(Request);

            var result = await ServerHelper.GetClient().PostAsync(Settings.RegisterUrl,
                new StringContent(data, Encoding.UTF8, "application/json"));

            if (result.StatusCode == HttpStatusCode.OK)
            {
                MessagingCenter.Send(this, Messages.RegistrationSuccess);
                return;
            }

            string errorBody = await result.Content.ReadAsStringAsync();
            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(errorBody), Messages.RegistrationFail);
        }
    }
}