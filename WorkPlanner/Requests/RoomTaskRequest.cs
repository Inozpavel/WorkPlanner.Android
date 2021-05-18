using System;
using Newtonsoft.Json;

namespace WorkPlanner.Requests
{
    public class RoomTaskRequest
    {
        public string TaskName { get; set; }

        public string TaskContent { get; set; }

        public string Details { get; set; }

        [JsonIgnore]
        public DateTime DeadlineDate { get; set; } = DateTime.Today;

        [JsonIgnore]
        public TimeSpan DeadlineTimeSpan { get; set; } = DateTime.Now.TimeOfDay;

        public DateTime DeadlineTime => new(DeadlineDate.Ticks + DeadlineTimeSpan.Ticks);
    }
}