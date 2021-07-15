using System.Text.RegularExpressions;
using Windows.UI;

namespace Stream.Models
{
    public class Highlight
    {
        public string Text { get; set; }

        public Color Color { get; set; }

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
