using Stream.Files;
using Stream.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Stream
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await CreateRootFrame(!e.PrelaunchActivated, e.Arguments);
        }

        private async Task<Frame> CreateRootFrame(bool navigate, string arguments)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                //{
                //    //TODO: Load state from previously suspended application
                //}

                Window.Current.Content = rootFrame;
            }

            await JumpList.InitializeAsync();
            await FileCache.InitializeAsync();
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

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        private async Task LoadConfigurationAsync()
        {
            if (!await Configuration.Configuration.LoadAsync())
            {
                await Configuration.Configuration.SaveAsync();
            }
        }
    }
}
