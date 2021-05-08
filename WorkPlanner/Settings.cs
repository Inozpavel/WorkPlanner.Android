namespace WorkPlanner
{
    public static class Settings
    {
        private const string UrlBase = "http://10.0.2.2:5000/gateway/";

        private const string AccountsUrlBase = UrlBase + "users-api/accounts/";

        public const string RegisterUrl = AccountsUrlBase + "register";

        public const string LoginUrl = UrlBase + "identity/connect/token";

        public const string ProfileDataUrl = UrlBase + "identity/connect/userinfo";

        public const string ResendEmailUrl = AccountsUrlBase + "resend-confirmation-mail";
    }
}