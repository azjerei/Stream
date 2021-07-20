using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Stream.Files
{
    public static class FileCache
    {
        public static IDictionary<string, string> Files => files;

        public static async Task InitializeAsync()
        {
            files = new Dictionary<string, string>();
            await ReadCacheAsync();

            foreach (var file in files)
            {
                await JumpList.AddItemAsync(file.Key, file.Value);
            }
        }

        public static void AddToCache(IStorageItem file)
        {
            if (!files.ContainsKey(file.Path))
            {
                var mru = StorageApplicationPermissions.MostRecentlyUsedList;
                var token = mru.Add(file);

                files.Add(token, file.Path);
                WriteCacheAsync();

                JumpList.AddItemAsync(file.Path, token).Wait();
            }
        }

        public static async Task<StorageFile> GetFromCacheAsync(string token)
        {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return await mru.GetFileAsync(token);
        }

        private static async Task ReadCacheAsync()
        {
            try
            {
                var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var cacheFile = await storageFolder.GetFileAsync(cache);
                var cachedFiles = await Windows.Storage.FileIO.ReadLinesAsync(cacheFile);

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

        private static async void WriteCacheAsync()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var cacheFile = await storageFolder.GetFileAsync(cache);
            var persistFiles = files.Take(10);
            var fileList = new List<string>();

            foreach (var file in persistFiles)
            {
                fileList.Add($"{file.Key}|{file.Value}");
            }

            await Windows.Storage.FileIO.WriteLinesAsync(cacheFile, fileList);
        }

        private static async void CreateCacheAsync()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(cache, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }

        private static IDictionary<string, string> files;
        private static readonly string cache = "cache.txt";
    }
}
