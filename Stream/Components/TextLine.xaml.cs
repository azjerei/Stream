using Stream.Extensions;
using Stream.Models;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Stream.Components
{
    public sealed partial class TextLine : UserControl
    {
        public LineText LineText 
        {
            get => this.content;
            set
            {
                var highlightText = string.Empty;

                if (value.Text.Contains("[;"))
                {
                    highlightText = value.Text;
                    value.Text = string.Empty;
                }

                this.content = value;
                this.DataContext = value;

                if (!string.IsNullOrEmpty(highlightText))
                {
                    HighlightText(highlightText);
                }
            }
        }

        public TextLine()
        {
            this.InitializeComponent();
        }

        private void HighlightText(string text)
        {
            var segments = text.Split("[;");
            foreach (var segment in segments)
            {
                if (segment.StartsWith('#'))
                {
                    var subsegments = segment.Split(";]");

                    foreach (var seg in subsegments)
                    {
                        if (seg.StartsWith('#'))
                        {
                            var color = seg.Substring(0, "#AARRGGBB".Length);
                            var txt = seg.Substring(color.Length);

                            this.TextContent.Inlines.Add(
                                    new Run()
                                    {
                                        Text = txt,
                                        Foreground = new SolidColorBrush(color.ToColor()),
                                        FontWeight = FontWeights.Bold
                                    });
                        }
                        else
                        {
                            this.TextContent.Inlines.Add(
                                new Run()
                                {
                                    Text = seg
                                });
                        }
                    }
                }
                else
                {
                    this.TextContent.Inlines.Add(
                            new Run()
                            {
                                Text = segment
                            });
                }
            }
        }

        private LineText content;
    }
}
