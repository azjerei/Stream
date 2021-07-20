using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Stream.Extensions
{
    /// <summary>
    /// Extension functionality for application title bar.
    /// </summary>
    public static class TitleBarExtensions
    {
        /// <summary>
        /// Replaces default application title bar with core application title bar view.
        /// Useful for getting a consistent theme across application proper and title bar.
        /// </summary>
        /// <param name="coreTitleBar">Title bar to extend.</param>
        public static void ExtendView(this CoreApplicationViewTitleBar coreTitleBar)
        {
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set transparent background on default title bar part that contains
            // the window control buttons (max, min close) so that they adapt to
            // the same color scheme.
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
    }
}
