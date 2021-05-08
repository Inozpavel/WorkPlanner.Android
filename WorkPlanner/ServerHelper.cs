using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkPlanner
{
    public static class ServerHelper
    {
        public static HttpClient GetClient() => new()
        {
            Timeout = TimeSpan.FromSeconds(4)
        };

        public static async Task<HttpClient> GetClientWithToken()
        {
            var client = GetClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await SecureStorage.GetAsync("accessToken"));
            return client;
        }


        public static async Task<string> SerializeObjectAsync<T>(T @object)
        {
            string data = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(@object,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            return data;
        }


        public static Action DecorateFailedConnectToServer(Func<Task> action, Action onError)
        {
            return async () =>
            {
                try
                {
                    await action();
                }
                catch (Exception e)
                {
                    if (e.Message == "Socket closed" || e.GetType() == typeof(OperationCanceledException) ||
                        e.GetType() == typeof(WebException))
                    {
                        onError?.Invoke();
                    }
                    else throw;
                }
            };
        }

        public static void HandleConnectionFailed(ContentPage page, BaseViewModel viewModel)
        {
            viewModel.ConnectionFailed += (_, _) =>
                page.DisplayAlert(AppResources.Error, AppResources.ConnectionFailed, "Ok");
        }
    }
}