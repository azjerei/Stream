using Windows.ApplicationModel.DataTransfer;

namespace Stream.Core
{
    /// <summary>
    /// Manages clipboard (copying text).
    /// </summary>
    public static class ClipboardManager
    {
        /// <summary>
        /// Copies text.
        /// </summary>
        /// <param name="text">Text to copy.</param>
        public static void CopyText(string text)
        {
            var dp = new DataPackage();
            dp.SetText(text);
            Clipboard.SetContent(dp);
        }
    }
}
