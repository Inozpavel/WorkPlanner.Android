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
        private readonly RoomsPageViewModel _viewModel;

        public RoomsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RoomsPageViewModel();

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

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Room selectedRoom)
                return;
            Navigation.PushAsync(new TasksPage(_viewModel, selectedRoom));
        }

        private async void AddRoomOnClicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet(AppResources.SelectAnAction, AppResources.Cancel, null,
                AppResources.CreateNewRoom, AppResources.JoinAnExistingRoom);
            if (result == AppResources.CreateNewRoom)
                await Navigation.PushModalAsync(new AddingRoomPage(_viewModel));
            else if (result == AppResources.JoinAnExistingRoom)
            {
                string id = await DisplayPromptAsync(AppResources.JoiningAnExistingRoom,
                    AppResources.EnterRoomId, cancel: AppResources.Cancel, maxLength: 36);
                if (id == null || id == AppResources.Cancel)
                    return;
                if (!Guid.TryParse(id, out var guidId))
                    await DisplayAlert(AppResources.Error, AppResources.WrongId, "Ok");
            }
        }
    }
}