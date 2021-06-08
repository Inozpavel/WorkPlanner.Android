using System;
using System.Linq;
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

        public TasksPage(Room room)
        {
            InitializeComponent();

            _room = room;
            _viewModel = new TasksPageViewModel(room);
            BindingContext = _viewModel;

            _viewModel.ConnectionFailed += (_, _) => TasksRefreshView.IsRefreshing = false;

            MessagingCenter.Subscribe<TasksPageViewModel>(this, Messages.TasksLoadSuccess,
                _ => TasksRefreshView.IsRefreshing = false);

            MessagingCenter.Subscribe<TasksPageViewModel>(this, Messages.TasksLoadFail, async _ =>
            {
                TasksRefreshView.IsRefreshing = false;
                await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
            });

            MessagingCenter.Subscribe<string>(this, Messages.TaskAdditionFail,
                async message => await DisplayAlert(AppResources.Error, message, "Ok"));

            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskAdditionSuccess,
                async _ => await Navigation.PopModalAsync());

            MessagingCenter.Subscribe<Room>(this, Messages.RoomInformationUpdateSuccess, updatedRoom =>
                TasksContentPage.Title = updatedRoom.RoomName);

            ServerHelper.HandleConnectionFailed(this, _viewModel);

            _viewModel.LoadTasksCommand.Execute(this);
        }

        private async void AddTaskOnClicked(object sender, EventArgs e)
        {
            var page = new AddingTaskPage(_room);
            await Navigation.PushModalAsync(page);
        }

        private void CopyRoomIdOnClicked(object sender, EventArgs e) =>
            Clipboard.SetTextAsync(_viewModel.Room.RoomId.ToString());

        private async void InformationOnClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new RoomInformationPage(_room));

        private async void TasksCollectionViewOnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            TaskCollectionView.SelectedItem = null;
            if (e.CurrentSelection.FirstOrDefault() is not RoomTask roomTask)
                return;

            await Navigation.PushAsync(new TaskDetailsPage(_room.RoomId, roomTask.RoomTaskId));
        }
    }
}