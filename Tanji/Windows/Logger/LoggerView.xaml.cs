using System.Windows;

using Tanji.Windows.Main;

namespace Tanji.Windows.Logger
{
    public partial class LoggerView : Window
    {
        private readonly PacketLogger _logger;

        public bool IsAlwaysOnTop
        {
            get { return _logger.IsAlwaysOnTop; }
            set { _logger.IsAlwaysOnTop = value; }
        }

        public LoggerView(MainViewModel mainVM)
        {
            InitializeComponent();

            _logger = new PacketLogger(mainVM);
            _logger.TopLevel = false;

            winFormHost.Child = _logger;
        }
    }
}