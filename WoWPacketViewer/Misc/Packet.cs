using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Misc
{
    public sealed partial class Packet
    {
        private byte[] _buffer;

        public Packet(uint opcodeValue, byte[] buffer, Direction direction)
        {
            OpcodeValue = opcodeValue;
            _buffer = buffer;
            Direction = direction;
        }

        public Opcode Opcode
        {
            get;
            set;
        }

        public uint OpcodeValue
        {
            get;
            private set;
        }

        public Direction Direction
        {
            get;
            private set;
        }

        public int Length
        {
            get
            {
                return _buffer.Length;
            }
        }
    }
}