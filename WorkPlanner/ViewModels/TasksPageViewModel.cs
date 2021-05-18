using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkPlanner.Models;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class TasksPageViewModel : DeletionViewModel<Room>
    {
        public TasksPageViewModel()
        {
        }


        public TasksPageViewModel(Room room)
        {
            Room = room;
            LoadTasksCommand = new Command(ServerHelper.DecorateFailedConnectToServer(Load, OnConnectionFailed));
            DeleteRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(DeleteRoom, OnConnectionFailed));
        }

        public Room Room { get; }

        public ObservableCollection<RoomTask> Tasks { get; } = new();

        public Command LoadTasksCommand { get; }

        public Command DeleteRoomCommand { get; }

        public event EventHandler SuccessfulLoading;

        public event EventHandler FailedLoading;

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
                    Tasks.Clear();
                    foreach (var roomTask in rooms)
                    {
                        Tasks.Add(roomTask);
                    }
                }

                SuccessfulLoading?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailedLoading?.Invoke(this, EventArgs.Empty);
        }

        private async Task DeleteRoom()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.DeleteAsync(string.Format(Settings.DeleteRoomUrl, Room.RoomId));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                OnSuccessfulDeletion(Room);
                return;
            }

            OnFailedDeletion(ServerHelper.GetErrorFromValidationResult(resultContent));
        }
    }
}