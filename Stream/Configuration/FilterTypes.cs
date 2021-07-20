using System;

namespace Stream.Configuration
{
    /// <summary>
    /// Filter types.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// String starts with X.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Strings ends with X.
        /// </summary>
        EndsWith,

        /// <summary>
        /// String contains X.
        /// </summary>
        Contains,

        /// <summary>
        /// String matches regex.
        /// </summary>
        RegularExpression
    }

    /// <summary>
    /// Filter type extensions.
    /// </summary>
    public static class FilterTypeExtensions
    {
        /// <summary>
        /// Resolves a <see cref="FilterType"/> from a string.
        /// </summary>
        /// <param name="val">String to resolve <see cref="FilterType"/> from.</param>
        /// <returns><see cref="FilterType"/></returns>
        public static FilterType ToFilterType(this string val)
        {
            if (val.Equals("StartsWith", StringComparison.OrdinalIgnoreCase))
            {
                return FilterType.StartsWith;
            }
            else if (val.Equals("EndsWith", StringComparison.OrdinalIgnoreCase))
            {
                return FilterType.EndsWith;
            }
            else if (val.Equals("Contains", StringComparison.OrdinalIgnoreCase))
            {
                return FilterType.Contains;
            }
            else if (val.Equals("RegExp", StringComparison.OrdinalIgnoreCase))
            {
                return FilterType.RegularExpression;
            }

            return FilterType.Contains;
        }
    }
}