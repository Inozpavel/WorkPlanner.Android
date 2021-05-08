using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkPlanner.Models;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class RoomPageViewModel : BaseViewModel
    {
        public RoomPageViewModel() => UpdateRoomsCommand =
            new Command(ServerHelper.DecorateFailedConnectToServer(Test, OnConnectionFailed));

        public Command UpdateRoomsCommand { get; set; }

        public ObservableCollection<Room> Rooms { get; set; } = new();

        public event EventHandler SuccessfulUpdate;

        public event EventHandler FailedUpdate;

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
    }
}