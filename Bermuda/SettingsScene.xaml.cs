using Bermuda.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsScene : Page
    {
        SettingsViewModel settingsviewmodel { get; set; }

        public SettingsScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            settingsviewmodel = new SettingsViewModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int accentIndex;
            string storedColor;

            signedInAsTextBlock.Text = NewMain.Current.mc.Session.UserDetails.Email.ToString();

            if (AppSettings.localSettings.Values["accentColor"] != null)
            {
                storedColor = AppSettings.localSettings.Values["accentColor"].ToString();
                accentIndex = int.Parse(storedColor);
            }

            else
                accentIndex = 12;      

            accentComboBox.SelectedIndex = accentIndex;

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            settingsviewmodel.Dispose();
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "Settings";
        }
    }
}
