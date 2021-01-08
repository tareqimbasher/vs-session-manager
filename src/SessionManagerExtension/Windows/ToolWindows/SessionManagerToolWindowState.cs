using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SessionManagerExtension.Windows.ToolWindows
{
    public class SessionManagerToolWindowState : NotifiesPropertyChanged
    {
        private Session _selectedSession;

        public Session SelectedSession
        {
            get => _selectedSession;
            set => SetField(ref _selectedSession, value);
        }
    }
}
