using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stream.Files
{
    public class FileCache
    {
        public IList<string> Files => this.files;

        public FileCache()
        {
            this.files = new List<string>();
            this.ReadCacheAsync();
        }

        public void AddToCache(string file)
        {
            if (!this.files.Contains(file))
            {
                this.files.Insert(0, file);
                this.WriteCacheAsync();
            }
        }

        private async void ReadCacheAsync()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                var cacheFile = await storageFolder.GetFileAsync(this.cache);
                var cache = await Windows.Storage.FileIO.ReadLinesAsync(cacheFile);
                foreach (var file in cache)
                {
                    this.files.Add(file);
                }
            }
            catch (FileNotFoundException)
            {
                this.CreateCacheAsync();
            }
        }

        private async void WriteCacheAsync()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var cacheFile = await storageFolder.GetFileAsync(this.cache);
            await Windows.Storage.FileIO.WriteLinesAsync(cacheFile, this.files.Take(10));
        }

        private async void CreateCacheAsync()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(this.cache, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }

        private IList<string> files;
        private readonly string cache = "cache.txt";
    }
}
