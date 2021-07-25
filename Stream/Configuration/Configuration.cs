using Newtonsoft.Json;
using Stream.Core;
using System.Threading.Tasks;

namespace Stream.Configuration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Application theme.
        /// </summary>
        public ThemeType Theme { get; set; }

        /// <summary>
        /// Saves configuration.
        /// </summary>
        public async void Save()
        {
            await FileManager.SaveFileAsync(ConfigFileName, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Loads configuration.
        /// </summary>
        /// <returns></returns>
        public static async Task<Configuration> LoadAsync()
        {
            var config = await FileManager.ReadLocalFileAsync(ConfigFileName);
            return config != null ? JsonConvert.DeserializeObject<Configuration>(config) : new Configuration();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private Configuration()
        {
            this.Theme = ThemeType.System;
        }

        private const string ConfigFileName = "app.config";
    }
}
