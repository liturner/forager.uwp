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
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Forager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            radLight.IsChecked = (int)ApplicationData.Current.RoamingSettings.Values["Theme"] == (int)ApplicationTheme.Light;
            radDark.IsChecked = (int)ApplicationData.Current.RoamingSettings.Values["Theme"] == (int)ApplicationTheme.Dark;
        }

        private async void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            radDark.IsChecked = true;
            ApplicationData.Current.RoamingSettings.Values["Theme"] = (int)ApplicationTheme.Dark;

            ContentDialog restartNotification = new ContentDialog()
            {
                Title = "App Restart Required",
                Content = "Changes to the theme will take effect next time you restart Forager.",
                PrimaryButtonText = "Ok",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false
            };
            await restartNotification.ShowAsync();
        }

        private async void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            radLight.IsChecked = true;
            ApplicationData.Current.RoamingSettings.Values["Theme"] = (int)ApplicationTheme.Light;

            ContentDialog restartNotification = new ContentDialog()
            {
                Title = "App Restart Required",
                Content = "Changes to the theme will take effect next time you restart Forager.",
                PrimaryButtonText = "Ok",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false
            };
            await restartNotification.ShowAsync();
        }
    }
}
