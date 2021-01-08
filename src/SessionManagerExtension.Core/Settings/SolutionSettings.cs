using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SessionManagerExtension.Settings
{
    public class SolutionSettings
    {
        public SolutionSettings()
        {
            Sessions = new ObservableCollection<Session>();
        }

        public SolutionSettings(IEnumerable<Session> sessions) : this()
        {
            Sessions = new ObservableCollection<Session>(sessions);
        }

        public ObservableCollection<Session> Sessions { get; set; }

        public static SolutionSettings Empty => new SolutionSettings();
    }
}
