using System;

namespace WorkPlanner.Models
{
    public class RoomTask
    {
        public Guid RoomTaskId { get; set; }

        public string TaskName { get; set; }

        public string TaskContent { get; set; }

        public string Details { get; set; }

        public DateTime TaskCreationTime { get; set; }

        public Guid TaskCreatorId { get; set; }

        public DateTime DeadlineTime { get; set; }
    }
}