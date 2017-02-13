using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Tanji.Services;
using Tanji.Windows.Main;

using Tangine.Habbo;

using Sulakore.Protocol;
using Sulakore.Communication;

namespace Tanji.Windows.Logger
{
    public partial class PacketLogger : Form, IReceiver, IHaltable, INotifyPropertyChanged
    {
        private readonly object _queueLock;
        private readonly MainViewModel _mainVM;
        private readonly Action<Queue<MessageEntry>> _displayEntries;
        private readonly Queue<DataInterceptedEventArgs> _intercepted;
        private readonly Dictionary<int, MessageItem> _ignoredMessages;

        private bool _isReceiving = false;
        public bool IsReceiving
        {
            get
            {
                return (_isReceiving &&
                    (IsViewingOutgoing || IsViewingIncoming));
            }
        }

        public bool IsDisplayingDetails
        {
            get
            {
                return (IsDisplayingHash ||
                  IsDisplayingStructure ||
                  IsDisplayingTimestamp ||
                  IsDisplayingParserClassName ||
                  IsDisplayingMessageClassName);
            }
        }
        public bool IsDisplayingFilters
        {
            get { return (IsDisplayingBlocked || IsDisplayingReplaced); }
        }

        public Color DetailHighlight { get; set; } = Color.DarkGray;
        public Color IncomingHighlight { get; set; } = Color.FromArgb(178, 34, 34);
        public Color OutgoingHighlight { get; set; } = Color.FromArgb(0, 102, 204);
        public Color StructureHighlight { get; set; } = Color.FromArgb(0, 204, 136);

