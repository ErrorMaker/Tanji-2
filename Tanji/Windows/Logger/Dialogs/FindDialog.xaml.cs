using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace Tanji.Windows.Logger.Dialogs
{
    public partial class FindDialog : Window
    {
        private readonly RichTextBox _loggerTxt;

        public FindDialog(RichTextBox loggerTxt)
        {
            _loggerTxt = loggerTxt;

            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;

            base.OnClosing(e);
        }
    }
}