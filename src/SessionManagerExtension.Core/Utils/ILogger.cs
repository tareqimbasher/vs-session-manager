using System;
using System.Collections.Generic;
using System.Text;

namespace SessionManagerExtension.Utils
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
        void ErrorAndShowPopupMessage(string message);
        void ErrorAndShowPopupMessage(string message, Exception exception);
    }
}
