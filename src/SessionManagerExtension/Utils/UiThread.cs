using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SessionManagerExtension.Utils
{
    internal static class UiThread
    {
        private static SynchronizationContext _syncContext;

        internal static void Capture()
        {
            // This must be called from the UI thread!
            ThreadHelper.ThrowIfNotOnUIThread();

            _syncContext = SynchronizationContext.Current;
        }

        public static void Execute(Action action)
        {
            _syncContext.Post(o => action(), null);
        }
    }
}