        #region Binded Properties
        private bool _isDisplayingBlocked = true;
        public bool IsDisplayingBlocked
        {
            get { return _isDisplayingBlocked; }
            set
            {
                _isDisplayingBlocked = true;
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

        private bool _isDisplayingHash = false;
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

        private bool _isDisplayingParserClassName = true;
        public bool IsDisplayingParserClassName
        {
            get { return _isDisplayingParserClassName; }
            set
            {
                _isDisplayingParserClassName = value;
                RaiseOnPropertyChanged();
            }
        }

        private bool _isDisplayingMessageClassName = true;
        public bool IsDisplayingMessageClassName
        {
            get { return _isDisplayingMessageClassName; }
            set
            {
                _isDisplayingMessageClassName = value;
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
                TopMost = value;
                _mainVM.IsAlwaysOnTop = value;

                _isAlwaysOnTop = value;
                RaiseOnPropertyChanged();
            }
        }
        #endregion

        public PacketLogger(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            _mainVM.AddHaltable(this);
            _mainVM.AddReceiver(this);

            _queueLock = new object();
            _displayEntries = DisplayEntries;
            _intercepted = new Queue<DataInterceptedEventArgs>();
            _ignoredMessages = new Dictionary<int, MessageItem>();

            InitializeComponent();

            Bind(BlockedBtn, "Checked", nameof(IsDisplayingBlocked));
            Bind(ReplacedBtn, "Checked", nameof(IsDisplayingReplaced));

            Bind(HashBtn, "Checked", nameof(IsDisplayingHash));
            Bind(StructureBtn, "Checked", nameof(IsDisplayingStructure));
            Bind(TimestampBtn, "Checked", nameof(IsDisplayingTimestamp));
            Bind(ParserClassNameBtn, "Checked", nameof(IsDisplayingParserClassName));
            Bind(MessageClassNameBtn, "Checked", nameof(IsDisplayingMessageClassName));

            Bind(ViewOutgoingBtn, "Checked", nameof(IsViewingOutgoing));
            Bind(ViewIncomingBtn, "Checked", nameof(IsViewingIncoming));

            Bind(AlwaysOnTopBtn, "Checked", nameof(IsAlwaysOnTop));
        }

        private void EmptyLogBtn_Click(object sender, EventArgs e)
        {
            LoggerTxt.Clear();
        }
        private void LogWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var entries = new Queue<MessageEntry>();
            while (_intercepted.Count > 0 && _isReceiving)
            {
                DataInterceptedEventArgs args = _intercepted.Dequeue();
                if (!IsLoggingAuthorized(args)) continue;

                SortedDictionary<ushort, MessageItem> messages = (args.IsOutgoing ?
                    _mainVM.Game.OutMessages : _mainVM.Game.InMessages);

                var entry = new MessageEntry();
                entries.Enqueue(entry);

                if (IsDisplayingTimestamp)
                {
                    entry.AddWriteChunk($"[{DateTime.Now.ToLongTimeString()}]\r\n", DetailHighlight);
                }

                MessageItem message = null;
                messages.TryGetValue(args.Message.Header, out message);

                if (IsDisplayingHash && message != null)
                {
                    entry.AddWriteChunk($"[{message.MD5}]\r\n", DetailHighlight);
                }

                Color entryHighlight = (args.IsOutgoing ? OutgoingHighlight : IncomingHighlight);
                entry.AddWriteChunk((args.IsOutgoing ? "Outgoing" : "Incoming"), entryHighlight);

                if (args.IsBlocked)
                {
                    entry.AddWriteChunk("[Blocked]", DetailHighlight);
                }
                if (!args.IsOriginal)
                {
                    entry.AddWriteChunk("[Replaced]", DetailHighlight);
                }

                string arrow = (args.IsOutgoing ? "->" : "<-");
                entry.AddWriteChunk($"({args.Message.Header}, {args.Message.Length}", entryHighlight);

                if (message != null)
                {
                    if (IsDisplayingMessageClassName)
                    {
                        entry.AddWriteChunk(", ", entryHighlight);
                        entry.AddWriteChunk(message.Class.QName.Name, DetailHighlight);
                    }
                    if (!args.IsOutgoing && IsDisplayingParserClassName)
                    {
                        entry.AddWriteChunk(", ", entryHighlight);
                        entry.AddWriteChunk(message.Parser.QName.Name, DetailHighlight);
                    }
                }
                entry.AddWriteChunk($") {arrow} {args.Message}\r\n", entryHighlight);

                if (IsDisplayingStructure && message?.Structure?.Length > 0)
                {
                    int position = 0;
                    HMessage packet = args.Message;
                    string structure = ("{l}{u:" + packet.Header + "}");
                    foreach (string valueType in message.Structure)
                    {
                        switch (valueType.ToLower())
                        {
                            case "int":
                            structure += ("{i:" + packet.ReadInteger(ref position) + "}");
                            break;

                            case "string":
                            structure += ("{i:" + packet.ReadString(ref position) + "}");
                            break;

                            case "boolean":
                            structure += ("{i:" + packet.ReadBoolean(ref position) + "}");
                            break;
                        }
                    }
                    if (packet.GetReadableBytes(position) == 0)
                    {
                        entry.AddWriteChunk(structure + "\r\n", StructureHighlight);
                    }
                }
                entry.AddWriteChunk("--------------------\r\n", DetailHighlight);
            }
            if (entries.Count > 0 && _isReceiving)
            {
                BeginInvoke(_displayEntries, entries);
            }
        }

        public void Halt()
        {
            Close();
        }
        public void Restore()
        {
            RevisionLbl.Text = ("Revision: " + _mainVM.Game.Revision);
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Show();
        }

        public void HandleOutgoing(DataInterceptedEventArgs e) => PushToQueue(e);
        public void HandleIncoming(DataInterceptedEventArgs e) => PushToQueue(e);

        private void PushToQueue(DataInterceptedEventArgs e)
        {
            e.Continue(true);
            lock (_queueLock)
            {
                if (IsLoggingAuthorized(e))
                {
                    _intercepted.Enqueue(e);
                    if (!LogWorker.IsBusy && _intercepted.Count > 0)
                    {
                        LogWorker.RunWorkerAsync();
                    }
                }
            }
        }
        private void DisplayEntries(Queue<MessageEntry> entries)
        {
            while (entries.Count > 0 && _isReceiving)
            {
                MessageEntry entry = entries.Dequeue();
                foreach (Tuple<string, Color> chunk in entry)
                {
                    LoggerTxt.SelectionStart = LoggerTxt.TextLength;
                    LoggerTxt.SelectionLength = 0;

                    LoggerTxt.SelectionColor = chunk.Item2;
                    LoggerTxt.AppendText(chunk.Item1);
                }
            }
        }
        private bool IsLoggingAuthorized(DataInterceptedEventArgs e)
        {
            if (!_isReceiving) return false;
            if (!IsDisplayingBlocked && e.IsBlocked) return false;
            if (!IsDisplayingReplaced && e.IsOriginal) return false;

            if (!IsViewingOutgoing && e.IsOutgoing) return false;
            if (!IsViewingIncoming && !e.IsOutgoing) return false;

            int header = e.Message.Header;
            if (!e.IsOutgoing)
            {
                header = (ushort.MaxValue - header);
            }

            if (_ignoredMessages.ContainsKey(header)) return false;
            return true;
        }

        protected override void OnActivated(EventArgs e)
        {
            _isReceiving = true;
            base.OnActivated(e);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            _isReceiving = false;
            _intercepted.Clear();

            LoggerTxt.Clear();
            WindowState = FormWindowState.Minimized;

            ShowInTaskbar = false;
            base.OnFormClosing(e);
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                Invoke(handler, this, e);
            }
        }

        protected void Bind(IBindableComponent bindable, string propertyName, string dataMember)
        {
            bindable.DataBindings.Add(propertyName, this, dataMember);
        }
        protected void RaiseOnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}