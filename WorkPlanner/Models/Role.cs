using System;

namespace WorkPlanner.Models
{
    public class Role
    {
        public Guid RoomRoleId { get; set; }

        public string RoomRoleName { get; set; }

        public string RoomRoleDescription { get; set; }
    }
}