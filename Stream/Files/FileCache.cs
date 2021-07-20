using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Stream.Files
{
    /// <summary>
    /// File cache.
    /// </summary>
    public static class FileCache
    {
        /// <summary>
        /// Gets files.
        /// </summary>
        public static IDictionary<string, string> Files => files;

        /// <summary>
        /// Initializes cache.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
        {
            files = new Dictionary<string, string>();
            await ReadCacheAsync();

            foreach (var file in files)
            {
                await JumpList.AddItemAsync(file.Key, file.Value);
            }
        }

        /// <summary>
        /// Adds a file to the cache.
        /// </summary>
        /// <param name="file">File to add.</param>
        public static void AddToCache(IStorageItem file)
        {
            if (!files.ContainsKey(file.Path))
            {
                // Add the item to the MRU.
                var mru = StorageApplicationPermissions.MostRecentlyUsedList;
                var token = mru.Add(file);

                files.Add(token, file.Path);
                WriteCacheAsync();

                // Update the jump list.
                JumpList.AddItemAsync(file.Path, token).Wait();
            }
        }

        /// <summary>
        /// Gets a file from the cache.
        /// </summary>
        /// <param name="token">File's MRU token.</param>
        /// <returns><see cref="StorageFile"/> for the given token.</returns>
        public static async Task<StorageFile> GetFromCacheAsync(string token)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return await mru.GetFileAsync(token);
        }

        /// <summary>
        /// Reads file cache file.
        /// </summary>
        /// <returns></returns>
        private static async Task ReadCacheAsync()
        {
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                var cacheFile = await storageFolder.GetFileAsync(cache);
                var cachedFiles = await FileIO.ReadLinesAsync(cacheFile);

                foreach (var file in cachedFiles)
                {
                    var parts = file.Split('|');
                    files.Add(parts[1], parts[0]);
                }
            }
            catch (FileNotFoundException)
            {
                CreateCacheAsync();
            }
        }

        /// <summary>
        /// Writes file cache file.
        /// </summary>
        private static async void WriteCacheAsync()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var cacheFile = await storageFolder.GetFileAsync(cache);
            var persistFiles = files.Take(10);
            var fileList = new List<string>();

            foreach (var file in persistFiles)
            {
                fileList.Add($"{file.Key}|{file.Value}");
            }

            await FileIO.WriteLinesAsync(cacheFile, fileList);
        }

        /// <summary>
        /// Creates cache file.
        /// </summary>
        private static async void CreateCacheAsync()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(cache, CreationCollisionOption.ReplaceExisting);
        }

        private static IDictionary<string, string> files;
        private static readonly string cache = "cache.txt";
    }
}
