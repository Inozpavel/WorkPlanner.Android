using System;
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
                new Command(ServerHelper.DecorateFailedConnectToServer(Test, OnConnectionFailed));

        }

        public Command UpdateRoomsCommand { get; set; }

        public Command JoinExistingRoomCommand { get; set; }

        public ObservableCollection<Room> Rooms { get; set; } = new();

        public event EventHandler SuccessfulUpdate;

        public event EventHandler FailedUpdate;

        public event EventHandler<Room> SuccessfulJoin;

        public event EventHandler<string> FailedJoin;

        private async Task Test()
        {
            var client = await ServerHelper.GetClientWithToken();
            var result = await client.GetAsync(Settings.AllRoomsUrl);

            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                Rooms.Clear();
                var rooms = JsonConvert.DeserializeObject<ObservableCollection<Room>>(content);
                foreach (var room in rooms)
                {
                    Rooms.Add(room);
                }

                SuccessfulUpdate?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailedUpdate?.Invoke(this, EventArgs.Empty);
        }

        private async Task Join(object parameter)
        {
            if (parameter is not Guid guid)
                OnFailedJoin(AppResources.WrongId);
        }

        private void OnFailedJoin(string message) => FailedJoin?.Invoke(this, message);
        
        private void OnSuccessfulJoin(Room room) => SuccessfulJoin?.Invoke(this, room);
    }
}