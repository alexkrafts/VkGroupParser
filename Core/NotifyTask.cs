using System;
using System.Threading.Tasks;

namespace Core
{
    public sealed class NotifyTask<TResult> : Notifier
    {
        /// <summary>
        /// Facade for a task to bind <see cref="task"/> info
        /// </summary>
        /// <param name="task"> target task </param>
        /// <param name="message"> message to display before complition </param>
        public NotifyTask(Task<TResult> task, string message)
        {
            _message = message;
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        /// <summary>
        /// Listen for a changes in the <see cref="task"/>
        /// </summary>
        /// <param name="task"> target task </param>
        /// <returns></returns>
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Message = e.Message;
            }

            if (task.IsCompleted && !task.IsFaulted)
            {
                Message = string.Empty;
                Result = Task.Result;
            }
            else
            {
                //if (task.Exception != null) Message = task.Exception.Message;
            }
        }
        /// <summary>
        /// Binding result of the task execution
        /// </summary>
        public TResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Task itself
        /// </summary>
        public Task<TResult> Task { get; private set; }

        /// <summary>
        /// Message shows process in the task
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        private string _message;
        private TResult _result;
    }

}
