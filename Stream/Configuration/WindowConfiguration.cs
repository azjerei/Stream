namespace Stream.Configuration
{
    /// <summary>
    /// Window configuration.
    /// </summary>
    public class WindowConfiguration
    {
        /// <summary>
        /// X position.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y position.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Window width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Window height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Filter configuration.
        /// </summary>
        public FilterConfiguration Filter { get; set; }
    }
}
