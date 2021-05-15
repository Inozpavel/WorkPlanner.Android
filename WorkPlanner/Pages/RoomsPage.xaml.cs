using System;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomsPage : ContentPage
    {
        private readonly RoomPageViewModel _viewModel;

        public RoomsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RoomPageViewModel();

            _viewModel.SuccessfulUpdate += (_, _) => { RoomsRefreshView.IsRefreshing = false; };
            _viewModel.FailedUpdate += ViewModelOnFailedUpdate;
            _viewModel.UpdateRoomsCommand.Execute(this);
            ServerHelper.HandleConnectionFailed(this, _viewModel);
        }

        private async void ViewModelOnFailedUpdate(object sender, EventArgs e)
        {
            RoomsRefreshView.IsRefreshing = false;
            await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
        }

        private void MenuItem_OnClicked(object sender, EventArgs e) =>
            Navigation.PushModalAsync(new AddRoomPage(_viewModel));

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Room selectedRoom)
                return;
            Navigation.PushAsync(new ContentPage());
        }
    }
}