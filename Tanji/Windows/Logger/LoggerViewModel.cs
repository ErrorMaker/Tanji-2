using System;
using System.Drawing;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms.Integration;

using Tanji.Helpers;
using Tanji.Services;
using Tanji.Controls;

using Tangine.Habbo;
using Tangine.Network;
using Tangine.Protocol;

namespace Tanji.Windows.Logger
{
    public class LoggerViewModel : ObservableObject, IHaltable, IReceiver
    {
        private readonly object _writeQueueLock;
        private readonly object _processQueueLock;
        private readonly HPacketLogger _packetLogger;
        private readonly Queue<DataInterceptedEventArgs> _intercepted;
        private readonly Dictionary<int, MessageItem> _ignoredMessages;
        private readonly Action<List<Tuple<string, Color>>> _displayEntry;

        public Color DetailHighlight { get; set; } = Color.DarkGray;
        public Color IncomingHighlight { get; set; } = Color.FromArgb(178, 34, 34);
        public Color OutgoingHighlight { get; set; } = Color.FromArgb(0, 102, 204);
        public Color StructureHighlight { get; set; } = Color.FromArgb(0, 204, 136);

        public bool IsReceiving
        {
            get
            {
                return (Visibility == Visibility.Visible &&
                    (IsViewingOutgoing || IsViewingIncoming));
            }
        }

        private WindowsFormsHost _packetLoggerHost = new WindowsFormsHost();
        public WindowsFormsHost PacketLoggerHost
        {
            get
            {
                if (_packetLoggerHost.Child == null)
                {
                    _packetLoggerHost.Child = _packetLogger;
                }
                return _packetLoggerHost;
            }
        }

