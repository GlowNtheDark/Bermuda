using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GoogleMusicApi.UWP.Common;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

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

        }

        bool storeMyCredentials = false;

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (errorTextBlock.Visibility == Visibility.Visible)
                errorTextBlock.Visibility = Visibility.Collapsed;

            string username = unameTextBox.Text;
            string password = passwordBox.Password;

            Login(username, password);
        }

        private async void Login(string username, string password)
        {
            MobileClient mc = new MobileClient();

            if (await mc.LoginAsync(username, password))
            {
                if (storeMyCredentials)
                    storeCredentials(username, password);

                Frame.Navigate(typeof(MainPage), new PassSession { session = mc });
            }
            else
            {
                errorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void storeCredentials(string username, string password)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential("Bermuda", username, password));
        }

        private void storeCredentialsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            storeMyCredentials = true;
        }

        private void storeCredentialsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            storeMyCredentials = false;
        }
    }
}
