using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkPlanner.Models;
using WorkPlanner.Requests;
using WorkPlanner.Resources;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class RoomInformationPageViewModel : BaseViewModel
    {
        public RoomInformationPageViewModel()
        {
        }

        public RoomInformationPageViewModel(Room room)
        {
            Room = room;
            UsersInRoom = new ObservableCollection<UserInRoom>();
            LoadUsersCommand = new Command(ServerHelper.DecorateFailedConnectToServer(UpdateUsers, OnConnectionFailed));
            DeleteRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(DeleteRoom, OnConnectionFailed));
            UpdateRoomCommand = new Command(ServerHelper.DecorateFailedConnectToServer(UpdateRoom, OnConnectionFailed));
            ChangeRoleCommand = new Command(ServerHelper.DecorateFailedConnectToServer(ChangeRole, OnConnectionFailed));

            MessagingCenter.Subscribe<UpdateUserRole>(this, Messages.RoleUpdateSuccess, updatedUserRole =>
            {
                var userInRoom = UsersInRoom.FirstOrDefault(x => x.User.UserId == updatedUserRole.UserId);
                if (userInRoom != null)
                {
                    userInRoom.RoleName = updatedUserRole.RoomRoleName;
                    userInRoom.RoleId = updatedUserRole.RoomRoleId;
                }

                int index = UsersInRoom.IndexOf(userInRoom);
                if (index > 0 && index < UsersInRoom.Count)
                    UsersInRoom[index] = userInRoom;
            });
        }

        public Room Room { get; }

        public ObservableCollection<UserInRoom> UsersInRoom { get; }

        public Command LoadUsersCommand { get; }

        public Command DeleteRoomCommand { get; }

        public Command UpdateRoomCommand { get; }

        public Command ChangeRoleCommand { get; }

        private async Task ChangeRole(object parameter)
        {
            if (parameter is not UpdateUserRole updateUserRole)
                return;

            var client = await ServerHelper.GetClientWithToken();

            string data = await ServerHelper.SerializeAsync(updateUserRole);
            var result = await client.PutAsync(string.Format(Settings.ChangeRoleUrl, Room.RoomId),
                new StringContent(data, Encoding.UTF8, "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(updateUserRole, Messages.RoleUpdateSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.RoleUpdateFail);
        }

        private async Task UpdateUsers()
        {
            var client = await ServerHelper.GetClientWithToken();

            var rolesResult = await client.GetAsync(Settings.AllRolesUrl);

            string rolesContent = await rolesResult.Content.ReadAsStringAsync();
            var roles = await ServerHelper.DeserializeAsync<List<Role>>(rolesContent);

            var usersResult = await client.GetAsync(string.Format(Settings.UsersInRoomUrl, Room.RoomId));

            string resultContent = await usersResult.Content.ReadAsStringAsync();

            if (usersResult.IsSuccessStatusCode)
            {
                var usersInRoom = await ServerHelper.DeserializeAsync<List<UserInRoom>>(resultContent);

                if (usersInRoom is {Count: > 0})
                {
                    foreach (var userInRoom in usersInRoom)
                    {
                        string roleName = roles.FirstOrDefault(x => x.RoomRoleId == userInRoom.RoleId)?.RoomRoleName;

                        userInRoom.RoleName = roleName switch
                        {
                            "Owner" => AppResources.OwnerRole,
                            "Admin" => AppResources.AdministratorRole,
                            "Member" => AppResources.MemberRole,
                            _ => roleName
                        };
                    }

                    UsersInRoom.Clear();
                    foreach (var userInRoom in usersInRoom)
                        UsersInRoom.Add(userInRoom);
                }

                MessagingCenter.Send(this, Messages.UsersUpdateSuccess);
                return;
            }

            MessagingCenter.Send(this, Messages.UsersUpdateFail);
        }

        private async Task DeleteRoom()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.DeleteAsync(string.Format(Settings.DeleteRoomUrl, Room.RoomId));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(Room, Messages.RoomDeletionSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.RoomDeletionFail);
        }

        private async Task UpdateRoom()
        {
            var client = await ServerHelper.GetClientWithToken();

            var room = new RoomRequest
            {
                RoomName = Room.RoomName,
                RoomDescription = Room.RoomDescription
            };
            string data = await ServerHelper.SerializeAsync(room);

            var result = await client.PutAsync(string.Format(Settings.UpdateRoomUrl, Room.RoomId),
                new StringContent(data, Encoding.UTF8, "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(Room, Messages.RoomInformationUpdateSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.RoomInformationUpdateFail);
        }
    }
}