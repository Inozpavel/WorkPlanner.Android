using System;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfilePageViewModel _viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ProfilePageViewModel();

            _viewModel.SuccessfulUpdate += ViewModelOnSuccessfulUpdate;
            _viewModel.FailedUpdate += ViewModelOnFailedUpdate;
            _viewModel.SuccessfulEmailResent += ViewModelOnSuccessfulEmailResent;
            _viewModel.FailedEmailResent += ViewModelOnFailedEmailResent;

            ServerHelper.HandleConnectionFailed(this, _viewModel);

            _viewModel.UpdateProfileCommand.Execute(this);
        }

        private async void ViewModelOnFailedUpdate(object sender, EventArgs e)
        {
            ProfileRefreshView.IsRefreshing = false;
            await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
        }

        private async void ViewModelOnFailedEmailResent(object sender, EventArgs e) =>
            await DisplayAlert(AppResources.Error, AppResources.EmailResentFailed, "Ok");

        private async void ViewModelOnSuccessfulEmailResent(object sender, EventArgs e) =>
            await DisplayAlert(AppResources.ActionCompleted, AppResources.EmailResentSuccess, "Ok");

        private void ViewModelOnSuccessfulUpdate(object sender, EventArgs e)
        {
            ProfileRefreshView.IsRefreshing = false;

            EmailImage.IsVisible = true;
            EmailImage.Source = _viewModel.IsEmailVerified
                ? ImageSource.FromFile("confirm.png")
                : ImageSource.FromFile("alert.png");
        }

        private async void EmailImage_OnClicked(object sender, EventArgs e)
        {
            if (_viewModel.IsEmailVerified)
                await DisplayAlert(AppResources.EmailConfirmedStatusTitle, AppResources.EmailIsConfirmedStatus, "Ok");
            else
            {
                if (!await DisplayAlert(AppResources.EmailConfirmedStatusTitle,
                    AppResources.EmailIsNotConfirmedStatus, AppResources.Yes, AppResources.No))
                {
                    return;
                }

                _viewModel.ResendConfirmationMailCommand.Execute(this);
            }
        }
    }
}