using WorkPlanner.Models;

namespace WorkPlanner.ViewModels
{
    public class TaskDetailsPageViewModel
    {
        public TaskDetailsPageViewModel()
        {
        }

        public TaskDetailsPageViewModel(RoomTask roomTask) => RoomTask = roomTask;
        public RoomTask RoomTask { get; }
    }
}