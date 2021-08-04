using Stream.Core;
using Stream.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Config = Stream.Configuration.Configuration;

namespace Stream.Views
{
    /// <summary>
    /// Settings view.
    /// </summary>
    public sealed partial class Settings : UserControl
    {
        /// <summary>
        /// Gets or sets workspaces list.
        /// </summary>
        public IList<string> Workspaces { get; set; }

        /// <summary>
        /// Gets or sets application configuration.
        /// </summary>
        public Config Configuration => this.configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();
            this.LoadConfiguration();
        }

        /// <summary>
        /// Toggles settings visibility.
        /// </summary>
        public void Toggle()
        {
            this.root.Visibility = this.root.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            this.SlideInStoryboard.Begin();
        }

        /// <summary>
        /// Loads configuration.
        /// </summary>
        private async void LoadConfiguration()
        {
            this.configuration = await Config.LoadAsync();
            this.ApplyTheme();
        }

        /// <summary>
        /// Applies a new application theme. Called when the user changes the theme in settings.
        /// </summary>
        private void ApplyTheme()
        {
            var theme = this.configuration.Theme.ToElementTheme();
            if (Window.Current.Content is FrameworkElement element)
            {
                element.RequestedTheme = theme;
            }
        }

        /// <summary>
        /// Called when theme is changed.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ThemeChecked(object sender, RoutedEventArgs e)
        {
            var theme = (sender as MenuFlyoutItem).Tag.ToString();
            this.configuration.Theme = theme.ToTheme();
            this.configuration.Save();
            this.ApplyTheme();
        }

        /// <summary>
        /// Called when user deletes a workspace.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private async void RemoveWorkspaceAsync(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog(LanguageManager.GetString("Dialog_RemoveWorkspace_Prompt"))
            {
                Title = LanguageManager.GetString("Dialog_RemoveWorkspace_Header"),
            };

            dialog.Commands.Add(new UICommand() { Label = LanguageManager.GetString("Yes"), Id = 0 });
            dialog.Commands.Add(new UICommand() { Label = LanguageManager.GetString("No"), Id = 1 });

            var result = await dialog.ShowAsync();
            if ((int)result.Id == 0)
            {
                var fileName = (sender as Button).Tag.ToString();
                await FileManager.DeleteLocalFileAsync($"{fileName}.workspace");
            }

            // Repopulate workspaces list.
            this.Workspaces = FileManager.GetLocalFiles(".workspace").ToList();
        }

        private Config configuration;
    }
}
