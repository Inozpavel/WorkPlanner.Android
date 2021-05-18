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
        private readonly TaskDetailsPageViewModel _viewModel;

        public TaskDetailsPage(RoomTask roomTask)
        {
            InitializeComponent();
            _viewModel = new TaskDetailsPageViewModel(roomTask);
            BindingContext = _viewModel;

            UpdateTimeToEnd();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateTimeToEnd();

                return true;
            });
        }

        private void UpdateTimeToEnd()
        {
            var timeToEnd = _viewModel.RoomTask.DeadlineTime - DateTime.Now;

            var timeSpan = new TimeSpan(timeToEnd.Days, timeToEnd.Hours, timeToEnd.Minutes, timeToEnd.Seconds);
            DeadlineTimerLabel.Text = timeToEnd.Ticks > 0
                ? $"{timeSpan:g}"
                : AppResources.TimeEnded;
        }
    }
}