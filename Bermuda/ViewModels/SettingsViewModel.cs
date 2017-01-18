using Bermuda.DataModels;
using Bermuda.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged, IDisposable
    {
        public ColorListViewModel colorlistviewmodel;

        public int selectedIndex { get; set; }

        public string loggedInUser => NewMain.Current.mc.Session.UserDetails.Email;

        public SettingsViewModel()
        {
            colorlistviewmodel = new ColorListViewModel();
        }

        public void accentChanged(object sender, object e)
        {
            if (AppSettings.localSettings.Values["accentColor"] != null)
            {
                int index = 12;

                ComboBox cb = sender as ComboBox;

                if (cb.SelectedIndex > -1)
                    index = cb.SelectedIndex;

                AppSettings.localSettings.Values["accentColor"] = index.ToString();
                PlayerService.Instance.updateTheme();
            }

            else
            {
                int index = 12;

                AppSettings.localSettings.Values["accentColor"] = index.ToString();
                PlayerService.Instance.updateTheme();
            }

        }

        public async void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(
            "Are you sure you want to log out?",
            "Confirm logout!");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if(result.Label == "Yes")
            {
                var vault = new Windows.Security.Credentials.PasswordVault();

                Windows.Security.Credentials.PasswordCredential credential = GetCredentialFromLocker();

                if (credential != null)
                    vault.Remove(new Windows.Security.Credentials.PasswordCredential("Bermuda", credential.UserName, credential.Password));

                NewMain.Current.mc = null;
                NewMain.Current.loadLoginFrame();
            }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            colorlistviewmodel = null;
        }
    }
}
