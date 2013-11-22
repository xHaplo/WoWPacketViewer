using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomExtensions;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer
{
    public partial class frmInspectPacket : Form
    {
        private frmMain _mainForm;
        private string _baseTitle;
        private bool _resizing = false;

        public frmInspectPacket(frmMain mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;
            _baseTitle = Text;

            // Enable double buffering on the data list to prevent flickering.
            lvwPacketData.DoubleBuffer();

            // Setup column headers for packet list.
            var columnHeaders = new ColumnHeader[8];
            for (var i = 0; i < columnHeaders.Length; i++)
                columnHeaders[i] = new ColumnHeader();

            columnHeaders[0].Text = "Bit offset";
            columnHeaders[1].Text = "Byte offset";
            columnHeaders[2].Text = "Name";
            columnHeaders[3].Text = "Value";
            columnHeaders[4].Text = "Type";
            columnHeaders[5].Text = "Bit length";
            columnHeaders[6].Text = "Byte length";
            columnHeaders[7].Text = "Bits";

            lvwPacketData.Columns.AddRange(columnHeaders);
        }

        public void LoadPacketData(Packet packet)
        {
            Text = string.Format("{0} - {1} (0x{2:X4})", 
                _baseTitle, packet.Opcode, packet.OpcodeValue);

            int bitPos = 0;
            float bytePos = 0;
            foreach (var readInfo in packet.ReadList)
            {
                var item = new ListViewItem();
                var bits = "";
                
                foreach (bool bit in readInfo.Bits)
                    bits += (bit ? "1" : "0");

                if (!readInfo.Ignored)
                {
                    item.Text = bitPos.ToString();
                    item.SubItems.Add(bytePos.ToString());
                    bitPos += readInfo.LengthBits;
                    bytePos += ((float)readInfo.LengthBits / 8);
                }
                else
                {
                    item.Text = "n/a";
                    item.SubItems.Add("n/a");
                }

                item.SubItems.Add(readInfo.Name);
                item.SubItems.Add(readInfo.Data);
                item.SubItems.Add(readInfo.Type.Name);
                item.SubItems.Add(readInfo.LengthBits.ToString());
                item.SubItems.Add(readInfo.Length.ToString());
                item.SubItems.Add(bits);

                lvwPacketData.Items.Add(item);
            }
        }

        public void ResetForm()
        {
            Text = _baseTitle;
            lvwPacketData.Items.Clear();
        }

        private void frmInspectPacket_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't let users close this form. Just hide it instead.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void frmInspectPacket_SizeChanged(object sender, EventArgs e)
        {
            if (!_resizing)
                ResizeColumnHeaders();
        }

        private void frmInspectPacket_ResizeBegin(object sender, EventArgs e)
        {
            _resizing = true;
        }

        private void frmInspectPacket_ResizeEnd(object sender, EventArgs e)
        {
            ResizeColumnHeaders();
            _resizing = false;
        }

        private void ResizeColumnHeaders()
        {
            for (int i = 0; i < lvwPacketData.Columns.Count - 1; i++)
                lvwPacketData.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);

            lvwPacketData.Columns[lvwPacketData.Columns.Count - 1].Width = -2;
        }
    }
}
