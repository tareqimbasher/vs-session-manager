using SessionManagerExtension.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace SessionManagerExtension.WPF.Converters
{
    public class PathRelativeOrFullConverter : IValueConverter
    {
        private IIDE _ide;

        private IIDE IDE
        {
            get
            {
                if (_ide == null)
                    _ide = SessionManagerPackage.ServiceProvider.GetService(typeof(IIDE)) as IIDE;
                return _ide;
            }
        }

        public PathRelativeOrFullConverter()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string path))
            {
                return value;
            }

            var solutionPath = IDE.Environment.Solution?.FullName;
            if (string.IsNullOrEmpty(solutionPath))
            {
                return value;
            }
            else
            {
                var solutionDirectoryPath = Path.GetDirectoryName(solutionPath);
                if (path.Contains(solutionDirectoryPath) == false)
                {
                    return value;
                }
                else
                {
                    return path.Replace(solutionDirectoryPath, string.Empty);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