        private Visibility _visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get
            {
                if (App.Master == null)
                {
                    return Visibility.Visible;
                }
                return _visibility;
            }
            set
            {
                _visibility = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingBlocked = true;
        public bool IsDisplayingBlocked
        {
            get { return _isDisplayingBlocked; }
            set
            {
                _isDisplayingBlocked = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingReplaced = true;
        public bool IsDisplayingReplaced
        {
            get { return _isDisplayingReplaced; }
            set
            {
                _isDisplayingReplaced = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingHash = true;
        public bool IsDisplayingHash
        {
            get { return _isDisplayingHash; }
            set
            {
                _isDisplayingHash = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingStructure = true;
        public bool IsDisplayingStructure
        {
            get { return _isDisplayingStructure; }
            set
            {
                _isDisplayingStructure = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingTimestamp = false;
        public bool IsDisplayingTimestamp
        {
            get { return _isDisplayingTimestamp; }
            set
            {
                _isDisplayingTimestamp = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingParserName = true;
        public bool IsDisplayingParserName
        {
            get { return _isDisplayingParserName; }
            set
            {
                _isDisplayingParserName = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingMessageName = true;
        public bool IsDisplayingMessageName
        {
            get { return _isDisplayingMessageName; }
            set
            {
                _isDisplayingMessageName = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isViewingOutgoing = true;
        public bool IsViewingOutgoing
        {
            get { return _isViewingOutgoing; }
            set
            {
                _isViewingOutgoing = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isViewingIncoming = true;
        public bool IsViewingIncoming
        {
            get { return _isViewingIncoming; }
            set
            {
                _isViewingIncoming = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isAlwaysOnTop = false;
        public bool IsAlwaysOnTop
        {
            get { return _isAlwaysOnTop; }
            set
            {
                _isAlwaysOnTop = value;
                if (Application.Current?.MainWindow != null)
                {
                    Application.Current.MainWindow.Topmost = value;
                }
                RaiseOnPropertyChanged();
            }
        }

        public Command FindCommand { get; }
        public Command IgnoreCommand { get; }
        public Command EmptyLogCommand { get; }

        public Command ToggleAlwaysOnTopCommand { get; }
        public Command ToggleViewOutgoingCommand { get; }
        public Command ToggleViewIncomingCommand { get; }

        public LoggerViewModel()
        {
            _displayEntry = DisplayEntries;
            _writeQueueLock = new object();
            _processQueueLock = new object();
            _packetLogger = new HPacketLogger();
            _intercepted = new Queue<DataInterceptedEventArgs>();
            _ignoredMessages = new Dictionary<int, MessageItem>();

            FindCommand = new Command(Find);
            IgnoreCommand = new Command(Ignore);
            EmptyLogCommand = new Command(EmptyLog);
            ToggleAlwaysOnTopCommand = new Command(ToggleAlwaysOnTop);
            ToggleViewOutgoingCommand = new Command(ToggleViewOutgoing);
            ToggleViewIncomingCommand = new Command(ToggleViewIncoming);
        }

        private void Find(object obj)
        { }
        private void Ignore(object obj)
        { }
        private void EmptyLog(object obj)
        {
            _packetLogger.LoggerTxt.Clear();
        }

        private void ToggleAlwaysOnTop(object obj)
        {
            IsAlwaysOnTop = !IsAlwaysOnTop;
        }
        private void ToggleViewIncoming(object obj)
        {
            IsViewingIncoming = !IsViewingIncoming;
        }
        private void ToggleViewOutgoing(object obj)
        {
            IsViewingOutgoing = !IsViewingOutgoing;
        }

        public void Halt()
        {
            Visibility = Visibility.Collapsed;
            _intercepted.Clear();
        }
        public void Restore()
        {
            _packetLogger.LoggerTxt.Clear();
            Visibility = Visibility.Visible;
        }

        public void HandleOutgoing(DataInterceptedEventArgs e)
        {
            PushToQueue(e);
        }
        public void HandleIncoming(DataInterceptedEventArgs e)
        {
            PushToQueue(e);
        }

        private void ProcessQueue()
        {
            while (IsReceiving && _intercepted.Count > 0)
            {
                DataInterceptedEventArgs args = _intercepted.Dequeue();
                if (!IsLoggingAuthorized(args)) continue;

                var entry = new List<Tuple<string, Color>>();
                if (args.IsBlocked)
                {
                    entry.Add(Tuple.Create("[Blocked]\r\n", DetailHighlight));
                }
                if (!args.IsOriginal)
                {
                    entry.Add(Tuple.Create("[Replaced]\r\n", DetailHighlight));
                }
                if (IsDisplayingTimestamp)
                {
                    entry.Add(Tuple.Create($"[{args.Timestamp.ToLongTimeString()}]\r\n", DetailHighlight));
                }
                MessageItem message = GetMessage(args);
                if (IsDisplayingHash && message != null)
                {
                    entry.Add(Tuple.Create($"[{message.MD5}]\r\n", DetailHighlight));
                }

                string arrow = "->";
                string title = "Outgoing";
                Color entryHighlight = OutgoingHighlight;
                if (!args.IsOutgoing)
                {
                    arrow = "<-";
                    title = "Incoming";
                    entryHighlight = IncomingHighlight;
                }

                entry.Add(Tuple.Create(title, entryHighlight));
                entry.Add(Tuple.Create($"({args.Packet.Header}, {args.Packet.Body.Count}", entryHighlight));
                if (message != null)
                {
                    if (IsDisplayingMessageName)
                    {
                        entry.Add(Tuple.Create(", ", entryHighlight));
                        entry.Add(Tuple.Create(message.Class.QName.Name, DetailHighlight));
                    }
                    if (!args.IsOutgoing && IsDisplayingParserName)
                    {
                        entry.Add(Tuple.Create(", ", entryHighlight));
                        entry.Add(Tuple.Create(message.Parser.QName.Name, DetailHighlight));
                    }
                }
                entry.Add(Tuple.Create(")", entryHighlight));
                entry.Add(Tuple.Create($" {arrow} ", DetailHighlight));
                entry.Add(Tuple.Create($"{args.Packet}\r\n", entryHighlight));

                if (IsDisplayingStructure && message?.Structure?.Length > 0)
                {
                    int position = 0;
                    HPacket packet = args.Packet;
                    string structure = ("{l}{u:" + packet.Header + "}");
                    foreach (string valueType in message.Structure)
                    {
                        switch (valueType.ToLower())
                        {
                            case "int":
                            structure += ("{i:" + packet.ReadInt32(ref position) + "}");
                            break;

                            case "string":
                            structure += ("{s:" + packet.ReadUTF8(ref position) + "}");
                            break;

                            case "double":
                            structure += ("{d:" + packet.ReadDouble(ref position) + "}");
                            break;

                            case "byte":
                            structure += ("{b:" + packet.ReadBytes(1, ref position)[0] + "}");
                            break;

                            case "boolean":
                            structure += ("{b:" + packet.ReadBoolean(ref position) + "}");
                            break;
                        }
                    }
                    if (packet.GetReadableBytes(position) == 0)
                    {
                        entry.Add(Tuple.Create(structure + "\r\n", StructureHighlight));
                    }
                }
                entry.Add(Tuple.Create("--------------------\r\n", DetailHighlight));

                while (!_packetLogger.IsHandleCreated) ;
                if (!IsReceiving) return;

                _packetLogger.BeginInvoke(_displayEntry, entry);
            }
        }
        private void PushToQueue(DataInterceptedEventArgs args)
        {
            lock (_writeQueueLock)
            {
                if (IsLoggingAuthorized(args))
                {
                    _intercepted.Enqueue(args);
                }
            }
            if (IsReceiving && Monitor.TryEnter(_processQueueLock))
            {
                try
                {
                    args.Continue(true);
                    while (IsReceiving && _intercepted.Count > 0)
                    {
                        ProcessQueue();
                    }
                }
                finally { Monitor.Exit(_processQueueLock); }
            }
        }

        private void DisplayEntries(List<Tuple<string, Color>> entry)
        {
            foreach (Tuple<string, Color> chunk in entry)
            {
                _packetLogger.LoggerTxt.SelectionStart = _packetLogger.LoggerTxt.TextLength;
                _packetLogger.LoggerTxt.SelectionLength = 0;

                _packetLogger.LoggerTxt.SelectionColor = chunk.Item2;
                _packetLogger.LoggerTxt.AppendText(chunk.Item1);
            }
        }
        private MessageItem GetMessage(DataInterceptedEventArgs args)
        {
            IDictionary<ushort, MessageItem> messages = (args.IsOutgoing ?
                App.Master.Game.OutMessages : App.Master.Game.InMessages);

            MessageItem message = null;
            messages.TryGetValue(args.Packet.Header, out message);

            return message;
        }
        private bool IsLoggingAuthorized(DataInterceptedEventArgs args)
        {
            if (!IsReceiving) return false;
            if (!IsDisplayingBlocked && args.IsBlocked) return false;
            if (!IsDisplayingReplaced && !args.IsOriginal) return false;

            if (!IsViewingOutgoing && args.IsOutgoing) return false;
            if (!IsViewingIncoming && !args.IsOutgoing) return false;

            if (_ignoredMessages.Count > 0)
            {
                int header = args.Packet.Header;
                if (!args.IsOutgoing)
                {
                    header = (ushort.MaxValue - header);
                }

                if (_ignoredMessages.ContainsKey(header)) return false;
            }
            return true;
        }
    }
}