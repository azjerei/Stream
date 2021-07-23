namespace Stream.Configuration
{
    /// <summary>
    /// Window configuration.
    /// </summary>
    public class WindowConfiguration
    {
        /// <summary>
        /// Gets or sets X position.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets Y position.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets filter configuration.
        /// </summary>
        public FilterConfiguration Filter { get; set; }

        /// <summary>
        /// Gets or sets highlight configuration.
        /// </summary>
        public HighlightConfiguration Highlight { get; set; }
    }
}
