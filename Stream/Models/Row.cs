namespace Stream.Models
{
    /// <summary>
    /// A row of text.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Gets or sets row number.
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Text { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as Row;

            return 
                this.RowNumber == other.RowNumber &&
                this.Text.Equals(other.Text);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.RowNumber.GetHashCode() + this.Text.GetHashCode();
        }
    }
}
