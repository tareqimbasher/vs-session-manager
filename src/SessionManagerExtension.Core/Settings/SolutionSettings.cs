using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SessionManagerExtension.Settings
{
    /// <summary>
    /// Represents the configuration of a solution.
    /// </summary>
    public class SolutionSettings
    {
        public SolutionSettings()
        {
            Sessions = new ObservableCollection<Session>();
        }

        /// <summary>
        /// List of Sessions for this solution.
        /// </summary>
        public ObservableCollection<Session> Sessions { get; set; }

        /// <summary>
        /// An empty instance of <see cref="SolutionSettings"/>.
        /// </summary>
        public static SolutionSettings Empty => new SolutionSettings();
    }
}
