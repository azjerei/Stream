namespace Stream.Models
{
    public class LineText
    {
        public int LineNumber { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as LineText;

            return 
                this.LineNumber == other.LineNumber &&
                this.Text.Equals(other.Text);
        }

        public override int GetHashCode()
        {
            return this.LineNumber.GetHashCode() + this.Text.GetHashCode();
        }
    }
}
