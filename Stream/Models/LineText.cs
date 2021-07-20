namespace Stream.Models
{
    /// <summary>
    /// A line of text.
    /// </summary>
    public class LineText
    {
        /// <summary>
        /// Gets or sets line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Text { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as LineText;

            return 
                this.LineNumber == other.LineNumber &&
                this.Text.Equals(other.Text);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.LineNumber.GetHashCode() + this.Text.GetHashCode();
        }
    }
}
