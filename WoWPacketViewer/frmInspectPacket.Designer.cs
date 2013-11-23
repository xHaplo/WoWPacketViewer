namespace WoWPacketViewer
{
    partial class frmInspectPacket
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
            this.lvwPacketData = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnBitOffset = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnByteOffset = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnBitLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnByteLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnBits = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.hexBox = new Be.Windows.Forms.HexBox();
            ((System.ComponentModel.ISupportInitialize)(this.lvwPacketData)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwPacketData
            // 
            this.lvwPacketData.AllColumns.Add(this.olvColumnBitOffset);
            this.lvwPacketData.AllColumns.Add(this.olvColumnByteOffset);
            this.lvwPacketData.AllColumns.Add(this.olvColumnName);
            this.lvwPacketData.AllColumns.Add(this.olvColumnValue);
            this.lvwPacketData.AllColumns.Add(this.olvColumnType);
            this.lvwPacketData.AllColumns.Add(this.olvColumnBitLength);
            this.lvwPacketData.AllColumns.Add(this.olvColumnByteLength);
            this.lvwPacketData.AllColumns.Add(this.olvColumnBits);
            this.lvwPacketData.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lvwPacketData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBitOffset,
            this.olvColumnByteOffset,
            this.olvColumnName,
            this.olvColumnValue,
            this.olvColumnType,
            this.olvColumnBitLength,
            this.olvColumnByteLength,
            this.olvColumnBits});
            this.lvwPacketData.EmptyListMsg = "No reads.\n\nEither an acknowledgement/request packet, or the handler has yet to be" +
    " implemented.";
            this.lvwPacketData.Location = new System.Drawing.Point(3, 3);
            this.lvwPacketData.Name = "lvwPacketData";
            this.lvwPacketData.OwnerDraw = true;
            this.lvwPacketData.ShowGroups = false;
            this.lvwPacketData.Size = new System.Drawing.Size(441, 218);
            this.lvwPacketData.TabIndex = 2;
            this.lvwPacketData.UseCompatibleStateImageBehavior = false;
            this.lvwPacketData.View = System.Windows.Forms.View.Details;
            this.lvwPacketData.VirtualMode = true;
            // 
            // olvColumnBitOffset
            // 
            this.olvColumnBitOffset.AspectName = "BitOffset";
            this.olvColumnBitOffset.CellPadding = null;
            this.olvColumnBitOffset.Text = "Bit offset";
            // 
            // olvColumnByteOffset
            // 
            this.olvColumnByteOffset.AspectName = "ByteOffset";
            this.olvColumnByteOffset.CellPadding = null;
            this.olvColumnByteOffset.Text = "Byte offset";
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "Name";
            this.olvColumnName.CellPadding = null;
            this.olvColumnName.Text = "Name";
            // 
            // olvColumnValue
            // 
            this.olvColumnValue.AspectName = "Value";
            this.olvColumnValue.CellPadding = null;
            this.olvColumnValue.Text = "Value";
            // 
            // olvColumnType
            // 
            this.olvColumnType.AspectName = "TypeName";
            this.olvColumnType.CellPadding = null;
            this.olvColumnType.Text = "Type";
            // 
            // olvColumnBitLength
            // 
            this.olvColumnBitLength.AspectName = "BitLength";
            this.olvColumnBitLength.CellPadding = null;
            this.olvColumnBitLength.Text = "Bit length";
            // 
            // olvColumnByteLength
            // 
            this.olvColumnByteLength.AspectName = "ByteLength";
            this.olvColumnByteLength.CellPadding = null;
            this.olvColumnByteLength.Text = "Byte length";
            // 
            // olvColumnBits
            // 
            this.olvColumnBits.AspectName = "Bits";
            this.olvColumnBits.CellPadding = null;
            this.olvColumnBits.Text = "Bit representation";
            // 
            // hexBox
            // 
            this.hexBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.hexBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox.InfoForeColor = System.Drawing.Color.Empty;
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(3, 227);
            this.hexBox.Name = "hexBox";
            this.hexBox.ReadOnly = true;
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(441, 133);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 4;
            this.hexBox.VScrollBarVisible = true;
            // 
            // frmInspectPacket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 357);
            this.Controls.Add(this.lvwPacketData);
            this.Controls.Add(this.hexBox);
            this.Name = "frmInspectPacket";
            this.Text = "Inspect packet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmInspectPacket_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.frmInspectPacket_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.frmInspectPacket_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.frmInspectPacket_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.lvwPacketData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView lvwPacketData;
        private BrightIdeasSoftware.OLVColumn olvColumnBitOffset;
        private BrightIdeasSoftware.OLVColumn olvColumnByteOffset;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnValue;
        private BrightIdeasSoftware.OLVColumn olvColumnType;
        private BrightIdeasSoftware.OLVColumn olvColumnBitLength;
        private BrightIdeasSoftware.OLVColumn olvColumnByteLength;
        private BrightIdeasSoftware.OLVColumn olvColumnBits;
        private Be.Windows.Forms.HexBox hexBox;


    }
}