using System;
using WorkPlanner.Models;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private readonly ToolbarItem _acceptToolbarItem;

        private readonly ProfilePageViewModel _viewModel;

        private string _savedFirstName;

        private string _savedLastName;

        private string _savedPatronymic;

        private string _savedPhoneNumber;

        public ProfilePage()
        {
            InitializeComponent();
            _acceptToolbarItem = new ToolbarItem("", "accept.png", AcceptOnClicked, ToolbarItemOrder.Primary);
            BindingContext = _viewModel = new ProfilePageViewModel();

            MessagingCenter.Subscribe<ProfilePageViewModel>(this, Messages.ProfileUpdateSuccess, _ =>
            {
                _acceptToolbarItem.IsEnabled = true;
                ToolbarItems.Clear();
            });

            MessagingCenter.Subscribe<string>(this, Messages.ProfileUpdateFail, async message =>
            {
                _acceptToolbarItem.IsEnabled = true;
                await DisplayAlert(AppResources.Error, message, "Ok");
            });

            MessagingCenter.Subscribe<ProfilePageViewModel>(this, Messages.ProfileLoadFail, async _ =>
            {
                ProfileRefreshView.IsRefreshing = false;
                await DisplayAlert(AppResources.Error, AppResources.UpdateFailed, "Ok");
            });

            MessagingCenter.Subscribe<Profile>(this, Messages.ProfileLoadSuccess, profile =>
            {
                ProfileRefreshView.IsRefreshing = false;

                _savedLastName = profile.LastName;
                _savedFirstName = profile.FirstName;
                _savedPatronymic = profile.Patronymic;
                _savedPhoneNumber = profile.PhoneNumber;

                UpdateAcceptButton();
                EmailImage.IsVisible = true;
                EmailImage.Source = _viewModel.Profile.IsEmailVerified
                    ? ImageSource.FromFile("confirm.png")
                    : ImageSource.FromFile("alert.png");
            });

            _viewModel.SuccessfulEmailResent += ViewModelOnSuccessfulEmailResent;
            _viewModel.FailedEmailResent += ViewModelOnFailedEmailResent;

            ServerHelper.HandleConnectionFailed(this, _viewModel);

            _viewModel.ConnectionFailed += (_, _) =>
            {
                ProfileRefreshView.IsRefreshing = false;
                _acceptToolbarItem.IsEnabled = true;
            };

            ProfileRefreshView.IsRefreshing = true;
            _viewModel.LoadProfileCommand.Execute(this);

            LastNameEntry.TextChanged += (_, _) => UpdateAcceptButton();
            FirstNameEntry.TextChanged += (_, _) => UpdateAcceptButton();
            PatronymicEntry.TextChanged += (_, _) => UpdateAcceptButton();
            PhoneNumberEntry.TextChanged += (_, _) => UpdateAcceptButton();
        }

        private void UpdateAcceptButton()
        {
            if (LastNameEntry.Text == _savedLastName && FirstNameEntry.Text == _savedFirstName &&
                PatronymicEntry.Text == _savedPatronymic && PhoneNumberEntry.Text == _savedPhoneNumber)
            {
                ToolbarItems.Clear();
                return;
            }

            ToolbarItems.Clear();
            ToolbarItems.Add(_acceptToolbarItem);
        }

        private void AcceptOnClicked()
        {
            _acceptToolbarItem.IsEnabled = false;
            _viewModel.UpdateProfileCommand.Execute(this);
        }

        private async void ViewModelOnFailedEmailResent(object sender, EventArgs e) =>
            await DisplayAlert(AppResources.Error, AppResources.EmailResentFailed, "Ok");

        private async void ViewModelOnSuccessfulEmailResent(object sender, EventArgs e) =>
            await DisplayAlert(AppResources.ActionCompleted, AppResources.EmailResentSuccess, "Ok");

        private async void EmailImageOnClicked(object sender, EventArgs e)
        {
            if (_viewModel.Profile.IsEmailVerified)
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