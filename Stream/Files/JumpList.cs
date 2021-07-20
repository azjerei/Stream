using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace Stream.Files
{
    public static class JumpList
    {
        public static async Task InitializeAsync()
        {
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;
            jumpList.Items.Clear();
            await jumpList.SaveAsync();
        }

        public static async Task AddItemAsync(string name, string token)
        {
            await ClearExistingItem(name, token);

            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            var item = JumpListItem.CreateWithArguments(token, name);
            item.GroupName = "Recent files";
            jumpList.Items.Add(item);
            await jumpList.SaveAsync();
        }

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
