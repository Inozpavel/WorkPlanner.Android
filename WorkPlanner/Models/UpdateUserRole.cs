using System;
using Newtonsoft.Json;

namespace WorkPlanner.Models
{
    public class UpdateUserRole
    {
        public Guid UserId { get; set; }

        public Guid RoomRoleId { get; set; }

        [JsonIgnore]
        public string RoomRoleName { get; set; }
    }
}