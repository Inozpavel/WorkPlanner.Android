using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using WorkPlanner.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class LoginPageViewModel : INotifyPropertyChanged
    {
        private string _password;

        public LoginPageViewModel()
        {
            LoginCommand = new Command(ServerHelper.HandleFailedConnectToServer(SendLoginData,
                () => OnFailedLogin?.Invoke(this, AppResources.ConnectionFailed)));
        }

        public ICommand LoginCommand { get; }

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

        public event EventHandler OnSuccessfulLogin;

        public event EventHandler<string> OnFailedLogin;


        private async Task SendLoginData()
        {
            var loginData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "MobileApp",
                ["username"] = Login,
                ["password"] = Password
            };
            var client = ServerHelper.GetClient();
            var result = await client.PostAsync(Settings.LoginUrl, new FormUrlEncodedContent(loginData));

            if (result.IsSuccessStatusCode)
            {
                var response = JObject.Parse(await result.Content.ReadAsStringAsync())
                    .ToObject<Dictionary<string, string>>();

                string accessToken = response["access_token"];

                if (ShouldRememberUser)
                {
                    try
                    {
                        await SecureStorage.SetAsync(nameof(Login), Login);
                        await SecureStorage.SetAsync(nameof(Password), Password);
                        await SecureStorage.SetAsync(nameof(accessToken), accessToken);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    SecureStorage.RemoveAll();
                }
                

                OnSuccessfulLogin?.Invoke(this, EventArgs.Empty);
                return;
            }

            OnFailedLogin?.Invoke(this, AppResources.IncorrectLoginData);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}