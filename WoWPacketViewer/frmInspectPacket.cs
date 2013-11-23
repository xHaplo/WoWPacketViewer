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
using BrightIdeasSoftware;
using Be.Windows.Forms;

namespace WoWPacketViewer
{
    public partial class frmInspectPacket : Form
    {
        private frmMain _mainForm;
        private string _baseTitle;
        private bool _resizing = false;
        private Packet _packet;

        public frmInspectPacket(frmMain mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;
            _baseTitle = Text;

            ResizeControls();
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

            var ms = packet.BaseStream as System.IO.MemoryStream;
            hexBox.ByteProvider = new DynamicByteProvider(ms.GetBuffer());

            _packet = packet;
        }

        public void ResetForm()
        {
            Text = _baseTitle;
            lvwPacketData.ClearObjects();
            _packet = null;
        }

        private void frmInspectPacket_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't let users close this form. Just hide it instead.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;

                if (_mainForm.WindowState == FormWindowState.Minimized)
                    _mainForm.WindowState = FormWindowState.Normal;

                _mainForm.Activate();

                // Ensure the window is correctly redrawn, if this window was in front of it.
                _mainForm.Invalidate(true);
            }
        }

        private void frmInspectPacket_SizeChanged(object sender, EventArgs e)
        {
            if (!_resizing)
                ResizeColumnHeaders();

            ResizeControls();
        }

        private void ResizeControls()
        {
            // Resize form
            lvwPacketData.Height = (int)(Height * 0.6);
            lvwPacketData.Top = 0;
            lvwPacketData.Width = Width - SystemInformation.HorizontalScrollBarThumbWidth;
            lvwPacketData.Left = 0;

            hexBox.Height = Height - lvwPacketData.Height - SystemInformation.Border3DSize.Height 
                - SystemInformation.VerticalScrollBarThumbHeight - SystemInformation.VerticalScrollBarArrowHeight;
            hexBox.Top = lvwPacketData.Bottom + 1;
            hexBox.Width = Width - SystemInformation.HorizontalScrollBarThumbWidth;
            hexBox.Left = 0;
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

        private void lvwPacketData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_packet == null
                || lvwPacketData.SelectedObject == null
                || !lvwPacketData.Focused)
                return;

            var item = (PacketReadItem)lvwPacketData.SelectedObject;

            // If we encounter any items that aren't directly in the packet (but were constructed using the data)
            // we should simply unselect the existing data (otherwise it'll be confusing).
            if (item.BitOffset < 0)
            {
                hexBox.SelectionStart = 0;
                hexBox.SelectionLength = 0;
            }
            // Select the bytes in the hex box that the selected read data corresponds to
            else
            {
                hexBox.Select((int)item.ByteOffset, item.ByteLength == 0 ? 1 : item.ByteLength);
            }
        }

        private void hexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
            SelectListEntryForSelectedHexBytes();
        }

        private void hexBox_SelectionStartChanged(object sender, EventArgs e)
        {
            SelectListEntryForSelectedHexBytes();
        }

        private void SelectListEntryForSelectedHexBytes()
        {
            if (_packet == null)
                return;

            var startPos = hexBox.SelectionStart;
            var endPos = hexBox.SelectionLength;

            var item = LookupReadItemAtBytePosition(startPos, endPos);
            if (item == null)
                return;

            var listItem = lvwPacketData.ModelToItem(item);

            // TODO: Highlight list item.
        }

        private PacketReadItem LookupReadItemAtBytePosition(long bytePosition, long selectedLength)
        {
            var curBitPos = 0;
            foreach (PacketReadItem item in lvwPacketData.Objects)
            {
                var curBytePos = (curBitPos / 8);
                if (curBytePos >= bytePosition)
                    return item;

                curBitPos += item.BitLength;
            }

            return null;
        }
    }
}
