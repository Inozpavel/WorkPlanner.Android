using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkPlanner.Models;
using WorkPlanner.Requests;
using Xamarin.Forms;

namespace WorkPlanner.ViewModels
{
    public class TaskDetailsPageViewModel : BaseViewModel
    {
        private readonly Guid _roomId;

        private readonly Guid _roomTaskId;

        public TaskDetailsPageViewModel()
        {
        }

        public TaskDetailsPageViewModel(Guid roomId, Guid roomTaskId)
        {
            DeleteTaskCommand = new Command(ServerHelper.DecorateFailedConnectToServer(DeleteTask, OnConnectionFailed));
            UpdateTaskCommand = new Command(ServerHelper.DecorateFailedConnectToServer(UpdateTask, OnConnectionFailed));
            LoadTaskCommand = new Command(ServerHelper.DecorateFailedConnectToServer(LoadTask, OnConnectionFailed));
            GetCreatorInformationCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(LoadTaskCreator, OnConnectionFailed));
            CompleteTaskCommand =
                new Command(ServerHelper.DecorateFailedConnectToServer(CompleteTask, OnConnectionFailed));
            _roomId = roomId;
            _roomTaskId = roomTaskId;
            RoomTask = new RoomTask
            {
                RoomTaskId = roomTaskId,
            };
        }

        public RoomTask RoomTask { get; } = new();

        public Command UpdateTaskCommand { get; }

        public Command LoadTaskCommand { get; }

        public Command GetCreatorInformationCommand { get; }

        public Command DeleteTaskCommand { get; }

        public Command CompleteTaskCommand { get; }

        private async Task CompleteTask()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.CompleteTaskUrl, _roomId, _roomTaskId));
            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(RoomTask, Messages.TasksCompletionSuccess);
                RoomTask.IsCompleted = true;
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.TasksCompletionFail);
        }

        private async Task DeleteTask()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.DeleteAsync(string.Format(Settings.TaskUrl, _roomId, _roomTaskId));

            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(RoomTask, Messages.TaskDeletionSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.TaskDeletionFail);
        }

        private async Task LoadTaskCreator()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.TaskCreatorUrl, _roomId, _roomTaskId));
            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var user = await ServerHelper.DeserializeAsync<User>(resultContent);
                MessagingCenter.Send(user, Messages.TaskCreatorLoadSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.TaskCreatorLoadFail);
        }

        private async Task LoadTask()
        {
            var client = await ServerHelper.GetClientWithToken();

            var result = await client.GetAsync(string.Format(Settings.TaskUrl, _roomId, _roomTaskId));

            string resultContent = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                var loadedTask = await ServerHelper.DeserializeAsync<RoomTask>(resultContent);
                RoomTask.TaskName = loadedTask.TaskName;
                RoomTask.TaskContent = loadedTask.TaskContent;
                RoomTask.Details = loadedTask.Details;
                RoomTask.DeadlineTime = loadedTask.DeadlineTime;
                RoomTask.TaskCreationTime = loadedTask.TaskCreationTime;
                RoomTask.IsCompleted = loadedTask.IsCompleted;
                MessagingCenter.Send(loadedTask, Messages.TaskLoadSuccess);
                return;
            }

            MessagingCenter.Send(this, Messages.TaskLoadFail);
        }

        private async Task UpdateTask()
        {
            var client = await ServerHelper.GetClientWithToken();

            var request = new UpdateTaskRequest
            {
                Details = RoomTask.Details,
                DeadlineTime = RoomTask.DeadlineTime,
                TaskContent = RoomTask.TaskContent,
                TaskName = RoomTask.TaskName
            };
            string data = await ServerHelper.SerializeAsync(request);
            var result = await client.PutAsync(string.Format(Settings.TaskUrl, _roomId, _roomTaskId),
                new StringContent(data, Encoding.UTF8, "application/json"));

            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                MessagingCenter.Send(RoomTask, Messages.TaskUpdateSuccess);
                return;
            }

            MessagingCenter.Send(ServerHelper.GetErrorFromResponse(resultContent), Messages.TaskUpdateFail);
        }
    }
}