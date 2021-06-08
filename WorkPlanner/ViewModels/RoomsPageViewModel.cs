using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
            JoinExistingRoomCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(JoinRoom, OnConnectionFailed));

            MessagingCenter.Subscribe<Room>(this, Messages.RoomAdditionSuccess, room => Rooms.Add(room));

            MessagingCenter.Subscribe<Room>(this, Messages.RoomDeletionSuccess, room => Rooms.Remove(room));

            MessagingCenter.Subscribe<Room>(this, Messages.RoomDeletionFail, room => Rooms.Remove(room));

            MessagingCenter.Subscribe<Room>(this, Messages.RoomInformationUpdateSuccess, updatedRoom =>
            {
                var room = Rooms.FirstOrDefault(x => x.RoomId == updatedRoom.RoomId);
                if (room == null)
                    return;
                int index = Rooms.IndexOf(room);
                if (index >= 0 && index < Rooms.Count)
                    Rooms[index] = room;
            });
        }

        public Command UpdateRoomsCommand { get; }

        public Command JoinExistingRoomCommand { get; }

        public ObservableCollection<Room> Rooms { get; set; } = new();


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

                    MessagingCenter.Send(this, Messages.RoomsUpdateSuccess);
                    return;
                }

                MessagingCenter.Send(this, Messages.RoomsUpdateSuccess);
            }
        }

        private async Task JoinRoom(object parameter)
        {
            if (parameter is not Guid roomId)
            {
                MessagingCenter.Send(AppResources.WrongId, Messages.RoomJoinFail);
                return;
            }

            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.JoiningRoomUrl, roomId));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var room = await ServerHelper.DeserializeAsync<Room>(resultContent);
                Rooms.Insert(0, room);
                MessagingCenter.Send(room, Messages.RoomJoinSuccess);
                return;
            }

            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                MessagingCenter.Send(AppResources.AlreadyRoomMember, Messages.RoomJoinFail);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.RoomJoinFail);
        }
    }
}