using System.Globalization;
using WorkPlanner.Pages;
using WorkPlanner.Resources;
using Xamarin.Forms;

namespace WorkPlanner
{
    public partial class App : Application
    {
        public App()
        {
            AppResources.Culture = CultureInfo.CurrentUICulture;
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}