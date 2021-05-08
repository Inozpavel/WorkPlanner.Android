using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WorkPlanner
{
    public static class ServerHelper
    {
        public static HttpClient GetClient() => new()
        {
            Timeout = TimeSpan.FromSeconds(4)
        };

        public static async Task<string> SerializeObjectAsync<T>(T @object)
        {
            string data = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(@object,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            return data;
        }


        public static Action HandleFailedConnectToServer(Func<Task> action, Action onError)
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
    }
}