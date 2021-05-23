using WorkPlanner.Resources;
using WorkPlanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPlanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingRoomPage : ContentPage
    {
        public AddingRoomPage()
        {
            InitializeComponent();
            AdditionRoomPageViewModel pageViewModel = new();
            BindingContext = pageViewModel;

            MessagingCenter.Subscribe<string>(this, Messages.RoomAdditionFail, async message =>
                await DisplayAlert(AppResources.Error, message, "Ok"));

            ServerHelper.HandleConnectionFailed(this, pageViewModel);
        }
    }
}