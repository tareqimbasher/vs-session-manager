using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SessionManagerExtension.Utils
{
    public class DefaultLogger : ILogger
    {
        private readonly string _logMessageFormat = "[{0}] ({1}): {2}";
        private readonly SessionManagerPackage _package;
        private IVsOutputWindowPane _outputPane;

        public DefaultLogger(PackageAccessor packageAccessor)
        {
            _package = packageAccessor.Package;
        }

        public void Debug(string message)
        {
            Log(string.Format(_logMessageFormat, "DEBUG", DateTime.Now, message));
        }

        public void Info(string message)
        {
            Log(string.Format(_logMessageFormat, "INFO", DateTime.Now, message));
        }

        public void Error(string message)
        {
            Log(string.Format(_logMessageFormat, "ERROR", DateTime.Now, message));
        }

        public void ErrorAndShowPopupMessage(string message)
        {
            Error(message);
            MessageBox.Show(
                message,
                "Session Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void ErrorAndShowPopupMessage(string message, Exception exception)
        {
            Error(message);
            MessageBox.Show(
                $"{message}\n\nDetails: {exception}\n\nFind more details in the output window.",
                "Session Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void Log(string message)
        {
            _package.JoinableTaskFactory.RunAsync(async () =>
            {
                if (_outputPane == null)
                {
                    Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;

                    IVsOutputWindow outWindow = await _package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;

                    await _package.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (outWindow != null)
                    {
                        outWindow.GetPane(ref generalPaneGuid, out _outputPane);
                    }
                    else
                    {
                        return;
                    }
                }

                if (_outputPane == null)
                {
                    return;
                }

                _outputPane.OutputStringThreadSafe(message);
                _outputPane.Activate();
            });
        }
    }
}
