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
        }

        public void LoadPacketData(Packet packet)
        {
            Text = string.Format("{0} - {1} (0x{2:X4})", 
                _baseTitle, packet.Opcode, packet.OpcodeValue);

            int bitPos = 0;
            float bytePos = 0;
            foreach (var readInfo in packet.ReadList)
            {
                var item = new PacketReadItem();
                var bits = "";
                
                foreach (bool bit in readInfo.Bits)
                    bits += (bit ? "1" : "0");

                if (!readInfo.Ignored)
                {
                    item.BitOffset = bitPos;
                    item.ByteOffset = bytePos;

                    bitPos += readInfo.LengthBits;
                    bytePos += ((float)readInfo.LengthBits / 8);
                }
                else
                {
                    item.BitOffset = -1;
                    item.ByteOffset = -1;
                }

                item.Name = readInfo.Name;
                item.Value = readInfo.Data;
                item.Type = readInfo.Type;
                item.BitLength = readInfo.LengthBits;
                item.ByteLength = readInfo.Length;
                item.Bits = bits;

                lvwPacketData.AddObject(item);
            }
        }

        public void ResetForm()
        {
            Text = _baseTitle;
            lvwPacketData.ClearObjects();
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
