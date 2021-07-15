using System;

namespace Stream.Configuration
{
    public enum FilterType
    {
        StartsWith,
        EndsWith,
        Contains,
        RegularExpression
    }

    public static class FilterTypeExtensions
    {
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