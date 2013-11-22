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
            this.lvwPacketData = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvwPacketData
            // 
            this.lvwPacketData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwPacketData.Location = new System.Drawing.Point(0, 0);
            this.lvwPacketData.Name = "lvwPacketData";
            this.lvwPacketData.Size = new System.Drawing.Size(441, 357);
            this.lvwPacketData.TabIndex = 0;
            this.lvwPacketData.UseCompatibleStateImageBehavior = false;
            this.lvwPacketData.View = System.Windows.Forms.View.Details;
            // 
            // frmInspectPacket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 357);
            this.Controls.Add(this.lvwPacketData);
            this.Name = "frmInspectPacket";
            this.Text = "Inspect packet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmInspectPacket_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.frmInspectPacket_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwPacketData;
    }
}