using Stream.Core;

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

        /// <summary>
        /// Gets name of filter type.
        /// </summary>
        /// <returns></returns>
        public string GetFilterType()
        {
            var type = string.Empty;

            if (!string.IsNullOrEmpty(this.Filter))
            {
                switch (this.Type)
                {
                    case FilterType.StartsWith:
                        type = $"[{LanguageManager.GetString("Filter_StartsWith/Content")}]:";
                        break;
                    case FilterType.EndsWith:
                        type = $"[{LanguageManager.GetString("Filter_EndsWith/Content")}]:";
                        break;
                    case FilterType.Contains:
                        type = $"[{LanguageManager.GetString("Filter_Contains/Content")}]:";
                        break;
                    case FilterType.RegularExpression:
                        type = $"[{LanguageManager.GetString("Filter_RegEx/Content")}]:";
                        break;
                    default:
                        break;
                }
            }

            return type;
        }
    }
}
