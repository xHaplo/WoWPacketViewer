using WoWPacketViewer.Enums;

namespace WoWPacketViewer
{
    class PacketListItem
    {
        public int Number
        {
            get;
            set;
        }

        public string Direction
        {
            get;
            set;
        }

        public string Opcode
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }
    }
}
