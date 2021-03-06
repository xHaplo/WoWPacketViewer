﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CustomExtensions;
using BrightIdeasSoftware;
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
        private string _logPath;
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

        private void LoadPacketDumpFromFile(string logPath)
        {
            // No file loaded, prompt for one.
            if (String.IsNullOrEmpty(logPath))
            {
                loadPacketDumpToolStripMenuItem_Click(null, null);
                return;
            }

            var packetList = new List<Packet>();
            if (!PacketDump.Load(logPath, ref packetList))
            {
                DisplayError("Failed to load packet dump. Please verify that the file exists, and is not currently in use.");
                return;
            }

            _logPath = logPath;

            Text = string.Format("{0} - {1}", _baseTitle, logPath);
            LoadPacketDump(packetList);
        }

        private void LoadPacketDump(List<Packet> packetList)
        {
            // Clear out the existing list.
            lvwPacketList.ClearObjects();

            // Store the packet list for later use.
            _packetList = packetList;

            // Add all the packets to the main list
            for (var i = 0; i < packetList.Count; i++)
            {
                var packet = packetList[i];
                var item = new PacketListItem();

                // Attach opcode
                packet.Opcode = Handler.LookupOpcode(_clientBuild, packet.OpcodeValue, packet.Direction);

                // Arctium includes 2 bytes of the header for initial non-"world" packets.
                // For these packets to be parsed correctly, we must chop this part of the header off first.
                // Their client packets also leaves lengths intact, so we get 2 extra bytes on the end. This must be removed too.
                if (Handler.LogType == PacketLogType.Arctium)
                {
                    switch (packet.Opcode)
                    {
                        case Opcode.MSG_VERIFY_CONNECTIVITY:
                        case Opcode.CMSG_AUTH_SESSION:
                        case Opcode.SMSG_AUTH_CHALLENGE:
                        {
                            var ms = packet.BaseStream as MemoryStream;
                            var bytesToRemove = (packet.Direction == Direction.ClientToServer ? 4 : 2);
                            var newPacket = new Packet(packet.OpcodeValue, ms.GetBuffer().SubArray(2, packet.Length - bytesToRemove), packet.Direction);
                            newPacket.Opcode = packet.Opcode;
                            packetList[i] = packet = newPacket;
                        } break;
                    }
                }

                item.Number = i + 1;
                item.Direction = (packet.Direction == Direction.ServerToClient ? "S -> C" : "C -> S");
                item.Opcode = string.Format("{0} (0x{1:X4})", packet.Opcode, packet.OpcodeValue);
                item.Length = packet.Length;

                lvwPacketList.AddObject(item);
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
            if (lvwPacketList.SelectedObject == null)
                return;

            var selectedItem = (PacketListItem)lvwPacketList.SelectedObject;
            if (selectedItem.Number > _packetList.Count)
                return;

            var packet = _packetList[selectedItem.Number - 1];

            // Reset the packet inspection form first, so as not to confuse users with old data.
            _inspectPacketForm.ResetForm();

            // Load packet data
            Handler.HandlePacket(_clientBuild, packet);

            // Load packet data into the form.
            _inspectPacketForm.LoadPacketData(packet, _logPath);

            // Show the form, or re-activate it if it's minimised.
            _inspectPacketForm.Show();
            if (_inspectPacketForm.WindowState == FormWindowState.Minimized)
                _inspectPacketForm.WindowState = FormWindowState.Normal;
            _inspectPacketForm.Activate();
        }

        private void lvwPacketList_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Load packet inspection form when enter is pressed on an item.
            if (e.KeyChar == 13)
            {
                LoadInfoForSelectedPacket();
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                // Refresh packet dump
                case Keys.F5:
                    reloadDumpToolStripMenuItem_Click(null, null);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void reloadDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPacketDumpFromFile(_logPath);
        }
    }
}
