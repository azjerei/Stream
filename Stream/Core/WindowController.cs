﻿using Stream.Components;
using Stream.Configuration;
using Stream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Stream.Core
{
    /// <summary>
    /// Window controller.
    /// </summary>
    public class WindowController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="canvasHost">Host canvas.</param>
        public WindowController(Canvas canvasHost)
        {
            this.canvas = canvasHost;
            this.windows = new List<ResizableWindow>();

            Window.Current.SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Removes a window from the window list.
        /// </summary>
        /// <param name="window">Window to remove.</param>
        public void RemoveWindow(ResizableWindow window)
        {
            this.windows.Remove(window);
            this.canvas.Children.Remove(window);
        }

        /// <summary>
        /// Called when a text line has been selected in one of the windows.
        /// Propagate the line number to all other windows and select it.
        /// </summary>
        /// <param name="caller">GUID of calling window.</param>
        /// <param name="lineNumber">Selected line number.</param>
        public void SelectLine(Guid caller, int lineNumber)
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
        /// Sets text content in all windows.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="forceRedraw">If true, force the windows to redraw.</param>
        public void SetText(string text, bool forceRedraw = false)
        {
            foreach (var window in this.windows)
            {
                window.SetText(text, forceRedraw);
            }
        }

        /// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="window">Window to add.</param>
        public void AddWindow(ResizableWindow window)
        {
            // Based new window's position on the number of windows.
            var x = 10 + (this.windows.Count * 20);
            var y = 10 + (this.windows.Count * 20);

            if (x > Window.Current.Bounds.Width - 640)
            {
                x -= 640;
            }

            if (y > Window.Current.Bounds.Height - 480)
            {
                y -= 480;
            }

            Canvas.SetLeft(window, x);
            Canvas.SetTop(window, y);

            this.windows.Add(window);
            this.canvas.Children.Add(window);
        }

        /// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="window">Window to add.</param>
        /// <param name="config">Window configuration</param>
        public void AddWindow(ResizableWindow window, WindowConfiguration config)
        {
            Canvas.SetLeft(window, config.X);
            Canvas.SetTop(window, config.Y);

            window.Width = config.Width;
            window.Height = config.Height;
            window.FilterConfiguration = config.Filter;
            window.HighlightConfiguration = config.Highlight;

            this.windows.Add(window);
            this.canvas.Children.Add(window);
        }

        /// <summary>
        /// Arrange windows according to type.
        /// </summary>
        /// <param name="type">Type of window arrangement.</param>
        public void Arrange(ArrangeType type)
        {
            switch (type)
            {
                case ArrangeType.Columns:
                    this.ArrangeColumns();
                    break;
                case ArrangeType.Rows:
                    this.ArrangeRows();
                    break;
                case ArrangeType.Grid:
                    this.ArrangeGrid();
                    break;
            }
        }

        /// <summary>
        /// Moves a window to the front in the window list. This
        /// happens when a user clicks on a window in the UI.
        /// </summary>
        /// <param name="window"></param>
        public void MoveWindowToFront(ResizableWindow window)
        {
            if (this.canvas.Children.Last() != window)
            {
                this.canvas.Children.Remove(window);
                this.canvas.Children.Add(window);
            }
        }

        /// <summary>
        /// Gets window configurations.
        /// </summary>
        /// <returns></returns>
        public IList<WindowConfiguration> GetWindowConfigurations()
        {
            var configurations = new List<WindowConfiguration>();

            foreach (var window in this.windows)
            {
                configurations.Add(new WindowConfiguration
                {
                    X = Canvas.GetLeft(window),
                    Y = Canvas.GetTop(window),
                    Width = window.Width,
                    Height = window.Height,
                    Filter = window.FilterConfiguration,
                    Highlight = window.HighlightConfiguration,
                });
            }

            return configurations;
        }

        /// <summary>
        /// Arrange windows into columns.
        /// </summary>
        private void ArrangeColumns()
        {
            if (!this.windows.Any())
            {
                return;
            }

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
        private void ArrangeRows()
        {
            if (!this.windows.Any())
            {
                return;
            }

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
        private void ArrangeGrid()
        {
            if (!this.windows.Any())
            {
                return;
            }

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

        private readonly Canvas canvas;
        private readonly IList<ResizableWindow> windows;
    }
}
