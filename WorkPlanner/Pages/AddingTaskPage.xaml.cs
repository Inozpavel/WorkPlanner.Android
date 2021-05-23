using System;
using WorkPlanner.Models;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingTaskPage : ContentPage
    {
        public AddingTaskPage(Room room)
        {
            InitializeComponent();
            AdditionTaskPageViewModel viewModel = new(room);
            BindingContext = viewModel;

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