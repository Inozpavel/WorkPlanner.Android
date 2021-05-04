using System;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        private View _savedContent;

        public RegisterPage()
        {
            InitializeComponent();
            var context = new RegisterPageViewModel();
            BindingContext = context;
            context.OnSendingDataStarted += ContextOnOnSendingDataStarted;
            context.OnRegistrationFailed += ContextOnOnRegistrationFailed;
            context.OnRegistrationSuccess += ContextOnOnRegistrationSuccess;
        }

        private void ContextOnOnSendingDataStarted(object sender, EventArgs e)
        {
            _savedContent = Content;
            Content = new ActivityIndicator
            {
                IsRunning = true
            };
        }

        private async void ContextOnOnRegistrationSuccess(object sender, EventArgs e)
        {
            Content = _savedContent;
            await DisplayAlert(AppResources.SuccessfulRegistrationAlertTitle,
                AppResources.SuccessfulRegistrationAlertMessage, "Ok");
            await Navigation.PopModalAsync();
        }

        private async void ContextOnOnRegistrationFailed(object sender, string errorMessage)
        {
            Content = _savedContent;
            await DisplayAlert(AppResources.FailedRegistrationAlertTitle, errorMessage, "Ok");
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
                return;
            PasswordEntry.IsPassword = !checkBox.IsChecked;
            PasswordConfirmationEntry.IsPassword = !checkBox.IsChecked;
        }
    }
}