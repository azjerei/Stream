using Newtonsoft.Json;
using Stream.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Stream.Configuration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Gets configuration.
        /// </summary>
        public static IDictionary<Guid, WindowConfiguration> Windows { get; set; } = new Dictionary<Guid, WindowConfiguration>();

        /// <summary>
        /// Loads configuration.
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> LoadAsync()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("config.json");
                var json = await FileIO.ReadTextAsync(file);
                Windows = JsonConvert.DeserializeObject<IDictionary<Guid, WindowConfiguration>>(json);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Saves configuration.
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> SaveAsync()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync("config.json");
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(Windows));

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Adds a window configuration.
        /// </summary>
        /// <param name="window">Configuration owner.</param>
        public static void AddWindowConfiguration(ResizableWindow window)
        {
            Windows.Add(
                window.Id,
                new WindowConfiguration()
                {
                    X = Canvas.GetLeft(window),
                    Y = Canvas.GetTop(window),
                    Width = window.Width,
                    Height = window.Height,
                    Filter = window.FilterConfiguration
                });
        }

        /// <summary>
        /// Removes a window configuration.
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveWindowConfiguration(Guid id)
        {
            Windows.Remove(id);
        }
    }
}
