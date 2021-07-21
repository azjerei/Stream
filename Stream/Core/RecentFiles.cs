using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.StartScreen;

namespace Stream.Core
{
    /// <summary>
    /// Wrapper for MRU (Most Recently Used) files and jump list.
    /// </summary>
    public static class RecentFiles
    {
        /// <summary>
        /// Initialize recent files. This will setup the jump list.
        /// </summary>
        public static async Task InitializeAsync()
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;
            jumpList.Items.Clear();

            // Add recent files to the jump list.
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var recentFiles = await GetFilesAsync();
            foreach (var file in recentFiles)
            {
                var item = await mru.GetFileAsync(file.Value);
                var jpItem = JumpListItem.CreateWithArguments(file.Value, item.Path);
                jpItem.GroupName = "Recent files";
                jumpList.Items.Add(jpItem);
            }

            await jumpList.SaveAsync();
        }

        /// <summary>
        /// Adds a file to the MRU and jump lists.
        /// </summary>
        /// <param name="file"></param>
        public static async void AddFileAsync(IStorageItem file)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var token = mru.Add(file);

            await AddToJumpListAsync(token, file.Path);
        }

        /// <summary>
        /// Gets a file from the MRU list.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetFileAsync(string token)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return await mru.GetFileAsync(token);
        }

        /// <summary>
        /// Gets all recent files.
        /// </summary>
        /// <returns></returns>
        public static async Task<IDictionary<string, string>> GetFilesAsync()
        {
            var files = new Dictionary<string, string>();

            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var entry in mru.Entries)
            {
                var file = await mru.GetFileAsync(entry.Token);
                files.Add(file.Path, entry.Token);
            }

            return files;
        }

        /// <summary>
        /// Adds an item to the jump list.
        /// </summary>
        /// <param name="token">Item's MRU token.</param>
        /// <param name="name">Item name.</param>
        /// <returns></returns>
        private static async Task AddToJumpListAsync(string token, string name)
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;
            var jpItem = JumpListItem.CreateWithArguments(token, name);
            jpItem.GroupName = "Recent files";
            jumpList.Items.Add(jpItem);
        }
    }
}
