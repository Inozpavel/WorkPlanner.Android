using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class ProfilePageViewModel : BaseViewModel
    {
        private string _email;

        private string _firstName;

        private bool _isEmailVerified;

        private string _lastName;

        private string _patronymic;

        private string _phoneNumber;

        public ProfilePageViewModel()
        {
            UpdateProfileCommand = new Command(ServerHelper.DecorateFailedConnectToServer(UpdateProfileData, () =>
            {
                OnConnectionFailed();
                OnFailedUpdate();
            }));
            ResendConfirmationMailCommand = new Command(
                ServerHelper.DecorateFailedConnectToServer(ResendConfirmationMail, OnConnectionFailed));
        }

        [JsonProperty("first_name")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Patronymic
        {
            get => _patronymic;
            set
            {
                _patronymic = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("phone_number")]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("email_verified")]
        public bool IsEmailVerified
        {
            get => _isEmailVerified;
            set
            {
                _isEmailVerified = value;
                OnPropertyChanged();
            }
        }

        public Command UpdateProfileCommand { get; }

        public Command ResendConfirmationMailCommand { get; }

        public event EventHandler SuccessfulUpdate;

        public event EventHandler FailedUpdate;

        public event EventHandler SuccessfulEmailResent;

        public event EventHandler FailedEmailResent;

        private async Task UpdateProfileData()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(Settings.ProfileDataUrl);

            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ProfilePageViewModel>(content);

                FirstName = model.FirstName;
                LastName = model.LastName;
                Patronymic = model.Patronymic;
                PhoneNumber = model.PhoneNumber;
                Email = model.Email;
                IsEmailVerified = model.IsEmailVerified;
                OnSuccessfulUpdate();
                return;
            }

            OnFailedUpdate();
        }

        private async Task ResendConfirmationMail()
        {
            var client = ServerHelper.GetClient();
            client.DefaultRequestHeaders.Add("registeredEmail", Email);

            var result = await client.GetAsync(Settings.ResendEmailUrl);

            if (result.IsSuccessStatusCode)
            {
                SuccessfulEmailResent?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailedEmailResent?.Invoke(this, EventArgs.Empty);
        }


        private void OnSuccessfulUpdate() => SuccessfulUpdate?.Invoke(this, EventArgs.Empty);

        private void OnFailedUpdate() => FailedUpdate?.Invoke(this, EventArgs.Empty);
    }
}