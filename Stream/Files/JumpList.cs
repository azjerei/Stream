using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace Stream.Files
{
    /// <summary>
    /// Jump List wrapper class.
    /// </summary>
    public static class JumpList
    {
        /// <summary>
        /// Initialize jump list.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
        {
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;
            jumpList.Items.Clear();
            await jumpList.SaveAsync();
        }

        /// <summary>
        /// Adds a file item to the jump list.
        /// </summary>
        /// <param name="name">File's name.</param>
        /// <param name="token">File's MRU token.</param>
        /// <returns></returns>
        public static async Task AddItemAsync(string name, string token)
        {
            await ClearExistingItem(name, token);

            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            var item = JumpListItem.CreateWithArguments(token, name);
            item.GroupName = "Recent files";
            jumpList.Items.Add(item);
            await jumpList.SaveAsync();
        }

        /// <summary>
        /// Clears an existing item from the jump list.
        /// </summary>
        /// <param name="name">File's name.</param>
        /// <param name="token">File's MRU token.</param>
        /// <returns></returns>
        private static async Task ClearExistingItem(string name, string token)
        {
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            var existing = jumpList.Items.Where(item => item.DisplayName.Equals(name));
            if (existing.Any()) {
                jumpList.Items.Remove(existing.First());
            }
        }
    }
}
