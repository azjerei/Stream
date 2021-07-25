using Stream.Extensions;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Stream.Components
{
    /// <summary>
    /// Represents a row of text.
    /// </summary>
    public sealed partial class TextRow : UserControl
    {
        /// <summary>
        /// Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(TextRow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets text property.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set { SetValue(TextProperty, value); this.HighlightText(value); }
        }

        /// <summary>
        /// Line number dependency property.
        /// </summary>
        public static readonly DependencyProperty RowNumberProperty = DependencyProperty.Register(
            "LineNumber",
            typeof(int),
            typeof(TextRow),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets row number.
        /// </summary>
        public int RowNumber
        {
            get => (int)GetValue(RowNumberProperty);
            set => SetValue(RowNumberProperty, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextRow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Highlights text.
        /// </summary>
        /// <param name="text">Text to highlight.</param>
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
