using System;

namespace WorkPlanner.Requests
{
    public class UpdateTaskRequest
    {
        public string TaskName { get; set; }

        public string TaskContent { get; set; }

        public string Details { get; set; }

        public DateTime DeadlineTime { get; set; }
    }
}