using Stream.Configuration;
using Windows.UI.Xaml;

namespace Stream.Extensions
{
    /// <summary>
    /// Extensions for themes.
    /// </summary>
    public static class ThemeExtensions
    {
        /// <summary>
        /// Resolves a theme from a string.
        /// </summary>
        /// <param name="themeStr">The string.</param>
        /// <returns>A <see cref="ThemeType"/>.</returns>
        public static ThemeType ToTheme(this string themeStr)
        {
            if (themeStr.Equals("Dark"))
            {
                return ThemeType.Dark;
            }
            else if (themeStr.Equals("Light"))
            {
                return ThemeType.Light;
            }

            return ThemeType.System;
        }

        /// <summary>
        /// Resolves an <see cref="ElementTheme"/> from a <see cref="ThemeType"/>.
        /// </summary>
        /// <param name="theme">The <see cref="ThemeType"/>.</param>
        /// <returns>An <see cref="ElementTheme"/>.</returns>
        public static ElementTheme ToElementTheme(this ThemeType theme)
        {
            switch (theme)
            {
                case ThemeType.Dark:
                    return ElementTheme.Dark;
                case ThemeType.Light:
                    return ElementTheme.Light;
                case ThemeType.System:
                default:
                    return ElementTheme.Default;
            }
        }
    }
}
