using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkPlanner.Models;
using WorkPlanner.Requests;
using WorkPlanner.Resources;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class AdditionTaskPageViewModel : AdditionViewModel<RoomTask>
    {
        private readonly Room _room;

        public AdditionTaskPageViewModel(Room room)
        {
            _room = room;
            RoomTaskRequest = new RoomTaskRequest();
            AddTaskCommand = new Command(ServerHelper.DecorateFailedConnectToServer(AddTask, OnConnectionFailed));
        }

        public AdditionTaskPageViewModel()
        {
        }

        public RoomTaskRequest RoomTaskRequest { get; }

        public Command AddTaskCommand { get; }

        private async Task AddTask()
        {
            if (RoomTaskRequest.DeadlineTime <= DateTime.Now)
            {
                OnFailedAddition(AppResources.DeadlineError);
                return;
            }

            var client = await ServerHelper.GetClientWithToken();

            var result = await client.PostAsync(string.Format(Settings.AddTaskUrl, _room.RoomId),
                new StringContent(await ServerHelper.SerializeAsync(RoomTaskRequest), Encoding.UTF8,
                    "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var roomTask = await ServerHelper.DeserializeAsync<RoomTask>(resultContent);
                OnSuccessfulAddition(roomTask);
                return;
            }

            OnFailedAddition(ServerHelper.GetErrorFromResponse(resultContent));
        }
    }
}