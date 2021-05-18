using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            Timeout = TimeSpan.FromSeconds(6)
        };

        public static async Task<HttpClient> GetClientWithToken()
        {
            var client = GetClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await SecureStorage.GetAsync("accessToken"));
            return client;
        }

        public static string GetErrorFromValidationResult(string validationResult)
        {
            try
            {
                var errors = JObject.Parse(validationResult)["errors"].ToObject<Dictionary<string, List<string>>>();
                return errors.FirstOrDefault().Value.FirstOrDefault();
            }
            catch (Exception)
            {
                return "Failed to get error!";
            }
        }


        public static async Task<string> SerializeAsync<T>(T @object)
        {
            string data = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(@object,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            return data;
        }

        public static async Task<T> DeserializeAsync<T>(string content) where T : class
        {
            try
            {
                var data = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(content));
                return data;
            }
            catch (Exception)
            {
                return null;
            }
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