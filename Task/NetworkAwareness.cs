using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.Storage;

namespace Tasks
{
    public sealed class NetworkAwareness : IBackgroundTask
    {

        BackgroundTaskDeferral _deferral; // Note: defined at class scope so we can mark it complete inside the OnCancel() callback if we choose to support cancellation
        IBackgroundTaskInstance _taskInstance = null;
        volatile bool _cancelRequested = false;


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            bool result = await Network();

            UpdateUI(result);

            _deferral.Complete();
        }

        private  void UpdateUI(bool condition)
        {
            if (condition)
            {
                try
                {
                    var key = _taskInstance.Task.Name;
                    String taskStatus = "Internet Access";
                    var settings = ApplicationData.Current.LocalSettings;
                    settings.Values[key] = taskStatus;
                }

                catch(Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }

            }
            else
            {
                try
                { 
                    var key = _taskInstance.Task.Name;
                    String taskStatus = "No Internet Access";
                    var settings = ApplicationData.Current.LocalSettings;
                    settings.Values[key] = taskStatus;
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
        }
        }

        public IAsyncOperation<bool> Network()
        {
            if (_cancelRequested == false)
            {
                return Task.Run(() => getNetworkStatus()).AsAsyncOperation();
            }


            else
            {
                return Task.Run(() => isCanceling()).AsAsyncOperation();
            }
        }

        public bool isCanceling()
        {
            // TODO: Record whether the task completed or was cancelled.
            var settings = ApplicationData.Current.LocalSettings;
            var key = _taskInstance.Task.TaskId.ToString();

            if (_cancelRequested)
            {
                settings.Values[key] = "Canceled";
            }
            else
            {
                settings.Values[key] = "Completed";
            }

            return false;
        }

        public bool getNetworkStatus()
        {
            try
            { 
                ConnectionProfile cf = NetworkInformation.GetInternetConnectionProfile();
            
                if (cf == null)
                {
                    return false;
                }

                var level = cf.GetNetworkConnectivityLevel();

                if (level == NetworkConnectivityLevel.InternetAccess)
                    return true;
                else
                    return false;
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return false;
            }
        }
    
        public void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
        }
    }
}
