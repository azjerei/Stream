using Stream.Configuration;
using Stream.Extensions;
using Stream.Models;
using Stream.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Stream.Components
{
    public sealed partial class ResizableWindow : UserControl
    {
        public Guid Id { get; }

        public FilterConfiguration Configuration { get; }

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

        public void SetText(string text)
        {
            if (text.Length > this.textLength || this.forceTextRedraw)
            {
                this.textLength = text.Length;
                this.forceTextRedraw = false;

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
                            textLines.Add($"{n,-10} {line}");
                        }

                        n++;
                    }

                    var lineNumber = 1;
                    textLines.ForEach((line =>
                    {
                        this.TextContent.Items.Add(new TextLine()
                        {
                            LineText = new LineText()
                            {
                                LineNumber = lineNumber++,
                                Text = line,
                            },
                        });
                    }));
                }
                else
                {
                    var lineNumber = 1;
                    lines.ToList().ForEach(line =>
                    {
                        this.TextContent.Items.Add(new TextLine()
                        {
                            LineText = new LineText()
                            {
                                LineNumber = lineNumber++,
                                Text = line,
                            },
                        });
                    });
                }
            }

            if (this.AutoscrollToggle.IsChecked ?? true)
            {
                this.TextContent.ScrollIntoView(this.TextContent.Items[this.TextContent.Items.Count - 1]);
            }
        }

        public void OnSizeChanged(double width, double height)
        {
            CheckSizeConstraints(width, height);
        }

        public async Task SelectLineAsync(int lineNumber)
        {
            var item = this.TextContent.Items.Where(i => (i as TextLine).LineText.LineNumber.Equals(lineNumber));
            if (item.Any())
            {
                this.AutoscrollToggle.IsChecked = false;
                this.TextContent.SelectedItem = item.First();
                //await this.TextContent.ScrollToItemAsync(this.TextContent.SelectedItem);
                this.TextContent.ScrollIntoView(this.TextContent.SelectedItem);
            }
        }

        private void ContainerGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            this.isResizing = e.Position.X > Width - this.ResizeRectangle.Width && 
                              e.Position.Y > Height - this.ResizeRectangle.Height;

            this.canMove = e.Position.X > Width - this.MoveRectangle.ActualWidth && 
                           e.Position.Y < this.MoveRectangle.ActualHeight;
        }

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
        private void CheckSizeConstraints(double width, double height)
        {
            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);
            if (x + this.Width > width) this.Width = Math.Max(1, width - x);
            if (y + this.Height > height - 80) this.Height = Math.Max(1, height - y - 80);
        }

        private void CheckPositionConstraints(double x, double y)
        {
            if (x < 0) x = 0;
            if (x + this.Width > Window.Current.Bounds.Width) x = Window.Current.Bounds.Width - this.Width;
            if (y < 0) y = 0;
            if (y + this.Height > Window.Current.Bounds.Height - 80) y = Window.Current.Bounds.Height - this.Height - 80;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        private void LineSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var lineNumber = (e.AddedItems.First() as TextLine).LineText.LineNumber;
                this.owner.OnLineSelected(this.Id, lineNumber);
            }
        }

        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {
            this.TextContent.Visibility = Visibility.Collapsed;
            this.ConfigView.Visibility = Visibility.Visible;
            this.SlideInStoryboard.Begin();
        }

        private void CloseConfiguration(object sender, RoutedEventArgs e)
        {
            this.TextContent.Visibility = Visibility.Visible;
            this.ConfigView.Visibility = Visibility.Collapsed;
        }

        private void FilterTypeChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                var type = rb.Tag.ToString().ToFilterType();
                this.Configuration.Type = type;
            }
        }

        private void IgnoreCaseChecked(object sender, RoutedEventArgs e)
        {
            this.Configuration.IgnoreCase = true;
        }

        private void IgnoreCaseUnchecked(object sender, RoutedEventArgs e)
        {
            this.Configuration.IgnoreCase = false;
        }

        private void ApplyConfiguration(object sender, RoutedEventArgs e)
        {
            this.Configuration.Filter = this.Filter.Text;
            //this.highlights[0].Text = this.Highlight1.Text;
            //this.highlights[1].Text = this.Highlight2.Text;
            //this.highlights[2].Text = this.Highlight3.Text;

            CloseConfiguration(null, null);
            
            this.forceTextRedraw = true;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Stream.Configuration.Configuration.RemoveWindowConfiguration(this.Id);
            this.owner.RemoveWindow(this);
        }

        private bool isResizing;
        private bool canMove;
        private bool forceTextRedraw;
        private int textLength;

        private readonly MainPage owner;
        private readonly IList<Highlight> highlights = new List<Highlight>()
        {
            new Highlight() { Text = string.Empty, Color = Colors.Salmon },
            new Highlight() { Text = string.Empty, Color = Colors.BlueViolet },
            new Highlight() { Text = string.Empty, Color = Colors.DeepSkyBlue }
        };
    }
}
