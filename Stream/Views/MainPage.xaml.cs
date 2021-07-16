using Stream.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Stream.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.SizeChanged += OnSizeChanged;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            this.windows = new List<ResizableWindow>();
            CreateDefaultWindow();
        }

        internal void RemoveWindow(ResizableWindow window)
        {
            this.windows.Remove(window);
            this.content.Children.Remove(window);
        }

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

        private void StartFileReloadTimer()
        {
            if (this.timer == null)
            {
                this.timer = new Timer(1200);
                this.timer.Elapsed += ReloadFile;
                this.timer.Start();
            }
        }

        private async void ReloadFile(object sender, ElapsedEventArgs e)
        {
            if (this.file != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ReadFile(this.file); });
            }
        }

        private void CreateDefaultWindow()
        {
            var window = new ResizableWindow(this)
            {
                Width = Window.Current.Bounds.Width - 20,
                Height = Window.Current.Bounds.Height - 100
            };
            Canvas.SetLeft(window, 10);
            Canvas.SetTop(window, 10);
            this.windows.Add(window);

            this.content.Children.Add(window);

            Configuration.Configuration.AddWindowConfiguration(window);
        }

        private async void ReadFile(StorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);

            foreach (var window in this.windows)
            {
                window.SetText(text);
            }

            StartFileReloadTimer();
        }

        private async void OpenFilePickerAsync(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add("*");

            this.file = await openPicker.PickSingleFileAsync();
            if (this.file != null)
            {
                this.title.Text = $"STREAM - {this.file.Path}";
                ReadFile(this.file);
            }
        }

        private void AddWindowAsync(object sender, RoutedEventArgs e)
        {
            var window = new ResizableWindow(this);
            Canvas.SetLeft(window, Window.Current.Bounds.Width / 2);
            Canvas.SetTop(window, 20);

            this.windows.Add(window);

            this.content.Children.Add(window);
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            foreach (var window in this.windows)
            {
                window.OnSizeChanged(e.Size.Width, e.Size.Height);
            }
        }

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

