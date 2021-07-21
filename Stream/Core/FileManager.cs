using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Stream.Core
{
    /// <summary>
    /// Manages opening of files, whether it is via file picker or cache.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="mode">Open file mode.</param>
        /// <param name="token">File's MRU token (only for <see cref="OpenFileMode.Cache"/>).</param>
        /// <returns></returns>
        public static async Task<StorageFile> OpenFileAsync(OpenFileMode mode, string token = null)
        {
            switch (mode)
            {
                case OpenFileMode.FilePicker:
                    return await OpenFileFromPickerAsync();
                case OpenFileMode.Cache:
                    return await OpenFileFromCacheAsync(token);
            }

            return null;
        }

        /// <summary>
        /// Shows a file picker dialog that allows the user to select a file to open.
        /// </summary>
        /// <returns></returns>
        private static async Task<StorageFile> OpenFileFromPickerAsync()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            openPicker.FileTypeFilter.Add("*");

            return await openPicker.PickSingleFileAsync();
        }

        /// <summary>
        /// Opens a file from the file cache.
        /// </summary>
        /// <param name="token">File's MRU token.</param>
        /// <returns></returns>
        private static async Task<StorageFile> OpenFileFromCacheAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(token);
            }

            return await RecentFiles.GetFileAsync(token);
        }
    }
}
