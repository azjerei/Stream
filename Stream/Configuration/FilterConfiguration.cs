namespace Stream.Configuration
{
    /// <summary>
    /// Filter configuration.
    /// </summary>
    public class FilterConfiguration
    {
        /// <summary>
        /// Gets or sets filter string.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets filter type.
        /// </summary>
        public FilterType Type { get; set; }

        /// <summary>
        /// Gets or sets whether to ignore filter string case.
        /// </summary>
        public bool IgnoreCase { get; set; }
    }
}
