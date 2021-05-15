using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddRoomPage : ContentPage
    {
        public AddRoomPage(RoomPageViewModel roomPageViewModel)
        {
            InitializeComponent();
            AddRoomPageViewModel pageViewModel = new();
            BindingContext = pageViewModel;

            pageViewModel.SuccessfulAddition += (_, request) =>
            {
                roomPageViewModel.Rooms.Add(request);
                Navigation.PopModalAsync();
            };

            pageViewModel.FailedAddition += async (_, message) =>
                await DisplayAlert(AppResources.Error, message, "Ok");

            ServerHelper.HandleConnectionFailed(this, pageViewModel);
        }
    }
}