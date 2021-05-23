using System;

namespace WorkPlanner.Models
{
    public class UserInRoom
    {
        public User User { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }
    }
}