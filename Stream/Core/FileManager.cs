using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Saves a file.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="content">File content.</param>
        /// <returns></returns>
        public static async Task SaveFileAsync(string name, string content)
        {
            var storage = ApplicationData.Current.LocalFolder;
            var file = await storage.CreateFileAsync(name);
            await FileIO.WriteTextAsync(file, content);
        }

        /// <summary>
        /// Gets list of local storage files based with provided extension.
        /// </summary>
        /// <param name="ext">Extension filter.</param>
        /// <returns>A readonly list of storage file names.</returns>
        public static IReadOnlyList<string> GetLocalFiles(string ext)
        {
            var storage = ApplicationData.Current.LocalFolder;
            var files = Directory.GetFiles(storage.Path);
            return files.Where(file => file.Contains(ext)).ToList();
        }

        /// <summary>
        /// Reads a local file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <returns>File content.</returns>
        public static async Task<string> ReadLocalFileAsync(string fileName)
        {
            var storage = ApplicationData.Current.LocalFolder;
            return await FileIO.ReadTextAsync(await storage.GetFileAsync(fileName));
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
