using System.Windows;
using System.ComponentModel;

namespace Tanji.Windows.Logger
{
    public partial class LoggerView : Window
    {
        public LoggerView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;

            base.OnClosing(e);
        }
    }
}