using System;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskDetailsPage : ContentPage
    {
        private readonly ToolbarItem _acceptToolbarItem;

        private readonly TaskDetailsPageViewModel _viewModel;

        private string _savedTaskContent;

        private string _savedTaskDetails;

        private string _savedTaskName;

        public TaskDetailsPage(Guid roomId, Guid roomTaskId)
        {
            InitializeComponent();
            _acceptToolbarItem = new ToolbarItem("", "accept.png", AcceptOnClicked, ToolbarItemOrder.Primary);

            _viewModel = new TaskDetailsPageViewModel(roomId, roomTaskId);
            BindingContext = _viewModel;

            UpdateTimeToEnd();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateTimeToEnd();
                return true;
            });

            MessagingCenter.Subscribe<TaskDetailsPageViewModel>(this, Messages.TaskLoadFail, async _ =>
            {
                TaskDetailsRefreshView.IsRefreshing = false;
                await DisplayAlert(AppResources.Error, AppResources.LoadFail, "Ok");
                await Navigation.PopAsync();
            });
            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskLoadSuccess, roomTask =>
            {
                _savedTaskName = roomTask.TaskName;
                _savedTaskContent = roomTask.TaskContent;
                _savedTaskDetails = roomTask.Details;
                TaskDetailsRefreshView.IsRefreshing = false;

                UpdateAcceptButton();
            });

            MessagingCenter.Subscribe<string>(this, Messages.TaskUpdateFail, async message =>
            {
                _acceptToolbarItem.IsEnabled = true;
                await DisplayAlert(AppResources.Error, message, "OK");
            });

            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskUpdateSuccess, _ =>
            {
                _acceptToolbarItem.IsEnabled = true;
                ToolbarItems.Clear();
            });

            MessagingCenter.Subscribe<User>(this, Messages.TaskCreatorLoadSuccess, async user =>
                await DisplayAlert(AppResources.CreatorInformation,
                    string.Format(AppResources.TaskIsCreatedBy, user.LastName, user.FirstName, user.Patronymic), "Ok"));

            MessagingCenter.Subscribe<string>(this, Messages.TaskCreatorLoadFail, async message =>
                await DisplayAlert(AppResources.Error, message, "Ok"));

            MessagingCenter.Subscribe<RoomTask>(this, Messages.TaskDeletionSuccess, async _ =>
                await Navigation.PopAsync());

            MessagingCenter.Subscribe<string>(this, Messages.TaskDeletionFail, async message =>
                await DisplayAlert(AppResources.Error, message, "Ok"));

            ServerHelper.HandleConnectionFailed(this, _viewModel);
            _viewModel.ConnectionFailed += (_, _) =>
            {
                TaskDetailsRefreshView.IsRefreshing = false;
                _acceptToolbarItem.IsEnabled = true;
            };
            _viewModel.LoadTaskCommand.Execute(this);

            NameEditor.TextChanged += (_, _) => UpdateAcceptButton();
            ContentEditor.TextChanged += (_, _) => UpdateAcceptButton();
            DetailsEditor.TextChanged += (_, _) => UpdateAcceptButton();
        }

        private void AcceptOnClicked()
        {
            _acceptToolbarItem.IsEnabled = false;
            _viewModel.UpdateTaskCommand.Execute(this);
        }

        private void UpdateTimeToEnd()
        {
            var timeToEnd = _viewModel.RoomTask.DeadlineTime - DateTime.Now;

            var timeSpan = new TimeSpan(timeToEnd.Days, timeToEnd.Hours, timeToEnd.Minutes, timeToEnd.Seconds);
            DeadlineTimerLabel.Text = timeToEnd.Ticks > 0
                ? $"{timeSpan:g}"
                : AppResources.TimeEnded;
        }

        private void UpdateAcceptButton()
        {
            if (ContentEditor.Text == _savedTaskContent && NameEditor.Text == _savedTaskName &&
                DetailsEditor.Text == _savedTaskDetails)
            {
                ToolbarItems.Clear();
                return;
            }

            ToolbarItems.Clear();
            ToolbarItems.Add(_acceptToolbarItem);
        }

        private async void DeleteTaskClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert(AppResources.Warning, AppResources.IrrevocableAction,
                AppResources.Yes, AppResources.No))
                return;
            _viewModel.DeleteTaskCommand.Execute(this);
        }

        private async void ConfirmTaskClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert(AppResources.Warning, AppResources.ConfirmTaskCompleted,
                AppResources.Yes, AppResources.No))
                return;
            _viewModel.CompleteTaskCommand.Execute(this);
        }
    }
}