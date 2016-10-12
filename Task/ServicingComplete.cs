
using Windows.ApplicationModel.Background;


namespace Tasks
{
    public sealed class ServicingComplete : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral; // Note: defined at class scope so we can mark it complete inside the OnCancel() callback if we choose to support cancellation
        IBackgroundTaskInstance _taskInstance = null;
        volatile bool _cancelRequested = false;


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            _deferral.Complete();
        }

        public void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
        }
    }
}

