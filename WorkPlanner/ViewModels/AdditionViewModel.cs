using System;

namespace WorkPlanner.ViewModels
{
    public abstract class AdditionViewModel<T> : BaseViewModel
    {
        public event EventHandler<T> SuccessfulAddition;

        public event EventHandler<string> FailedAddition;

        protected void OnSuccessfulAddition(T result) => SuccessfulAddition?.Invoke(this, result);

        protected void OnFailedAddition(string message) => FailedAddition?.Invoke(this, message);
    }
}