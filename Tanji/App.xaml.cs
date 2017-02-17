using System;
using System.Windows;
using System.Collections.Generic;

using Tanji.Services;
using Tanji.Windows.Logger;
using Tanji.Services.Connection;

using Tangine.Habbo;
using Tangine.Modules;

using Sulakore.Communication;
using Sulakore.Habbo.Web;
using Sulakore.Modules;

namespace Tanji
{
    public partial class App : Application, IMaster
    {
        private readonly List<IHaltable> _haltables;
        private readonly SortedList<int, IReceiver> _receivers;

        public HGame Game { get; set; }
        HGame ITContext.Game => Game;

        public HGameData GameData { get; set; }

        public HConnection Connection { get; }
        IHConnection IContractor.Connection => Connection;

        public static IMaster Master { get; private set; }

        public App()
        {
            _haltables = new List<IHaltable>();
            _receivers = new SortedList<int, IReceiver>();

            GameData = new HGameData();
            Connection = new HConnection();

            Connection.Connected += Connected;
            Connection.DataOutgoing += HandleData;
            Connection.DataIncoming += HandleData;
            Connection.Disconnected += Disconnected;
        }

        public void AddHaltable(IHaltable haltable)
        {
            _haltables.Add(haltable);
        }
        public void AddReceiver(IReceiver receiver)
        {
            switch (receiver.GetType().Name)
            {
                case nameof(ConnectionViewModel):
                _receivers.Add(0, receiver);
                break;

                case nameof(LoggerViewModel):
                _receivers.Add(1, receiver);
                break;
            }
        }

        private void Connected(object sender, EventArgs e)
        {
            foreach (IHaltable haltable in _haltables)
            {
                haltable.Dispatcher.Invoke(haltable.Restore);
            }
        }
        private void Disconnected(object sender, EventArgs e)
        {
            foreach (IHaltable haltable in _haltables)
            {
                haltable.Dispatcher.Invoke(haltable.Halt);
            }
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

        protected override void OnStartup(StartupEventArgs e)
        {
            Master = this;
            base.OnStartup(e);
        }
    }
}