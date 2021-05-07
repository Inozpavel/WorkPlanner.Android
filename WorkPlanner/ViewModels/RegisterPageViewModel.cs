﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using WorkPlanner.Requests;
using WorkPlanner.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public sealed class RegisterPageViewModel : INotifyPropertyChanged
    {
        public RegisterPageViewModel()
        {
            Request = new RegisterUserRequest();
            RegisterCommand = new Command(ServerHelper.HandleFailedConnectToServer(Register,
                () => OnRegistrationFailed?.Invoke(this, AppResources.ConnectionFailed)));
        }

        public ICommand RegisterCommand { get; }

        public RegisterUserRequest Request { get; }

        public event PropertyChangedEventHandler PropertyChanged;

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

            string data = await ServerHelper.SerializeObjectAsync(Request);

            var result = await ServerHelper.GetClient().PostAsync(Settings.RegisterUrl,
                new StringContent(data, Encoding.UTF8, "application/json"));

            if (result.StatusCode == HttpStatusCode.OK)
                OnRegistrationSuccess?.Invoke(this, EventArgs.Empty);
            else
            {
                var errorBody = JObject.Parse(await result.Content.ReadAsStringAsync());
                var errors = errorBody["errors"].ToObject<Dictionary<string, List<string>>>();
                foreach (var keyValuePair in errors)
                {
                    OnRegistrationFailed?.Invoke(this, keyValuePair.Value.First());
                    return;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}