using System;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingTaskPage : ContentPage
    {
        public AddingTaskPage(TasksPageViewModel tasksPageViewModel, Room room)
        {
            InitializeComponent();
            AdditionTaskPageViewModel viewModel = new(room);
            BindingContext = viewModel;


            viewModel.SuccessfulAddition += (_, task) =>
            {
                tasksPageViewModel.Tasks.Insert(0, task);
                Navigation.PopAsync();
            };
            viewModel.FailedAddition += async (_, message) => await DisplayAlert(AppResources.Error, message, "Ok");
            ServerHelper.HandleConnectionFailed(this, viewModel);
        }

        private void DatePicker_OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is not DatePicker datePicker)
                return;
            datePicker.MinimumDate = DateTime.Today;
        }
    }
}