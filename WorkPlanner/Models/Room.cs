using System;

namespace WorkPlanner.Models
{
    public class Room
    {
        public Guid RoomId { get; set; }

        public string RoomName { get; set; }

        public string RoomDescription { get; set; }

        public DateTime CreationDate { get; set; }
    }
}