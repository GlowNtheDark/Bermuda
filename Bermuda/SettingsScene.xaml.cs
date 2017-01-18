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

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            var vault = new Windows.Security.Credentials.PasswordVault();

            Windows.Security.Credentials.PasswordCredential credential = GetCredentialFromLocker();

            if (credential != null)
                vault.Remove(new Windows.Security.Credentials.PasswordCredential("Bermuda", credential.UserName, credential.Password));

            NewMain.Current.mc = null;
            NewMain.Current.loadLoginFrame();
        }

        private Windows.Security.Credentials.PasswordCredential GetCredentialFromLocker()
        {
            Windows.Security.Credentials.PasswordCredential credential = null;

            var vault = new Windows.Security.Credentials.PasswordVault();

            try
            {
                var credentialList = vault.FindAllByResource("Bermuda");

                if (credentialList.Count > 0)
                {
                    if (credentialList.Count == 1)
                    {
                        credential = credentialList[0];
                        credential.RetrievePassword();
                    }
                }

                return credential;
            }

            catch
            {
                return credential;
            }
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
