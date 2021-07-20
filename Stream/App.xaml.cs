using Stream.Files;
using Stream.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Stream
{
    /// <summary>
    /// Main application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Called when application is launched.
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await CreateRootFrame(!e.PrelaunchActivated, e.Arguments);
        }

        /// <summary>
        /// Creates root frame element that hosts the application's UI.
        /// </summary>
        /// <param name="navigate"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private async Task<Frame> CreateRootFrame(bool navigate, string arguments)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            // Initialize jump list content.
            await JumpList.InitializeAsync();

            // Initialize file cache.
            await FileCache.InitializeAsync();

            // Load configuration.
            await this.LoadConfigurationAsync();

            if (navigate)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), arguments);
                }

                Window.Current.Activate();
            }

            return Window.Current.Content as Frame;
        }

        /// <summary>
        /// Called when navigation fails.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Called when application is suspended.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        /// <summary>
        /// Loads application configuration.
        /// </summary>
        /// <returns></returns>
        private async Task LoadConfigurationAsync()
        {
            if (!await Configuration.Configuration.LoadAsync())
            {
                await Configuration.Configuration.SaveAsync();
            }
        }
    }
}
