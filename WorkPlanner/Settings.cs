﻿namespace WorkPlanner
{
    public static class Settings
    {
        private const string UrlBase = "http://mc.icomm.pro:4000/gateway/";

        // private const string UrlBase = "http://10.0.2.2:5000/gateway/";

        private const string AccountsUrlBase = UrlBase + "users-api/accounts/";

        public const string RegisterUrl = AccountsUrlBase + "register";

        public const string LoginUrl = UrlBase + "identity/connect/token";

        public const string ProfileDataUrl = UrlBase + "identity/connect/userinfo";

        public const string ResendEmailUrl = AccountsUrlBase + "resend-confirmation-mail";

        public const string UpdateProfileUrl = AccountsUrlBase + "profile";


        private const string RoomsUrlBase = UrlBase + "rooms-api/rooms";

        public const string AllRoomsUrl = RoomsUrlBase;

        public const string AddRoomUrl = RoomsUrlBase;

        public const string UpdateRoomUrl = RoomsUrlBase + "/{0}";

        public const string DeleteRoomUrl = RoomsUrlBase + "/{0}";

        public const string JoiningRoomUrl = RoomsUrlBase + "/join/{0}";

        public const string UsersInRoomUrl = RoomsUrlBase + "/{0}/roles";


        private const string TasksUrlBase = UrlBase + "rooms-api/room/{0}/tasks";

        public const string AllTasksUrl = TasksUrlBase;

        public const string TaskUrl = TasksUrlBase + "/{1}";

        public const string TaskCreatorUrl = TaskUrl + "/creator";

        public const string AddTaskUrl = TasksUrlBase;

        public const string CompleteTaskUrl = TaskUrl + "/complete";


        private const string RolesUrlBase = UrlBase + "rooms-api/";

        public const string AllRolesUrl = RolesUrlBase + "roles";

        public const string ChangeRoleUrl = RolesUrlBase + "rooms/{0}/roles";
    }
}