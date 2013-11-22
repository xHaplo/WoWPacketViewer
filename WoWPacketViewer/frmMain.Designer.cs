namespace WoWPacketViewer
{
    partial class frmMain
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPacketDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvwPacketList = new BrightIdeasSoftware.FastDataListView();
            this.olvColumnNo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDirection = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnOpcode = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwPacketList)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(487, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPacketDumpToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadPacketDumpToolStripMenuItem
            // 
            this.loadPacketDumpToolStripMenuItem.Name = "loadPacketDumpToolStripMenuItem";
            this.loadPacketDumpToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.loadPacketDumpToolStripMenuItem.Text = "&Load packet dump";
            this.loadPacketDumpToolStripMenuItem.Click += new System.EventHandler(this.loadPacketDumpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(170, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clientVersionToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // clientVersionToolStripMenuItem
            // 
            this.clientVersionToolStripMenuItem.Name = "clientVersionToolStripMenuItem";
            this.clientVersionToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.clientVersionToolStripMenuItem.Text = "Client version";
            // 
            // lvwPacketList
            // 
            this.lvwPacketList.AllColumns.Add(this.olvColumnNo);
            this.lvwPacketList.AllColumns.Add(this.olvColumnDirection);
            this.lvwPacketList.AllColumns.Add(this.olvColumnOpcode);
            this.lvwPacketList.AllColumns.Add(this.olvColumnLength);
            this.lvwPacketList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnNo,
            this.olvColumnDirection,
            this.olvColumnOpcode,
            this.olvColumnLength});
            this.lvwPacketList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwPacketList.EmptyListMsg = "No packets here - I\'m starving! :-(\n\nIf you could be so kind as to feed me, I would very much appreciate it.\n\nYou can do so via File -> Load packet dump.\n\nThanks!";
            this.lvwPacketList.FullRowSelect = true;
            this.lvwPacketList.GridLines = true;
            this.lvwPacketList.Location = new System.Drawing.Point(0, 24);
            this.lvwPacketList.Name = "lvwPacketList";
            this.lvwPacketList.ShowGroups = false;
            this.lvwPacketList.Size = new System.Drawing.Size(487, 364);
            this.lvwPacketList.TabIndex = 1;
            this.lvwPacketList.UseCompatibleStateImageBehavior = false;
            this.lvwPacketList.View = System.Windows.Forms.View.Details;
            this.lvwPacketList.VirtualMode = true;
            this.lvwPacketList.DoubleClick += new System.EventHandler(this.lvwPacketList_DoubleClick);
            // 
            // olvColumnNo
            // 
            this.olvColumnNo.AspectName = "Number";
            this.olvColumnNo.CellPadding = null;
            this.olvColumnNo.Text = "#";
            // 
            // olvColumnDirection
            // 
            this.olvColumnDirection.AspectName = "Direction";
            this.olvColumnDirection.CellPadding = null;
            this.olvColumnDirection.Text = "Direction";
            // 
            // olvColumnOpcode
            // 
            this.olvColumnOpcode.AspectName = "Opcode";
            this.olvColumnOpcode.CellPadding = null;
            this.olvColumnOpcode.Text = "Opcode";
            // 
            // olvColumnLength
            // 
            this.olvColumnLength.AspectName = "Length";
            this.olvColumnLength.CellPadding = null;
            this.olvColumnLength.Text = "Length";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 388);
            this.Controls.Add(this.lvwPacketList);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.Text = "WoW packet viewer";
            this.ResizeBegin += new System.EventHandler(this.frmMain_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvwPacketList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPacketDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clientVersionToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumnNo;
        private BrightIdeasSoftware.OLVColumn olvColumnDirection;
        private BrightIdeasSoftware.OLVColumn olvColumnOpcode;
        private BrightIdeasSoftware.OLVColumn olvColumnLength;
        private BrightIdeasSoftware.FastObjectListView lvwPacketList;
    }
}

