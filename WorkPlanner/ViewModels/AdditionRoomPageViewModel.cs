using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkPlanner.Models;
using WorkPlanner.Requests;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class AdditionRoomPageViewModel : BaseViewModel
    {
        public AdditionRoomPageViewModel()
        {
            Request = new RoomRequest();
            AddRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(AddRoom, OnConnectionFailed));
        }

        public RoomRequest Request { get; }

        public Command AddRoomCommand { get; }

        private async Task AddRoom()
        {
            var client = await ServerHelper.GetClientWithToken();

            string requestBody = await ServerHelper.SerializeAsync(Request);
            var result = await client.PostAsync(Settings.AddRoomUrl,
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var room = await ServerHelper.DeserializeAsync<Room>(resultContent);
                MessagingCenter.Send(room, Messages.RoomAdditionSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.RoomAdditionFail);
        }
    }
}