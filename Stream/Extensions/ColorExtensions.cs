using System;
using Windows.UI;

namespace Stream.Extensions
{
    public static class ColorExtensions
    {
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
