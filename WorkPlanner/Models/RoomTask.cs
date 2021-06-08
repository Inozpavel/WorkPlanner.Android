using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkPlanner.Models
{
    public class RoomTask : INotifyPropertyChanged
    {
        private DateTime _deadlineTime;

        private string _details;

        private bool _isCompleted;

        private string _taskContent;

        private DateTime _taskCreationTime;

        private string _taskName;

        public Guid RoomTaskId { get; set; }

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }

        public string TaskContent
        {
            get => _taskContent;
            set
            {
                _taskContent = value;
                OnPropertyChanged();
            }
        }

        public string Details
        {
            get => _details;
            set
            {
                _details = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }

        public DateTime TaskCreationTime
        {
            get => _taskCreationTime;
            set
            {
                _taskCreationTime = value;
                OnPropertyChanged();
            }
        }

        public Guid TaskCreatorId { get; set; }

        public DateTime DeadlineTime
        {
            get => _deadlineTime;
            set
            {
                _deadlineTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}