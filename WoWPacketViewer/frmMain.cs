using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CustomExtensions;
using WoWPacketViewer.Misc;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer
{
    public partial class frmMain : Form
    {
        private uint _clientBuild;
        private ToolStripMenuItem _checkedClientVersionMenu;
        private List<Packet> _packetList;
        private string _baseTitle;
        private bool _resizing = false;

        private frmInspectPacket _inspectPacketForm;

        public frmMain()
        {
            InitializeComponent();

            // Set the base title
            _baseTitle = Text;

            // Load packet handlers, and obtain all known client builds.
            Handler.LoadHandlers();

            // Add all known versions to the menu & set the newest as default.
            AddAvailableClientVersionsToMenu();

            // Enable double buffering on the packet list to prevent flickering.
            lvwPacketList.DoubleBuffer();

            // Setup column headers for packet list.
            var columnHeaders = new ColumnHeader[4];
            for (var i = 0; i < columnHeaders.Length; i++)
                columnHeaders[i] = new ColumnHeader();

            columnHeaders[0].Text = "#";
            columnHeaders[1].Text = "Direction";
            columnHeaders[2].Text = "Opcode";
            columnHeaders[3].Text = "Length";

            lvwPacketList.Columns.AddRange(columnHeaders);

            // Setup the packet inspection form
            _inspectPacketForm = new frmInspectPacket(this);
        }

        private void AddAvailableClientVersionsToMenu()
        {
            // Add all known client builds to the menu, so the user can select which to use.
            foreach (var kvp in ClientVersion.AvailableVersions)
            {
                var menuLabel = string.Format("{0} ({1})", kvp.Value, kvp.Key);
                var menuItem = new ToolStripMenuItem(menuLabel);

                // Tag the menu item with its corresponding client build, for reference purposes.
                menuItem.Tag = kvp.Key;

                // Ensure this item can be checked, and the build is set when doing so.
                menuItem.Click += clientBuildMenuItem_OnClick;
                clientVersionToolStripMenuItem.DropDownItems.Add(menuItem);

                // Set the client build. Last set is most recently, and will be preferred.
                _clientBuild = kvp.Key;
                _checkedClientVersionMenu = menuItem;
            }

            // Remember to check the last menu item in the list
            _checkedClientVersionMenu.Checked = true;
        }

        private void clientBuildMenuItem_OnClick(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            // Unchecking, don't worry. We're only worried about when things are set.
            if (item == _checkedClientVersionMenu)
                return;

            _clientBuild = (uint) item.Tag;

            // Find & uncheck the existing checked item.
            _checkedClientVersionMenu.CheckState = CheckState.Unchecked;

            // Change the checked menu.
            _checkedClientVersionMenu = item;
            _checkedClientVersionMenu.CheckState = CheckState.Checked;
        }

        private void loadPacketDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            ofd.Title = "Load packet dump";
            if (ofd.ShowDialog() == DialogResult.OK)
                LoadPacketDumpFromFile(ofd.FileName);
        }

        private void DisplayError(string text, string caption = "Error")
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoadPacketDumpFromFile(string filename)
        {
            var packetList = new List<Packet>();
            if (!PacketDump.Load(filename, ref packetList))
            {
                DisplayError("Failed to load packet dump. Please verify that the file exists, and is not currently in use.");
                return;
            }

            Text = string.Format("{0} - {1}", _baseTitle, filename);
            LoadPacketDump(packetList);
        }

        private void LoadPacketDump(List<Packet> packetList)
        {
            // Clear out the existing list.
            lvwPacketList.Items.Clear();

            // Store the packet list for later use.
            _packetList = packetList;

            // Add all the packets to the main list
            for (var i = 0; i < packetList.Count; i++)
            {
                var packet = packetList[i];
                var item = new ListViewItem();

                // Attach opcode
                packet.Opcode = Handler.LookupOpcode(_clientBuild, packet.OpcodeValue, packet.Direction);

                item.Text = (i + 1).ToString();
                item.SubItems.Add(packet.Direction == Direction.ServerToClient ? "S -> C" : "C -> S");
                item.SubItems.Add(string.Format("{0} (0x{1:X4})", packet.Opcode, packet.OpcodeValue));
                item.SubItems.Add(packet.Length.ToString());
                lvwPacketList.Items.Add(item);
            }

            // Resize the column headers to fit the content width
            ResizeColumnHeaders();
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (!_resizing)
                ResizeColumnHeaders();
        }

        private void frmMain_ResizeBegin(object sender, EventArgs e)
        {
            _resizing = true;
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            ResizeColumnHeaders();
            _resizing = false;
        }

        private void ResizeColumnHeaders()
        {
            for (int i = 0; i < lvwPacketList.Columns.Count - 1; i++)
                lvwPacketList.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);

            lvwPacketList.Columns[lvwPacketList.Columns.Count - 1].Width = -2;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lvwPacketList_DoubleClick(object sender, EventArgs e)
        {
            LoadInfoForSelectedPacket();
        }

        private void LoadInfoForSelectedPacket()
        {
            if (lvwPacketList.SelectedItems == null
                || lvwPacketList.SelectedItems.Count == 0)
                return;

            var selectedItem = lvwPacketList.SelectedItems[0];
            var packet = _packetList[selectedItem.Index];

            // Reset the packet inspection form first, so as not to confuse users with old data.
            _inspectPacketForm.ResetForm();

            // Load packet data in form
            if (!Handler.HandlePacket(_clientBuild, packet))
            {
                DisplayError("There is no registered handler for this packet. I do not know how to handle it.");
                return;
            }

            _inspectPacketForm.LoadPacketData(packet);

            // Show the form, or re-activate it if it's minimised.
            _inspectPacketForm.Show();
            _inspectPacketForm.WindowState = FormWindowState.Normal;
            _inspectPacketForm.Activate();
        }
    }
}
