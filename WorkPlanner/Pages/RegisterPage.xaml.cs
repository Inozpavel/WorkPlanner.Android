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
            RegisterPageViewModel viewModel = new();


            MessagingCenter.Subscribe<RegisterPageViewModel>(this, Messages.RegistrationSuccess, async _ =>
            {
                Content = _savedContent;
                await DisplayAlert(AppResources.SuccessfulRegistrationAlertTitle,
                    AppResources.SuccessfulRegistrationAlertMessage, "Ok");
                await Navigation.PopModalAsync();
            });
            MessagingCenter.Subscribe<string>(this, Messages.RegistrationFail, async message =>
            {
                Content = _savedContent;
                await DisplayAlert(AppResources.Error, message, "Ok");
            });
            MessagingCenter.Subscribe<RegisterPageViewModel>(this, Messages.ActionStarted, _ =>
            {
                _savedContent = Content;
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Color.Orange
                };
            });
            viewModel.ConnectionFailed += (_, _) => Content = _savedContent;

            ServerHelper.HandleConnectionFailed(this, viewModel);

            BindingContext = viewModel;
        }

        private void CheckBoxOnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
                return;
            PasswordEntry.IsPassword = !checkBox.IsChecked;
            PasswordConfirmationEntry.IsPassword = !checkBox.IsChecked;
        }
    }
}