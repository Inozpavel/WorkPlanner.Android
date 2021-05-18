using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class RoomsPageViewModel : BaseViewModel
    {
        public RoomsPageViewModel()
        {
            UpdateRoomsCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(UpdateRooms, OnConnectionFailed));
            JoinExistingRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(JoinRoom, OnConnectionFailed));
        }

        public Command UpdateRoomsCommand { get; }

        public Command JoinExistingRoomCommand { get; }

        public ObservableCollection<Room> Rooms { get; set; } = new();

        public event EventHandler SuccessfulUpdate;

        public event EventHandler FailedUpdate;

        public event EventHandler<string> FailedJoin;

        private async Task UpdateRooms()
        {
            var client = await ServerHelper.GetClientWithToken();
            var result = await client.GetAsync(Settings.AllRoomsUrl);

            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(content);

                if (rooms is {Count: > 0})
                {
                    Rooms.Clear();
                    foreach (var room in rooms)
                        Rooms.Add(room);
                }

                SuccessfulUpdate?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailedUpdate?.Invoke(this, EventArgs.Empty);
        }

        private async Task JoinRoom(object parameter)
        {
            if (parameter is not Guid roomId)
                OnFailedJoin(AppResources.WrongId);

            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.JoiningRoomUrl, roomId));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var room = await ServerHelper.DeserializeAsync<Room>(resultContent);
                Rooms.Insert(0, room);
            }

            OnFailedJoin(AppResources.WrongId);
        }

        private void OnFailedJoin(string message) => FailedJoin?.Invoke(this, message);
    }
}