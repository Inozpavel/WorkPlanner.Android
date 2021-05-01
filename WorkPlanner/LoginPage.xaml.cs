using System;
using System.Text.RegularExpressions;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private const string EmailRegex = @"^\S+@\S+$";

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel(Navigation);
            UpdateLoginButtonState();
        }

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
    }
}