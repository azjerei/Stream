using Stream.Models;
using Windows.UI;

namespace Stream.Configuration
{
    /// <summary>
    /// Configuration for text highlighting.
    /// </summary>
    public class HighlightConfiguration
    {
        /// <summary>
        /// Gets highlight 1.
        /// </summary>
        public Highlight Highlight1 => this.highlight1;

        /// <summary>
        /// Gets highlight 2.
        /// </summary>
        public Highlight Highlight2 => this.highlight2;

        /// <summary>
        /// Gets highlight 3.
        /// </summary>
        public Highlight Highlight3 => this.highlight3;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HighlightConfiguration()
        {
            this.highlight1 = new Highlight { Text = string.Empty, Color = Colors.Salmon };
            this.highlight2 = new Highlight { Text = string.Empty, Color = Colors.ForestGreen };
            this.highlight3 = new Highlight { Text = string.Empty, Color = Colors.DeepSkyBlue };
        }

        private readonly Highlight highlight1;
        private readonly Highlight highlight2;
        private readonly Highlight highlight3;
    }
}
