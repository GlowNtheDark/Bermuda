using Bermuda.DataModels;
using Bermuda.Services;
using Bermuda.ViewModels;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewMain : Page
    {
        public static NewMain Current;

        MediaPlayer Player => PlayerService.Instance.Player;

        MessageList Messagelist
        {
            get { return MessagingService.Instance.Messagelist; }
            set { MessagingService.Instance.Messagelist = value; }
        }

        public NewMain()
        {
            this.InitializeComponent();

            Current = this;

            MainViewModel = new MainMenuViewModel(Player, Messagelist, this.Dispatcher);

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


            if (!checkForBGTask(networkBackgroundTask))
                createNetworkAwarenessTask(networkBackgroundTask);
            else
                reregisterNetworkAwarenessTask(networkBackgroundTask);

            if (!checkForBGTask(servicingBackgroundTask))
                createServicingTask(servicingBackgroundTask);
            else
                reregisterServicingTask(servicingBackgroundTask);
        }

        Type npScene = typeof(NowPlayingScene);
        Type qpScene = typeof(QuickPlayScene);
        Type stScene = typeof(SettingsScene);
        Type plScene = typeof(PlaylistsScene);
        Type srScene = typeof(SearchScene);

        public MobileClient mc;
        public MainMenuViewModel MainViewModel { get; set; }
        public string lastNetworkState;
        private string networkBackgroundTask = "Network-Awareness-Task";
        private string servicingBackgroundTask = "Servicing-Complete-Task";

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            if (MainViewModel.AlertVisibility == Visibility.Visible)
            {
                MainViewModel.AlertVisibility = Visibility.Collapsed;
                MessagingService.Instance.isNewAlert = false;
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            StackPanel sp = e.ClickedItem as StackPanel;

            loadFrame(sp.Name);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (mc != null)
                {

                    if (await testAuthorizationLevel())
                    {
                        if (AppSettings.localSettings.Values["lastPage"] != null)
                            loadFrame(AppSettings.localSettings.Values["lastPage"].ToString());
                        else
                            loadFrame("QuickPlay");
                    }
                    else
                    {
                        //Load setting page and show error about sub
                    }
                }

                else
                {
                    //PassSession data = e.Parameter as PassSession;
                    mc = PassSession.session;

                    if (await testAuthorizationLevel())
                    {
                        if (AppSettings.localSettings.Values["lastPage"] != null)
                            loadFrame(AppSettings.localSettings.Values["lastPage"].ToString());
                        else
                            loadFrame("QuickPlay");
                    }
                    else
                    {
                        //Load setting page and show error about sub
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private async void createNetworkAwarenessTask(string taskName)
        {
            try
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "Tasks.NetworkAwareness";
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange, false));
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskRegistration task = builder.Register();
                task.Completed += new BackgroundTaskCompletedEventHandler(networkAwarenessOnCompleted);
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }


        }

        private async void createServicingTask(string taskName)
        {
            try
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "Tasks.ServicingComplete";
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskRegistration task = builder.Register();
                task.Completed += new BackgroundTaskCompletedEventHandler(servicingOnCompleted);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }


        }

        private bool checkForBGTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }

            return false;
        }

        private async void networkAwarenessOnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var key = task.Name.ToString();
            string status = settings.Values[key].ToString();

            //if status == InternetAccess - enable app functionality and display flyout noting change

            //else - disable app functionality and display flyout noting change

            if (status != lastNetworkState)//Don't update for the same state twice.
            {
                if (status == "Internet Access")
                {
                    bool authTest = await testAuthorizationLevel();
                    lastNetworkState = "Internet Access";

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {

                        

                    });
                }

                else
                {
                    lastNetworkState = "No Internet Access";
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {



                    });
                }
            }
        }

        private void servicingOnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            if (checkForBGTask(networkBackgroundTask))
                unregisterTask(networkBackgroundTask);
            if (checkForBGTask(servicingBackgroundTask))
                unregisterTask(servicingBackgroundTask);

            BackgroundExecutionManager.RemoveAccess();
        }

        private void reregisterNetworkAwarenessTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                    createNetworkAwarenessTask(task.Value.Name);
                }
            }
        }

        private void reregisterServicingTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    unregisterTask(task.Value.Name);
                    createServicingTask(task.Value.Name);
                }
            }
        }

        private void unregisterTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        public async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        private async Task<bool> testAuthorizationLevel()
        {
            try
            {
                Track data = await getTrack(mc, "Tolw673c2mkdbbthmo4e6vzgsdu");

                Uri data2 = await GetStreamUrl(mc, data);

                if (data2 != null)
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

        public async Task<Track> getTrack(MobileClient mc, string trackId)
        {
            Track data;
            data = await mc.GetTrackAsync(trackId);
            return data;
        }

        public void loadFrame(string frameName)
        {
            if (frameName == "NowPlaying")
            {
                pageTitle.Text = "Now Playing";
                pageTitle.Visibility = Visibility.Visible;
                mainFrame.Navigate(npScene);
            }
            else if (frameName == "QuickPlay")
            {
                pageTitle.Text = "QuickPlay";
                pageTitle.Visibility = Visibility.Visible;
                mainFrame.Navigate(qpScene);
            }

            else if (frameName == "Settings")
            {
                pageTitle.Text = "Settings";
                pageTitle.Visibility = Visibility.Visible;
                mainFrame.Navigate(stScene);
            }

            else if (frameName == "Playlists")
            {
                pageTitle.Text = "Playlists";
                pageTitle.Visibility = Visibility.Visible;
                mainFrame.Navigate(plScene);
            }

            else
            {
                pageTitle.Text = "Search";
                pageTitle.Visibility = Visibility.Visible;
                mainFrame.Navigate(srScene);
            }

        }

        public void loadLoginFrame()
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void Page_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.GamepadView)
                MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
    }
}
