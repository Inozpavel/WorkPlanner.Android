using System;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomsPage : ContentPage
    {
        public RoomsPage()
        {
            RoomPageViewModel viewModel;
            InitializeComponent();
            BindingContext = viewModel = new RoomPageViewModel();

            viewModel.SuccessfulUpdate += (_, _) => { RoomsRefreshView.IsRefreshing = false; };
            viewModel.FailedUpdate += ViewModelOnFailedUpdate;

            ServerHelper.HandleConnectionFailed(this, viewModel);
        }

        private async void ViewModelOnFailedUpdate(object sender, EventArgs e)
        {
            RoomsRefreshView.IsRefreshing = false;
            await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
        }

        private void MenuItem_OnClicked(object sender, EventArgs e) => Navigation.PushModalAsync(new ContentPage());
    }
}