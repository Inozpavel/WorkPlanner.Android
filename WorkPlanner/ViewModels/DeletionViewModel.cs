using System;

namespace WorkPlanner.ViewModels
{
    public abstract class DeletionViewModel<T> : BaseViewModel
    {
        public event EventHandler<T> SuccessfulDeletion;

        public event EventHandler<string> FailedDeletion;

        protected void OnSuccessfulDeletion(T result) => SuccessfulDeletion?.Invoke(this, result);

        protected void OnFailedDeletion(string message) => FailedDeletion?.Invoke(this, message);
    }
}