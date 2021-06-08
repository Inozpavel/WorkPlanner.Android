using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkPlanner.Models;
using WorkPlanner.Requests;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class ProfilePageViewModel : BaseViewModel
    {
        public ProfilePageViewModel()
        {
            Profile = new Profile();
            LoadProfileCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(LoadProfile, OnConnectionFailed));
            ResendConfirmationMailCommand = new Command(
                ServerHelper.DecorateFailedConnectToServer(ResendConfirmationMail, OnConnectionFailed));
            UpdateProfileCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(UpdateProfile, OnConnectionFailed));
        }

        public Profile Profile { get; }

        public Command LoadProfileCommand { get; }

        public Command UpdateProfileCommand { get; }

        public Command ResendConfirmationMailCommand { get; }

        public event EventHandler SuccessfulEmailResent;

        public event EventHandler FailedEmailResent;

        private async Task LoadProfile()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(Settings.ProfileDataUrl);

            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<Profile>(content);

                Profile.FirstName = model.FirstName;
                Profile.LastName = model.LastName;
                Profile.Patronymic = model.Patronymic;
                Profile.PhoneNumber = model.PhoneNumber;
                Profile.Email = model.Email;
                Profile.IsEmailVerified = model.IsEmailVerified;

                MessagingCenter.Send(model, Messages.ProfileLoadSuccess);
                return;
            }

            MessagingCenter.Send(this, Messages.ProfileLoadFail);
        }

        private async Task UpdateProfile()
        {
            var client = await ServerHelper.GetClientWithToken();

            var updateModel = new UpdateProfileRequest
            {
                LastName = Profile.LastName,
                FirstName = Profile.FirstName,
                Patronymic = Profile.Patronymic,
                PhoneNumber = Profile.PhoneNumber,
            };
            string data = await ServerHelper.SerializeAsync(updateModel);

            var result = await client.PutAsync(Settings.UpdateProfileUrl,
                new StringContent(data, Encoding.UTF8, "application/json"));

            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(this, Messages.ProfileUpdateSuccess);
                return;
            }

            string resultContent = await result.Content.ReadAsStringAsync();
            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.ProfileUpdateFail);
        }

        private async Task ResendConfirmationMail()
        {
            var client = ServerHelper.GetClient();
            client.DefaultRequestHeaders.Add("registeredEmail", Profile.Email);

            var result = await client.GetAsync(Settings.ResendEmailUrl);

            if (result.IsSuccessStatusCode)
            {
                SuccessfulEmailResent?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailedEmailResent?.Invoke(this, EventArgs.Empty);
        }
    }
}