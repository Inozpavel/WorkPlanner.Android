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

        private const string RoomsUrlBase = UrlBase + "rooms-api/rooms";

        private const string TasksUrlBase = UrlBase + "rooms-api/room/{0}/tasks";

        public const string AllRoomsUrl = RoomsUrlBase;

        public const string AddRoomUrl = RoomsUrlBase;

        public const string DeleteRoomUrl = RoomsUrlBase + "/{0}";

        public const string AllTasksUrl = TasksUrlBase;

        public const string AddTaskUrl = TasksUrlBase;

        public const string JoiningRoomUrl = RoomsUrlBase + "/join/{0}";
    }
}