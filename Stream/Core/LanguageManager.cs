using Windows.ApplicationModel.Resources;

namespace Stream.Core
{
    /// <summary>
    /// Language handler.
    /// </summary>
    public static class LanguageManager
    {
        /// <summary>
        /// Gets the string stored in the given resource.
        /// </summary>
        /// <param name="resource">Resource name.</param>
        /// <returns>Resource string.</returns>
        public static string GetString(string resource)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            return resourceLoader.GetString(resource);
        }
    }
}
