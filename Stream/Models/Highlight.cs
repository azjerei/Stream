using System.Text.RegularExpressions;
using Windows.UI;

namespace Stream.Models
{
    /// <summary>
    /// Text highlight.
    /// </summary>
    public class Highlight
    {
        /// <summary>
        /// Gets or sets text to highlight.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets color of highlighted text.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Converts a regular text string into a highlighted text string.
        /// </summary>
        /// <param name="text">Text to convert.</param>
        /// <returns>Text with highlights.</returns>
        public string HighlightText(string text)
        {
            try
            {
                var formattedText = Regex.Escape(this.Text);
                if (!string.IsNullOrEmpty(formattedText))
                {
                    var format = string.Join("|", formattedText.Split(","));
                    var regex = new Regex(@"(?:" + format + ")");
                    foreach (var match in regex.Matches(text))
                    {
                        text = text.Replace(match.ToString(), $"[;{this.Color}{match};]");
                    }
                }
            }
            catch
            {
            }

            return text;
        }
    }
}
