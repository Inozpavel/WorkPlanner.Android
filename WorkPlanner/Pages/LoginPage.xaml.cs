using System;
using System.Text.RegularExpressions;
using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private const string EmailRegex = @"^\S+@\S+$";

        public LoginPage()
        {
            InitializeComponent();
            var context = new LoginPageViewModel();
            BindingContext = context;
            UpdateLoginButtonState();

            context.OnSuccessfulLogin += ContextOnOnSuccessfulLogin;
            context.OnFailedLogin += ContextOnOnFailedLogin;
        }

        private void ContextOnOnFailedLogin(object sender, string message) =>
            DisplayAlert(AppResources.FailedLoginAlertTitle, message, "Ok");

        private async void ContextOnOnSuccessfulLogin(object sender, EventArgs e) =>
            await Navigation.PushAsync(new MainTabbedPage());

        private void CheckCorrectDataEntered(object sender, EventArgs e)
        {
            LoginEntry.TextColor = !Regex.IsMatch(LoginEntry.Text ?? "", EmailRegex) ? Color.Red : Color.Black;
            if (string.IsNullOrWhiteSpace(LoginEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
                return;
            UpdateLoginButtonState();
        }

        private void ChangePasswordVisibility(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
                return;
            PasswordEntry.IsPassword = !checkBox.IsChecked;
        }

        private void UpdateLoginButtonState() =>
            LoginButton.IsEnabled = PasswordEntry.Text?.Length > 0 && LoginEntry.TextColor != Color.Red;

        private void Button_OnClicked(object sender, EventArgs e) => Navigation.PushModalAsync(new RegisterPage());
    }
}