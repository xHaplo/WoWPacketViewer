using System;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer
{
    struct PacketReadItem
    {
        public int BitOffset
        {
            get;
            set;
        }

        public float ByteOffset
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public Type Type
        {
            get;
            set;
        }

        public string TypeName
        {
            get
            {
                return Type.Name;
            }
        }

        public int BitLength
        {
            get;
            set;
        }

        public int ByteLength
        {
            get;
            set;
        }

        public string Size
        {
            get
            {
                return string.Format("{0}B ({1} bits)", ByteLength, BitLength);
            }
        }

        public string Bits
        {
            get;
            set;
        }

        public byte[] Bytes
        {
            get;
            set;
        }
    }
}
