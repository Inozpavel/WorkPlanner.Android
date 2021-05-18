using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingRoomPage : ContentPage
    {
        public AddingRoomPage(RoomsPageViewModel roomsPageViewModel)
        {
            InitializeComponent();
            AdditionRoomPageViewModel pageViewModel = new();
            BindingContext = pageViewModel;

            pageViewModel.SuccessfulAddition += (_, request) =>
            {
                roomsPageViewModel.Rooms.Add(request);
                Navigation.PopModalAsync();
            };

            pageViewModel.FailedAddition += async (_, message) =>
                await DisplayAlert(AppResources.Error, message, "Ok");

            ServerHelper.HandleConnectionFailed(this, pageViewModel);
        }
    }
}