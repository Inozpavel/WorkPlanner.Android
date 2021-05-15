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
    public class AddRoomPageViewModel : BaseViewModel
    {
        public AddRoomPageViewModel()
        {
            Request = new RoomRequest();
            AddRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(Send, OnConnectionFailed));
        }

        public RoomRequest Request { get; }

        public Command AddRoomCommand { get; }

        public event EventHandler<Room> SuccessfulAddition;

        public event EventHandler<string> FailedAddition;

        private async Task Send()
        {
            var client = await ServerHelper.GetClientWithToken();

            string requestBody = await ServerHelper.SerializeObjectAsync(Request);
            var result = await client.PostAsync(Settings.AddRoomUrl,
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var room = JsonConvert.DeserializeObject<Room>(resultContent);
                SuccessfulAddition?.Invoke(this, room);
                return;
            }

            FailedAddition?.Invoke(this, ServerHelper.GetErrorFromValidationResult(resultContent));
        }
    }
}