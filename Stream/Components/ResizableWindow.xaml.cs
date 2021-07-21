using Stream.Configuration;
using Stream.Models;
using Stream.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Stream.Components
{
    /// <summary>
    /// Represents a resizable/movable window.
    /// </summary>
    public sealed partial class ResizableWindow : UserControl
    {
        /// <summary>
        /// Gets window ID.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets filter configuration.
        /// </summary>
        public FilterConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">Owning <see cref="MainPage"/> instance.</param>
        public ResizableWindow(MainPage owner)
        {
            this.InitializeComponent();

            this.owner = owner;
            this.Configuration = new FilterConfiguration()
            {
                Filter = string.Empty,
                IgnoreCase = true,
                Type = FilterType.Contains
            };
            this.Id = Guid.NewGuid();
            this.AutoscrollToggle.IsChecked = true;
            this.OptionContains.IsChecked = true;
            this.OptionIgnoreCase.IsChecked = true;
        }

        /// <summary>
        /// Sets text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        /// <param name="forceRedraw">If true, window will redraw itself.</param>
        public void SetText(string text, bool forceRedraw = false)
        {
            if (text.Length > this.textLength || this.forceTextRedraw || forceRedraw)
            {
                this.TextContent.Items.Clear();
                this.textLength = text.Length;
                this.forceTextRedraw = false;

                if (string.IsNullOrEmpty(text)) return;

                var lines = text.Replace("\r", string.Empty).Split('\n');

                if (!string.IsNullOrEmpty(this.Configuration.Filter))
                {
                    var filter = this.Configuration.Filter;
                    var ignoreCase = this.Configuration.IgnoreCase;

                    Regex regexp = null;
                    try
                    {
                        regexp = this.Configuration.Type == FilterType.RegularExpression ? new Regex(filter) : null;
                    }
                    catch
                    {
                    }

                    var textLines = new List<string>();
                    var n = 1;

                    foreach (var line in lines)
                    {
                        var add = false;

                        switch (this.Configuration.Type)
                        {
                            case FilterType.StartsWith:
                                add = line.StartsWith(filter, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                                break;
                            case FilterType.EndsWith:
                                add = line.EndsWith(filter, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                                break;
                            case FilterType.Contains:
                            default:
                                add = line.Contains(filter, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                                break;
                            case FilterType.RegularExpression:
                                if (regexp != null)
                                {
                                    add = regexp.IsMatch(line);
                                }
                                break;
                        }

                        if (add)
                        {
                            this.TextContent.Items.Add(new LineText()
                            {
                                LineNumber = n,
                                Text = this.Highlight(line),
                            });
                        }

                        n++;
                    }
                }
                else
                {
                    var lineNumber = 1;
                    lines.ToList().ForEach(line =>
                    {
                        this.TextContent.Items.Add(new LineText()
                        {
                            LineNumber = lineNumber++,
                            Text = this.Highlight(line),
                        });
                    });
                }
            }

            if (this.AutoscrollToggle.IsChecked ?? true)
            {
                if (this.TextContent.Items.Any())
                {
                    this.TextContent.ScrollIntoView(this.TextContent.Items[this.TextContent.Items.Count - 1]);
                }
            }
        }

        /// <summary>
        /// Called when window should resize itself.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        public void OnSizeChanged(double width, double height)
        {
            this.CheckSizeConstraints(width, height);
        }

        /// <summary>
        /// Select a line at the specified line number. If the line number does not exist,
        /// due to filtering, ignore the request.
        /// </summary>
        /// <param name="lineNumber">Line number to select.</param>
        public void SelectLine(int lineNumber)
        {
            var item = this.TextContent.Items.Where(i => (i as LineText).LineNumber.Equals(lineNumber));
            if (item.Any())
            {
                this.AutoscrollToggle.IsChecked = false;
                this.TextContent.SelectedItem = item.First();
                this.TextContent.ScrollIntoView(this.TextContent.SelectedItem);
            }
        }

        /// <summary>
        /// Called when container grid manipulation starts.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ContainerGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // Check if we are resizing.
            this.isResizing = e.Position.X > Width - this.ResizeRectangle.Width && 
                              e.Position.Y > Height - this.ResizeRectangle.Height;

            // Check if we can move.
            this.canMove = e.Position.X > Width - this.MoveRectangle.ActualWidth && 
                           e.Position.Y < this.MoveRectangle.ActualHeight;
        }

        /// <summary>
        /// Called when container grid is being manipulated.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ContainerGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (this.isResizing)
            {
                this.Width = Math.Max(250, Width + e.Delta.Translation.X);
                this.Height = Math.Max(350, Height + e.Delta.Translation.Y);
                CheckSizeConstraints(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
            else if (canMove)
            {
                var x = Canvas.GetLeft(this) + e.Delta.Translation.X;
                var y = Canvas.GetTop(this) + e.Delta.Translation.Y;
                CheckPositionConstraints(x, y);
            }
        }

        /// <summary>
        /// Checks size constraints and limits them to the size of the application window.
        /// </summary>
        /// <param name="width">Window width.</param>
        /// <param name="height">Window height.</param>
        private void CheckSizeConstraints(double width, double height)
        {
            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);
            if (x + this.Width > width) this.Width = Math.Max(1, width - x);
            if (y + this.Height > height - 80) this.Height = Math.Max(1, height - y - 80);
        }

        /// <summary>
        /// Checks position constraints and limits them to the application window.
        /// </summary>
        /// <param name="x">Window X position.</param>
        /// <param name="y">Window Y position.</param>
        private void CheckPositionConstraints(double x, double y)
        {
            if (x < 0) x = 0;
            if (x + this.Width > Window.Current.Bounds.Width) x = Window.Current.Bounds.Width - this.Width;
            if (y < 0) y = 0;
            if (y + this.Height > Window.Current.Bounds.Height - 80) y = Window.Current.Bounds.Height - this.Height - 80;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        /// <summary>
        /// Called when a line is selected.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void LineSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var lineNumber = (e.AddedItems.First() as LineText).LineNumber;
                this.owner.OnLineSelected(this.Id, lineNumber);
            }
        }

        /// <summary>
        /// Show configuration.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {
            this.TextContent.Visibility = Visibility.Collapsed;
            this.ConfigView.Visibility = Visibility.Visible;
            this.SlideInStoryboard.Begin();
        }

        /// <summary>
        /// Close configuration.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void CloseConfiguration(object sender, RoutedEventArgs e)
        {
            this.TextContent.Visibility = Visibility.Visible;
            this.ConfigView.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Called when filter type is checked.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void FilterTypeChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                var type = rb.Tag.ToString().ToFilterType();
                this.Configuration.Type = type;
            }
        }

        /// <summary>
        /// Called when "ignore case" is checked.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void IgnoreCaseChecked(object sender, RoutedEventArgs e)
        {
            this.Configuration.IgnoreCase = true;
        }

        /// <summary>
        /// Called when "ignore case" is unchecked.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void IgnoreCaseUnchecked(object sender, RoutedEventArgs e)
        {
            this.Configuration.IgnoreCase = false;
        }

        /// <summary>
        /// Applies configuration.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ApplyConfiguration(object sender, RoutedEventArgs e)
        {
            this.Configuration.Filter = this.Filter.Text;
            this.highlights[0].Text = this.Highlight1.Text;
            this.highlights[1].Text = this.Highlight2.Text;
            this.highlights[2].Text = this.Highlight3.Text;

            this.CloseConfiguration(null, null);
            
            this.forceTextRedraw = true;
        }

        /// <summary>
        /// Closes and removes window.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Stream.Configuration.Configuration.RemoveWindowConfiguration(this.Id);
            this.owner.RemoveWindow(this);
        }

        /// <summary>
        /// Called when the mouse is pressed inside the container grid. This will move the
        /// window to the front of the rendering stack.
        /// </summary>
        /// <param name="sender">Event origin.</param>
        /// <param name="e">Event arguments.</param>
        private void ContainerGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.owner.MoveWindowToFront(this);
        }

        /// <summary>
        /// Highlights text.
        /// </summary>
        /// <param name="text">Text to highlight.</param>
        /// <returns></returns>
        private string Highlight(string text)
        {
            var result = text;
            foreach (var highlight in this.highlights)
            {
                result = highlight.HighlightText(result);
            }

            return result;
        }

        private bool isResizing;
        private bool canMove;
        private bool forceTextRedraw;
        private int textLength;

        private readonly MainPage owner;
        private readonly IList<Highlight> highlights = new List<Highlight>()
        {
            new Highlight() { Text = string.Empty, Color = Colors.Salmon },
            new Highlight() { Text = string.Empty, Color = Colors.ForestGreen },
            new Highlight() { Text = string.Empty, Color = Colors.DeepSkyBlue }
        };
    }
}
