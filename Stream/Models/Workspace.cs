using Stream.Configuration;
using System.Collections.Generic;

namespace Stream.Models
{
    /// <summary>
    /// Workspace model.
    /// </summary>
    public class Workspace
    {
        /// <summary>
        /// Gets or sets workspace name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets window configurations.
        /// </summary>
        public IList<WindowConfiguration> Windows { get; set; } = new List<WindowConfiguration>();
    }
}
