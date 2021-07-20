﻿using Stream.Components;
using Stream.Core;
using Stream.Extensions;
using Stream.Files;
using System;
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
            this.controller = new WindowController(this.content);
            this.timer = new FileReloadTimer(1000, () =>
            {
                this.ReloadFileAsync();
            });

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
        /// Moves a window to the front in the window list. This
        /// happens when a user clicks on a window in the UI.
        /// </summary>
        /// <param name="window"></param>
        public void MoveWindowToFront(ResizableWindow window)
        {
            this.controller.MoveWindowToFront(window);
        }

        /// <summary>
        /// Removes a window from the window list.
        /// </summary>
        /// <param name="window">Window to remove.</param>
        public void RemoveWindow(ResizableWindow window)
        {
            this.controller.RemoveWindow(window);
        }

        /// <summary>
        /// Called when a text line has been selected in one of the windows.
        /// Propagate the line number to all other windows and select it.
        /// </summary>
        /// <param name="caller">GUID of calling window.</param>
        /// <param name="lineNumber">Selected line number.</param>
        public void OnLineSelected(Guid caller, int lineNumber)
        {
            this.controller.SelectLine(caller, lineNumber);
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
        /// Reloads file. Called when file reload timer expires.
        /// </summary>
        private async void ReloadFileAsync()
        {
            if (this.file != null)
            {
                // Read the file on the main thread.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    this.controller.SetText(await FileIO.ReadTextAsync(this.file));
                });
            }
        }

        /// <summary>
        /// Opens a file picker dialog.
        /// </summary>
        /// <param name="sender">Calling UI element.</param>
        /// <param name="args">Event arguments.</param>
        private async void OpenFilePickerAsync(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            this.OnFileOpened(await FileManager.OpenFileAsync(OpenFileMode.FilePicker));
        }

        /// <summary>
        /// Called when a file has been requested via the jumplist.
        /// </summary>
        /// <param name="token">File MRU token.</param>
        private async void OpenFileAsync(string token)
        {
            this.OnFileOpened(await FileManager.OpenFileAsync(OpenFileMode.Cache, token));
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
                this.timer.Start();
                this.SetState(ViewState.FileOpened);
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
            this.controller.SetText(string.Empty, true);
            this.timer.Stop();
            this.file = null;
            this.SetState(ViewState.FilePending);
        }

        /// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event argument.</param>
        private void AddWindow(object sender, RoutedEventArgs e)
        {
            this.controller.AddWindow(new ResizableWindow(this));
        }

        /// <summary>
        /// Arrange windows into columns.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeColumns(object sender, RoutedEventArgs e)
        {
            this.controller.Arrange(ArrangeType.Columns);
        }

        /// <summary>
        /// Arrange windows into rows.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeRows(object sender, RoutedEventArgs e)
        {
            this.controller.Arrange(ArrangeType.Rows);
        }

        /// <summary>
        /// Arrange windows in an X by Y grid.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ArrangeGrid(object sender, RoutedEventArgs e)
        {
            this.controller.Arrange(ArrangeType.Grid);
        }

        /// <summary>
        /// Sets view state.
        /// </summary>
        /// <param name="state">View state.</param>
        private void SetState(ViewState state)
        {
            // Change command bar actions.
            this.openButton.Visibility = state == ViewState.FilePending ? Visibility.Visible : Visibility.Collapsed;
            this.closeButton.Visibility = state == ViewState.FileOpened ? Visibility.Visible : Visibility.Collapsed;
            this.closeSeparator.Visibility = state == ViewState.FileOpened ? Visibility.Visible : Visibility.Collapsed;
            this.newWindowButton.Visibility = state == ViewState.FileOpened ? Visibility.Visible : Visibility.Collapsed;
            this.arrangeButton.Visibility = state == ViewState.FileOpened ? Visibility.Visible : Visibility.Collapsed;
        }

        private StorageFile file;
        private readonly WindowController controller;
        private readonly FileReloadTimer timer;

        private enum ViewState
        {
            FilePending,
            FileOpened,
        }
    }
}

