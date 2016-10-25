using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;
using Windows.System;
using Windows.UI.Notifications;
using Windows.Security.Credentials;
using GoogleMusicApi.UWP.Common;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace Bermuda
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
#if WINDOWS_UWP
            string family = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (family == "Windows.Xbox")
            { 
                this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
            }
#endif
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
            Windows.System.MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;
            Windows.System.MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

        }

        bool _isInBackgroundMode = false;

        private string resourceName = "Bermuda";

        private async void Login(string username, string password, Frame rootframe)
        {
            MobileClient mc = new MobileClient();

            if (await mc.LoginAsync(username, password))
            {
                rootframe.Navigate(typeof(MainPage), new PassSession { session = mc });
            }
        }

        private bool checkStoredCredentials()
        {
            var loginCredential = GetCredentialFromLocker();

            if (loginCredential != null)
                return true;
            else
                return false;

        }

        private PasswordCredential GetCredentialFromLocker()
        {
            PasswordCredential credential = null;

            var vault = new PasswordVault();

            try
            {
                var credentialList = vault.FindAllByResource(resourceName);


                if (credentialList.Count > 0)
                {
                    if (credentialList.Count == 1)
                    {
                        credential = credentialList[0];
                        credential.RetrievePassword();
                    }

                    else
                    {
                        // When there are multiple usernames,
                        // retrieve the default username. If one doesn't
                        // exist, then display UI to have the user select
                        // a default username.
                    }
                }
            }

            catch
            {
                return credential;
            }

            return credential;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

#if WINDOWS_UWP
            string family = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (family == "Windows.Xbox")
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
                bool result = Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(true);
            }
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                    {
                        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                        if (titleBar != null)
                        {
                            titleBar.ButtonBackgroundColor = Colors.Black;
                            titleBar.ButtonForegroundColor = Colors.White;
                            titleBar.BackgroundColor = Colors.Black;
                            titleBar.ForegroundColor = Colors.White;
                        }
                    }


                    bool areCredentialsStored = checkStoredCredentials();

                    if (areCredentialsStored)
                    {
                        PasswordCredential credential = GetCredentialFromLocker();
                        Login(credential.UserName, credential.Password, rootFrame);
                    }
                    else
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }


        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            _isInBackgroundMode = true;

            System.Diagnostics.Debug.WriteLine("Entered background.");
        }

        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("New memory limit" + e.NewLimit.ToString());
            if (MemoryManager.AppMemoryUsage >= e.NewLimit)
            {
                ReduceMemoryUsage(e.NewLimit);

            }
        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            
            var level = MemoryManager.AppMemoryUsageLevel;

            System.Diagnostics.Debug.WriteLine("Memory Usage increased. Lvl:" + level.ToString());

            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                ReduceMemoryUsage(MemoryManager.AppMemoryUsageLimit);
            }
        }

        public void ReduceMemoryUsage(ulong limit)
        {
            System.Diagnostics.Debug.WriteLine("Reducing memory usage.");
            if (_isInBackgroundMode && Window.Current.Content != null)
            {

                Window.Current.Content = null;
                GC.Collect();
            }
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            _isInBackgroundMode = false;
            System.Diagnostics.Debug.WriteLine("Leaving background.");
            // Reastore view content if it was previously unloaded.
            if (Window.Current.Content == null)
            {
                CreateRootFrame(ApplicationExecutionState.Running, string.Empty);
            }
        }

        void CreateRootFrame(ApplicationExecutionState previousExecutionState, string arguments)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                System.Diagnostics.Debug.WriteLine("CreateFrame: Initializing root frame ...");

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (previousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), arguments);
            }
        }
    }
}
