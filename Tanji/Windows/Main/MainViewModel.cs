using System;
using System.Windows;
using System.Collections.Generic;

using Tanji.Helpers;
using Tanji.Services;
using Tanji.Windows.Logger;
using Tanji.Services.Connection;

using Sulakore.Modules;
using Sulakore.Habbo.Web;
using Sulakore.Communication;

using Tangine.Habbo;

namespace Tanji.Windows.Main
{
    public class MainViewModel : ObservableObject, IMaster
    {
        private readonly PacketLogger _logger;
        private readonly List<IHaltable> _haltables;
        private readonly SortedList<int, IReceiver> _receivers;

        private string _title = "Tanji - Disconnected";
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isAlwaysOnTop = true;
        public bool IsAlwaysOnTop
        {
            get { return _isAlwaysOnTop; }
            set
            {
                _isAlwaysOnTop = value;
                RaiseOnPropertyChanged();
            }
        }

        public HGame Game { get; set; }
        public HGameData GameData { get; }
        public HConnection Connection { get; }
        IHConnection IContractor.Connection => Connection;

        public MainViewModel()
        {
            _haltables = new List<IHaltable>();
            _receivers = new SortedList<int, IReceiver>();

            _logger = new PacketLogger(this);

            App.Master = this;
            GameData = new HGameData();
            Connection = new HConnection();

            Connection.Connected += Connected;
            Connection.DataOutgoing += HandleData;
            Connection.DataIncoming += HandleData;
            Connection.Disconnected += Disconnected;
        }

        public void Halt()
        {
            IsAlwaysOnTop = true;
            Title = "Tanji - Disconnected";

            _haltables.ForEach(h => h.Halt());
        }
        public void Restore()
        {
            IsAlwaysOnTop = _logger.IsAlwaysOnTop;
            Title = $"Tanji - Connected[{Connection.RemoteEndPoint}]";

            _haltables.ForEach(h => h.Restore());
        }

        public void AddHaltable(IHaltable haltable)
        {
            _haltables.Add(haltable);
        }
        public void AddReceiver(IReceiver receiver)
        {
            if (_receivers.ContainsValue(receiver)) return;
            switch (receiver.GetType().Name)
            {
                case nameof(ConnectionViewModel):
                _receivers.Add(0, receiver);
                break;

                case nameof(PacketLogger):
                _receivers.Add(1, receiver);
                break;
            }
        }

        private void Connected(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(Restore);
        }
        private void Disconnected(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(Halt);
        }
        private void HandleData(object sender, DataInterceptedEventArgs e)
        {
            if (_receivers.Count == 0) return;
            foreach (IReceiver receiver in _receivers.Values)
            {
                if (!receiver.IsReceiving) continue;
                if (e.IsOutgoing)
                {
                    receiver.HandleOutgoing(e);
                }
                else
                {
                    receiver.HandleIncoming(e);
                }
            }
        }
    }
}