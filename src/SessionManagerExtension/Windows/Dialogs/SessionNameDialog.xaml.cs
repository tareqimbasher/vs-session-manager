using System;
using System.Windows;
using System.Windows.Input;

namespace SessionManagerExtension.Dialogs
{
    /// <summary>
    /// Interaction logic for NewSessionDialog.xaml
    /// </summary>
    public partial class SessionNameDialog : Window
    {
        private readonly string _nameSeed;

        public SessionNameDialog(string nameSeed = null)
        {
            _nameSeed = nameSeed;
            InitializeComponent();
        }

        public string SessionName => SessionNameTextbox.Text;

        protected override void OnInitialized(EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_nameSeed))
            {
                SessionNameTextbox.Text = _nameSeed;
                SessionNameTextbox.CaretIndex = int.MaxValue;
            }

            base.OnInitialized(e);
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SessionName))
            {
                return;
            }

            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SessionNameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnConfirm(null, null);
            }
        }
    }
}
