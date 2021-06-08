using System;
using System.Linq;
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

            MessagingCenter.Subscribe<RoomsPageViewModel>(this, Messages.RoomsUpdateSuccess,
                _ => RoomsRefreshView.IsRefreshing = false);

            MessagingCenter.Subscribe<RoomsPageViewModel>(this, Messages.RoomsUpdateFail,
                async _ =>
                {
                    RoomsRefreshView.IsRefreshing = false;
                    await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
                });

            MessagingCenter.Subscribe<Room>(this, Messages.RoomAdditionSuccess,
                async _ => await Navigation.PopModalAsync());

            MessagingCenter.Subscribe<string>(this, Messages.RoomJoinFail,
                async message => await DisplayAlert(AppResources.Error, message, "Ok"));

            MessagingCenter.Subscribe<Room>(this, Messages.RoomDeletionSuccess, async _ =>
            {
                await Navigation.PopAsync();
                await Navigation.PopAsync();
            });

            ServerHelper.HandleConnectionFailed(this, _viewModel);

            _viewModel.ConnectionFailed += (_, _) => RoomsRefreshView.IsRefreshing = false;

            RoomsRefreshView.IsRefreshing = true;
            _viewModel.UpdateRoomsCommand.Execute(this);
        }

        private async void AddRoomOnClicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet(AppResources.SelectAnAction, AppResources.Cancel, null,
                AppResources.CreateNewRoom, AppResources.JoinAnExistingRoom);
            if (result == AppResources.CreateNewRoom)
                await Navigation.PushModalAsync(new AddingRoomPage());
            else if (result == AppResources.JoinAnExistingRoom)
            {
                string id = await DisplayPromptAsync(AppResources.JoiningAnExistingRoom,
                    AppResources.EnterRoomId, cancel: AppResources.Cancel, maxLength: 36);
                if (id == null || id == AppResources.Cancel)
                    return;
                if (!Guid.TryParse(id, out var guidId))
                {
                    await DisplayAlert(AppResources.Error, AppResources.WrongId, "Ok");
                    return;
                }

                _viewModel.JoinExistingRoomCommand.Execute(guidId);
            }
        }

        private void CollectionViewOnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            RoomsCollectionView.SelectedItem = null;

            if (e.CurrentSelection.FirstOrDefault() is not Room selectedRoom)
                return;
            Navigation.PushAsync(new TasksPage(selectedRoom));
        }
    }
}