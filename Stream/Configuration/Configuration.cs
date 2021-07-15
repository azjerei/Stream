using Newtonsoft.Json;
using Stream.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Stream.Configuration
{
    public static class Configuration
    {
        public static IDictionary<Guid, WindowConfiguration> Windows { get; set; } = new Dictionary<Guid, WindowConfiguration>();

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
                    Filter = window.Configuration
                });
        }

        public static void RemoveWindowConfiguration(Guid id)
        {
            Windows.Remove(id);
        }
    }
}
