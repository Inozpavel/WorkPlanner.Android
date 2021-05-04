namespace WorkPlanner
{
    public static class Settings
    {
        private const string UrlBase = "http://10.0.2.2:5000/gateway/";

        private const string AccountsUrlBase = UrlBase + "identity/accounts/";

        public const string RegisterUrl = AccountsUrlBase + "register/";
    }
}