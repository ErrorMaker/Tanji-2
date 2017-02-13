using Tanji.Properties;

namespace Tanji.Windows.Logger
{
    partial class PacketLogger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoggerMenu = new System.Windows.Forms.MenuStrip();
            this.FileBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.FindBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.FindMessageBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.IgnoreMessageBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.FileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.EmptyLogBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ViewBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.DisplayFiltersBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.BlockedBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ReplacedBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.DisplayDetailsBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.HashBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.StructureBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.TimestampBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ParserClassNameBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.MessageClassNameBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ViewSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.ViewOutgoingBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ViewIncomingBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.ViewSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.AlwaysOnTopBtn = new Tanji.Controls.BindableToolStripMenuItem();
            this.LoggerTxt = new System.Windows.Forms.RichTextBox();
            this.LoggerStrip = new System.Windows.Forms.StatusStrip();
            this.RevisionLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.LogWorker = new System.ComponentModel.BackgroundWorker();
            this.LoggerMenu.SuspendLayout();
            this.LoggerStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoggerMenu
            // 
            this.LoggerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileBtn,
            this.ViewBtn});
            this.LoggerMenu.Location = new System.Drawing.Point(0, 0);
            this.LoggerMenu.Name = "LoggerMenu";
            this.LoggerMenu.Size = new System.Drawing.Size(710, 24);
            this.LoggerMenu.TabIndex = 0;
            // 
            // FileBtn
            // 
            this.FileBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindBtn,
            this.FindMessageBtn,
            this.IgnoreMessageBtn,
            this.FileSep1,
            this.EmptyLogBtn});
            this.FileBtn.Name = "FileBtn";
            this.FileBtn.Size = new System.Drawing.Size(37, 20);
            this.FileBtn.Text = "File";
            // 
            // FindBtn
            // 
            this.FindBtn.Enabled = false;
            this.FindBtn.Name = "FindBtn";
            this.FindBtn.Size = new System.Drawing.Size(157, 22);
            this.FindBtn.Text = "Find";
            // 
            // FindMessageBtn
            // 
            this.FindMessageBtn.Enabled = false;
            this.FindMessageBtn.Name = "FindMessageBtn";
            this.FindMessageBtn.Size = new System.Drawing.Size(157, 22);
            this.FindMessageBtn.Text = "Find Message";
            // 
            // IgnoreMessageBtn
            // 
            this.IgnoreMessageBtn.Enabled = false;
            this.IgnoreMessageBtn.Name = "IgnoreMessageBtn";
            this.IgnoreMessageBtn.Size = new System.Drawing.Size(157, 22);
            this.IgnoreMessageBtn.Text = "Ignore Message";
            // 
            // FileSep1
            // 
            this.FileSep1.Name = "FileSep1";
            this.FileSep1.Size = new System.Drawing.Size(154, 6);
            // 
            // EmptyLogBtn
            // 
            this.EmptyLogBtn.Name = "EmptyLogBtn";
            this.EmptyLogBtn.Size = new System.Drawing.Size(157, 22);
            this.EmptyLogBtn.Text = "Empty Log";
            this.EmptyLogBtn.Click += new System.EventHandler(this.EmptyLogBtn_Click);
            // 
            // ViewBtn
            // 
            this.ViewBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DisplayFiltersBtn,
            this.DisplayDetailsBtn,
            this.ViewSep1,
            this.ViewOutgoingBtn,
            this.ViewIncomingBtn,
            this.ViewSep2,
            this.AlwaysOnTopBtn});
            this.ViewBtn.Name = "ViewBtn";
            this.ViewBtn.Size = new System.Drawing.Size(44, 20);
            this.ViewBtn.Text = "View";
            // 
            // DisplayFiltersBtn
            // 
            this.DisplayFiltersBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BlockedBtn,
            this.ReplacedBtn});
            this.DisplayFiltersBtn.Name = "DisplayFiltersBtn";
            this.DisplayFiltersBtn.Size = new System.Drawing.Size(153, 22);
            this.DisplayFiltersBtn.Text = "Display Filters";
            // 
            // BlockedBtn
            // 
            this.BlockedBtn.Checked = true;
            this.BlockedBtn.CheckOnClick = true;
            this.BlockedBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BlockedBtn.Name = "BlockedBtn";
            this.BlockedBtn.Size = new System.Drawing.Size(152, 22);
            this.BlockedBtn.Text = "Blocked";
            // 
            // ReplacedBtn
            // 
            this.ReplacedBtn.Checked = true;
            this.ReplacedBtn.CheckOnClick = true;
            this.ReplacedBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplacedBtn.Name = "ReplacedBtn";
            this.ReplacedBtn.Size = new System.Drawing.Size(152, 22);
            this.ReplacedBtn.Text = "Replaced";
            // 
            // DisplayDetailsBtn
            // 
            this.DisplayDetailsBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HashBtn,
            this.StructureBtn,
            this.TimestampBtn,
            this.ParserClassNameBtn,
            this.MessageClassNameBtn});
            this.DisplayDetailsBtn.Name = "DisplayDetailsBtn";
            this.DisplayDetailsBtn.Size = new System.Drawing.Size(153, 22);
            this.DisplayDetailsBtn.Text = "Display Details";
            // 
            // HashBtn
            // 
            this.HashBtn.CheckOnClick = true;
            this.HashBtn.Name = "HashBtn";
            this.HashBtn.Size = new System.Drawing.Size(185, 22);
            this.HashBtn.Text = "Hash";
            // 
            // StructureBtn
            // 
            this.StructureBtn.Checked = true;
            this.StructureBtn.CheckOnClick = true;
            this.StructureBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StructureBtn.Name = "StructureBtn";
            this.StructureBtn.Size = new System.Drawing.Size(185, 22);
            this.StructureBtn.Text = "Structure";
            // 
            // TimestampBtn
            // 
            this.TimestampBtn.CheckOnClick = true;
            this.TimestampBtn.Name = "TimestampBtn";
            this.TimestampBtn.Size = new System.Drawing.Size(185, 22);
            this.TimestampBtn.Text = "Timestamp";
            // 
            // ParserClassNameBtn
            // 
            this.ParserClassNameBtn.Checked = true;
            this.ParserClassNameBtn.CheckOnClick = true;
            this.ParserClassNameBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ParserClassNameBtn.Name = "ParserClassNameBtn";
            this.ParserClassNameBtn.Size = new System.Drawing.Size(185, 22);
            this.ParserClassNameBtn.Text = "Parser Class Name";
            // 
            // MessageClassNameBtn
            // 
            this.MessageClassNameBtn.Checked = true;
            this.MessageClassNameBtn.CheckOnClick = true;
            this.MessageClassNameBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MessageClassNameBtn.Name = "MessageClassNameBtn";
            this.MessageClassNameBtn.Size = new System.Drawing.Size(185, 22);
            this.MessageClassNameBtn.Text = "Message Class Name";
            // 
            // ViewSep1
            // 
            this.ViewSep1.Name = "ViewSep1";
            this.ViewSep1.Size = new System.Drawing.Size(150, 6);
            // 
            // ViewOutgoingBtn
            // 
            this.ViewOutgoingBtn.Checked = true;
            this.ViewOutgoingBtn.CheckOnClick = true;
            this.ViewOutgoingBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewOutgoingBtn.Name = "ViewOutgoingBtn";
            this.ViewOutgoingBtn.Size = new System.Drawing.Size(153, 22);
            this.ViewOutgoingBtn.Text = "View Outgoing";
            // 
            // ViewIncomingBtn
            // 
            this.ViewIncomingBtn.Checked = true;
            this.ViewIncomingBtn.CheckOnClick = true;
            this.ViewIncomingBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewIncomingBtn.Name = "ViewIncomingBtn";
            this.ViewIncomingBtn.Size = new System.Drawing.Size(153, 22);
            this.ViewIncomingBtn.Text = "View Incoming";
            // 
            // ViewSep2
            // 
            this.ViewSep2.Name = "ViewSep2";
            this.ViewSep2.Size = new System.Drawing.Size(150, 6);
            // 
            // AlwaysOnTopBtn
            // 
            this.AlwaysOnTopBtn.CheckOnClick = true;
            this.AlwaysOnTopBtn.Name = "AlwaysOnTopBtn";
            this.AlwaysOnTopBtn.Size = new System.Drawing.Size(153, 22);
            this.AlwaysOnTopBtn.Text = "Always On Top";
            // 
            // LoggerTxt
            // 
            this.LoggerTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.LoggerTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LoggerTxt.DetectUrls = false;
            this.LoggerTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoggerTxt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.LoggerTxt.ForeColor = System.Drawing.Color.White;
            this.LoggerTxt.HideSelection = false;
            this.LoggerTxt.Location = new System.Drawing.Point(0, 24);
            this.LoggerTxt.Name = "LoggerTxt";
            this.LoggerTxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LoggerTxt.ShowSelectionMargin = true;
            this.LoggerTxt.Size = new System.Drawing.Size(710, 475);
            this.LoggerTxt.TabIndex = 1;
            this.LoggerTxt.Text = "";
            // 
            // LoggerStrip
            // 
            this.LoggerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RevisionLbl});
            this.LoggerStrip.Location = new System.Drawing.Point(0, 499);
            this.LoggerStrip.Name = "LoggerStrip";
            this.LoggerStrip.Size = new System.Drawing.Size(710, 22);
            this.LoggerStrip.TabIndex = 2;
            this.LoggerStrip.Text = "statusStrip1";
            // 
            // RevisionLbl
            // 
            this.RevisionLbl.Name = "RevisionLbl";
            this.RevisionLbl.Size = new System.Drawing.Size(268, 17);
            this.RevisionLbl.Text = "Revision: PRODUCTION-000000000000-000000000";
            // 
            // LogWorker
            // 
            this.LogWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LogWorker_DoWork);
            // 
            // PacketLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(710, 521);
            this.Controls.Add(this.LoggerTxt);
            this.Controls.Add(this.LoggerStrip);
            this.Controls.Add(this.LoggerMenu);
            this.Icon = global::Tanji.Properties.Resources.Tanji_256;
            this.MainMenuStrip = this.LoggerMenu;
            this.Name = "PacketLogger";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tanji - Packet Logger";
            this.LoggerMenu.ResumeLayout(false);
            this.LoggerMenu.PerformLayout();
            this.LoggerStrip.ResumeLayout(false);
            this.LoggerStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip LoggerMenu;
        private Controls.BindableToolStripMenuItem FileBtn;
        private Controls.BindableToolStripMenuItem FindBtn;
        private Controls.BindableToolStripMenuItem FindMessageBtn;
        private Controls.BindableToolStripMenuItem IgnoreMessageBtn;
        private System.Windows.Forms.ToolStripSeparator FileSep1;
        private Controls.BindableToolStripMenuItem EmptyLogBtn;
        private Controls.BindableToolStripMenuItem ViewBtn;
        private Controls.BindableToolStripMenuItem DisplayFiltersBtn;
        private Controls.BindableToolStripMenuItem BlockedBtn;
        private Controls.BindableToolStripMenuItem ReplacedBtn;
        private Controls.BindableToolStripMenuItem DisplayDetailsBtn;
        private System.Windows.Forms.ToolStripSeparator ViewSep1;
        private Controls.BindableToolStripMenuItem ViewOutgoingBtn;
        private Controls.BindableToolStripMenuItem ViewIncomingBtn;
        private System.Windows.Forms.ToolStripSeparator ViewSep2;
        private Controls.BindableToolStripMenuItem AlwaysOnTopBtn;
        private System.Windows.Forms.RichTextBox LoggerTxt;
        private System.Windows.Forms.StatusStrip LoggerStrip;
        internal System.Windows.Forms.ToolStripStatusLabel RevisionLbl;
        private Controls.BindableToolStripMenuItem HashBtn;
        private Controls.BindableToolStripMenuItem TimestampBtn;
        private Controls.BindableToolStripMenuItem ParserClassNameBtn;
        private Controls.BindableToolStripMenuItem MessageClassNameBtn;
        private Controls.BindableToolStripMenuItem StructureBtn;
        private System.ComponentModel.BackgroundWorker LogWorker;
    }
}