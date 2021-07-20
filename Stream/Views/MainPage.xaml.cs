using Stream.Components;
using Stream.Extensions;
using Stream.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Stream.Views
{
    /// <summary>
    /// Main application view.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.windows = new List<ResizableWindow>();

            Window.Current.SizeChanged += OnSizeChanged;
            CoreApplication.GetCurrentView().TitleBar.ExtendView();

            foreach (var file in FileCache.Files)
            {
                var item = new MenuFlyoutItem()
                {
                    Text = file.Key,
                };

                item.Click += (s, e) =>
                {
                    this.OpenFileAsync(file.Value);
                };

                this.openButtonFlyout.Items.Add(item);
            }
        }

        /// <summary>
        /// Called when view is navigated to.
        /// </summary>
        /// <param name="e">Navigation events.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var token = e.Parameter.ToString();
            if (!string.IsNullOrEmpty(token))
            {
                // If there was a token argument provided, the user
                // clicked a file in the jumplist. Get the file
                // from the cache and open it.
                this.OnFileOpened(await FileCache.GetFromCacheAsync(token));
            }
        }

        /// <summary>
        /// Moves a window to the front in the window list. This
        /// happens when a user clicks on a window in the UI.
        /// </summary>
        /// <param name="window"></param>
        internal void MoveWindowToFront(ResizableWindow window)
        {
            if (this.content.Children.Last() != window)
            {
                this.content.Children.Remove(window);
                this.content.Children.Add(window);
            }
        }

        /// <summary>
        /// Removes a window from the window list.
        /// </summary>
        /// <param name="window">Window to remove.</param>
        internal void RemoveWindow(ResizableWindow window)
        {
            this.windows.Remove(window);
            this.content.Children.Remove(window);
        }

        /// <summary>
        /// Called when a text line has been selected in one of the windows.
        /// Propagate the line number to all other windows and select it.
        /// </summary>
        /// <param name="caller">GUID of calling window.</param>
        /// <param name="lineNumber">Selected line number.</param>
        internal void OnLineSelected(Guid caller, int lineNumber)
        {
            foreach (var window in this.windows)
            {
                if (!window.Id.Equals(caller))
                {
                    window.SelectLine(lineNumber);
                }
            }
        }

        /// <summary>
        /// Starts file reload timer.
        /// </summary>
        private void StartFileReloadTimer()
        {
            if (this.timer == null)
            {
                this.timer = new Timer(1000);
                this.timer.Elapsed += ReloadFile;
                this.timer.Start();
            }
        }

        /// <summary>
        /// Stops file reload timer.
        /// </summary>
        private void StopFileReloadTimer()
        {
            this.timer.Stop();
            this.timer = null;
        }

        /// <summary>
        /// Reloads file. Called when file reload timer expires.
        /// </summary>
        /// <param name="sender">Calling timer.</param>
        /// <param name="e">Timer events.</param>
        private async void ReloadFile(object sender, ElapsedEventArgs e)
        {
            if (this.file != null)
            {
                // Read the file on the main thread.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.ReadFile(this.file); });
            }
        }

        /// <summary>
        /// Read a file.
        /// </summary>
        /// <param name="file">File to read.</param>
        private async void ReadFile(StorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            foreach (var window in this.windows)
            {
                window.SetText(text);
            }

            this.StartFileReloadTimer();
        }

        /// <summary>
        /// Opens a file picker dialog.
        /// </summary>
        /// <param name="sender">Calling UI element.</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenFilePickerAsync(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add("*");

            this.OnFileOpened(await openPicker.PickSingleFileAsync());
        }

        /// <summary>
        /// Called when a file has been requested via the jumplist.
        /// </summary>
        /// <param name="token">File MRU token.</param>
        private async void OpenFileAsync(string token)
        {
            this.OnFileOpened(await FileCache.GetFromCacheAsync(token));
        }

        /// <summary>
        /// Called when a file has been opened either via the file open dialog
        /// or requested via the jumplist.
        /// </summary>
        /// <param name="file">Opened file.</param>
        private void OnFileOpened(StorageFile file)
        {
            if (file != null)
            {
                this.file = file;
                this.title.Text = $"STREAM - {this.file.Path}";

                FileCache.AddToCache(this.file);
                this.ReadFile(this.file);

                // Change command bar actions.
                this.openButton.Visibility = Visibility.Collapsed;
                this.closeButton.Visibility = Visibility.Visible;
                this.closeSeparator.Visibility = Visibility.Visible;
                this.newWindowButton.Visibility = Visibility.Visible;
                this.arrangeButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Closes the opened file.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void CloseFile(object sender, RoutedEventArgs e)
        {
            this.title.Text = "STREAM";

            foreach (var window in this.windows)
            {
                window.SetText(string.Empty, true);
            }

            this.StopFileReloadTimer();
            this.file = null;
            
            // Change command bar actions.
            this.openButton.Visibility = Visibility.Visible;
            this.closeButton.Visibility = Visibility.Collapsed;
            this.closeSeparator.Visibility = Visibility.Collapsed;
            this.newWindowButton.Visibility = Visibility.Collapsed;
            this.arrangeButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event argument.</param>
        private void AddWindow(object sender, RoutedEventArgs e)
        {
            // Based new window's position on the number of windows.
            var window = new ResizableWindow(this);
            var x = 10 + (this.windows.Count * 20);
            var y = 10 + (this.windows.Count * 20);

            if (x > Window.Current.Bounds.Width - 640) x -= 640;
            if (y > Window.Current.Bounds.Height - 480) y -= 480;

            Canvas.SetLeft(window,x);
            Canvas.SetTop(window, y);

            this.windows.Add(window);
            this.content.Children.Add(window);
        }

        /// <summary>
        /// Called when application window is resized.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            foreach (var window in this.windows)
            {
                window.OnSizeChanged(e.Size.Width, e.Size.Height);
            }
        }

        /// <summary>
        /// Arrange windows into columns.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeColumns(object sender, RoutedEventArgs e)
        {
            if (!this.windows.Any()) return;

            var bounds = Window.Current.Bounds;
            bounds.Height -= 80;

            var allocatedWidth = bounds.Width / this.windows.Count;
            var x = 0.0;

            foreach (var window in this.windows)
            {
                Canvas.SetLeft(window, x);
                Canvas.SetTop(window, 0);
                window.Width = allocatedWidth;
                window.Height = bounds.Height;

                x += allocatedWidth;
            }
        }

        /// <summary>
        /// Arrange windows into rows.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeRows(object sender, RoutedEventArgs e)
        {
            if (!this.windows.Any()) return;

            var bounds = Window.Current.Bounds;
            bounds.Height -= 80;

            var allocatedHeight = bounds.Height / this.windows.Count;
            var y = 0.0;

            foreach (var window in this.windows)
            {
                Canvas.SetLeft(window, 0);
                Canvas.SetTop(window, y);
                window.Width = bounds.Width;
                window.Height = allocatedHeight;

                y += allocatedHeight;
            }
        }

        /// <summary>
        /// Arrange windows in an X by Y grid.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeGrid(object sender, RoutedEventArgs e)
        {
            if (!this.windows.Any()) return;

            var bounds = Window.Current.Bounds;
            bounds.Height -= 80;

            var numRows = (int)Math.Floor(Math.Sqrt(this.windows.Count));
            while (this.windows.Count % numRows != 0)
            {
                numRows--;
            }

            var numCols = this.windows.Count / numRows;
            var maxWidth = bounds.Width / numCols;
            var maxHeight = bounds.Height / numRows;
            var x = 0.0;
            var y = 0.0;

            foreach (var window in this.windows)
            {
                Canvas.SetLeft(window, x);
                Canvas.SetTop(window, y);
                window.Width = maxWidth;
                window.Height = maxHeight;

                y += maxHeight;

                if (y >= bounds.Height)
                {
                    y = 0;
                    x += maxWidth;
                }
            }
        }

        private StorageFile file;
        private Timer timer;

        private readonly IList<ResizableWindow> windows;
    }
}

