using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkPlanner.Models;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class TasksPageViewModel : BaseViewModel
    {
        public TasksPageViewModel()
        {
        }


        public TasksPageViewModel(Room room)
        {
            Room = room;
            LoadTasksCommand = new Command(ServerHelper.DecorateFailedConnectToServer(Load, OnConnectionFailed));

            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskDeleted, sender => RoomTasks.Remove(sender));

            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskAdditionSuccess, task => RoomTasks.Add(task));
        }

        public Room Room { get; }

        public ObservableCollection<RoomTask> RoomTasks { get; } = new();

        public Command LoadTasksCommand { get; }

        private async Task Load()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.AllTasksUrl, Room.RoomId));

            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var rooms = JsonConvert.DeserializeObject<List<RoomTask>>(resultContent);

                if (rooms is {Count: > 0})
                {
                    RoomTasks.Clear();
                    foreach (var roomTask in rooms)
                    {
                        RoomTasks.Add(roomTask);
                    }
                }

                MessagingCenter.Send(this, Messages.TasksUpdateSuccess);
                return;
            }

            MessagingCenter.Send(this, Messages.TasksUpdateFail);
        }
    }
}