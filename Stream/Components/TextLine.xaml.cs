using Stream.Extensions;
using Stream.Models;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Stream.Components
{
    public sealed partial class TextLine : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(TextLine),
            new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); this.HighlightText(value); }
        }

        public static readonly DependencyProperty LineNumberProperty = DependencyProperty.Register(
            "LineNumber",
            typeof(int),
            typeof(TextLine),
            new PropertyMetadata(null));

        public int LineNumber
        {
            get { return (int)GetValue(LineNumberProperty); }
            set { SetValue(LineNumberProperty, value); }
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
    }
}
