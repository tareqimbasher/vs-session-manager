using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SessionManagerExtension
{
    /// <summary>
    /// Base class for types that implement <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class NotifiesPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
