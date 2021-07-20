using System;
using System.Timers;

namespace Stream.Core
{
    /// <summary>
    /// Timer that reloads the currently opened file.
    /// </summary>
    public class FileReloadTimer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="interval">Reload interval (milliseconds).</param>
        /// <param name="reloadFunc">Function to execute when timer expires.</param>
        public FileReloadTimer(int interval, Action reloadFunc)
        {
            this.timer = new Timer(interval);
            this.timer.Elapsed += (s, e) =>
            {
                reloadFunc();
            };
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        private readonly Timer timer;
    }
}
