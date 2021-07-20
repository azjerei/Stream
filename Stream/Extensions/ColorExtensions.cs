using System;
using Windows.UI;

namespace Stream.Extensions
{
    /// <summary>
    /// Color extensions.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Resolve a <see cref="Windows.UI.Color"/> from a hex string (e.g. FF00FF).
        /// </summary>
        /// <param name="hex">Color hex string (e.g. FF00FF).</param>
        /// <returns></returns>
        public static Color ToColor(this string hex)
        {
            hex = hex.Replace("#", string.Empty);
            
            var a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            var r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            var g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            var b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));

            return Color.FromArgb(a, r, g, b);
        }
    }
}
