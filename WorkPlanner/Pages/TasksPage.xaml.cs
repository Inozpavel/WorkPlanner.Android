using System;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        private readonly Room _room;

        private readonly TasksPageViewModel _viewModel;

        public TasksPage(RoomsPageViewModel roomsPageViewModel, Room room)
        {
            _room = room;
            InitializeComponent();

            _viewModel = new TasksPageViewModel(room);

            _viewModel.SuccessfulLoading += (_, _) => TasksRefreshView.IsRefreshing = false;
            _viewModel.FailedLoading += (_, _) =>
            {
                TasksRefreshView.IsRefreshing = false;
                DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
            };
            _viewModel.SuccessfulDeletion += (_, deletedRoom) =>
            {
                roomsPageViewModel.Rooms.Remove(deletedRoom);
                Navigation.PopAsync();
            };
            _viewModel.FailedDeletion += (_, message) => DisplayAlert(AppResources.Error, message, "Ok");

            _viewModel.LoadTasksCommand.Execute(this);
            BindingContext = _viewModel;
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not RoomTask roomTask)
                return;

            await Navigation.PushAsync(new TaskDetailsPage(roomTask));
        }

        private async void AddTaskOnClicked(object sender, EventArgs e)
        {
            var page = new AddingTaskPage(_viewModel, _room);
            await Navigation.PushAsync(page);
        }

        private async void DeleteRoomOnClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert(AppResources.Warning, AppResources.IrrevocableAction,
                AppResources.Yes, AppResources.No))
            {
                _viewModel.DeleteRoomCommand.Execute(this);
            }
        }

        private void CopyRoomIdOnClicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(_viewModel.Room.RoomId.ToString());
        }
    }
}