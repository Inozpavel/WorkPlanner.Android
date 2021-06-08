using System;
using System.Collections.Generic;
using System.Linq;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomInformationPage : ContentPage
    {
        private readonly ToolbarItem _acceptToolbarItem;

        private readonly string _savedRoomDescription;

        private readonly string _savedRoomName;

        private readonly RoomInformationPageViewModel _viewModel;

        public RoomInformationPage(Room room)
        {
            InitializeComponent();
            _acceptToolbarItem = new ToolbarItem("", "accept.png", AcceptOnClicked, ToolbarItemOrder.Primary);
            _savedRoomName = room.RoomName;
            _savedRoomDescription = room.RoomDescription;

            BindingContext = _viewModel = new RoomInformationPageViewModel(room);

            RoomNameEntry.TextChanged += RoomInformationChanged;
            RoomDescriptionEditor.TextChanged += RoomInformationChanged;
            _viewModel.ConnectionFailed += (_, _) =>
            {
                UsersRefreshView.IsRefreshing = false;
                _acceptToolbarItem.IsEnabled = true;
            };

            MessagingCenter.Subscribe<Room>(this, Messages.RoomInformationUpdateSuccess, _ =>
            {
                _acceptToolbarItem.IsEnabled = true;
                ToolbarItems.Clear();
            });

            MessagingCenter.Subscribe<string>(this, Messages.RoomInformationUpdateFail, message =>
            {
                _acceptToolbarItem.IsEnabled = true;
                DisplayAlert(AppResources.Error, message, "Ok");
            });

            MessagingCenter.Subscribe<string>(this, Messages.RoleUpdateFail,
                message => DisplayAlert(AppResources.Error, message, "Ok"));

            MessagingCenter.Subscribe<RoomInformationPageViewModel>(this, Messages.UsersUpdateFail, _ =>
            {
                UsersRefreshView.IsRefreshing = false;
                DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
            });

            MessagingCenter.Subscribe<RoomInformationPageViewModel>(this, Messages.UsersUpdateSuccess,
                _ => UsersRefreshView.IsRefreshing = false);

            MessagingCenter.Subscribe<string>(this, Messages.RoomDeletionFail,
                async message => await DisplayAlert(AppResources.Error, message, "Ok"));

            ChangeUserRoleCommand = new Command(ChangeUserRole);

            ServerHelper.HandleConnectionFailed(this, _viewModel);

            UsersRefreshView.IsRefreshing = true;
            _viewModel.LoadUsersCommand.Execute(this);
        }

        public Command ChangeUserRoleCommand { get; }

        private async void DeleteRoomOnClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert(AppResources.Warning, AppResources.IrrevocableAction,
                AppResources.Yes, AppResources.No))
                return;

            _viewModel.DeleteRoomCommand.Execute(this);
        }

        private void AcceptOnClicked()
        {
            _acceptToolbarItem.IsEnabled = false;
            _viewModel.UpdateRoomCommand.Execute(this);
        }

        private void RoomInformationChanged(object sender, TextChangedEventArgs e)
        {
            if (RoomDescriptionEditor.Text == _savedRoomDescription && RoomNameEntry.Text == _savedRoomName)
            {
                ToolbarItems.Clear();
                return;
            }

            ToolbarItems.Clear();
            ToolbarItems.Add(_acceptToolbarItem);
        }

        private async void ChangeUserRole(object parameter)
        {
            if (parameter is not Guid userId)
                return;

            string roleName = await DisplayActionSheet(AppResources.AvailableRoles, AppResources.Cancel, null,
                AppResources.OwnerRole, AppResources.AdministratorRole, AppResources.MemberRole);
            if (roleName == AppResources.Cancel || roleName == null)
                return;

            var client = ServerHelper.GetClient();
            var result = await client.GetAsync(Settings.AllRolesUrl);
            string content = await result.Content.ReadAsStringAsync();
            var roles = await ServerHelper.DeserializeAsync<List<Role>>(content);

            Guid? roleId = null;
            if (roleName == AppResources.OwnerRole)
                roleId = roles.FirstOrDefault(x => x.RoomRoleName == "Owner")?.RoomRoleId;
            else if (roleName == AppResources.AdministratorRole)
                roleId = roles.FirstOrDefault(x => x.RoomRoleName == "Admin")?.RoomRoleId;
            else if (roleName == AppResources.MemberRole)
                roleId = roles.FirstOrDefault(x => x.RoomRoleName == "Member")?.RoomRoleId;

            if (roleId == null)
            {
                MessagingCenter.Send(AppResources.IncorrectRoleName, Messages.RoleUpdateFail);
                return;
            }

            var updateUserRole = new UpdateUserRole
            {
                UserId = userId,
                RoomRoleId = (Guid) roleId,
                RoomRoleName = roleName
            };
            _viewModel.ChangeRoleCommand.Execute(updateUserRole);
        }
    }
}