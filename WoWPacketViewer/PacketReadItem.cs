using System;
using System.Collections;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer
{
    public class PacketReadItem
    {
        public PacketReadItem(string name, string value, Type type, int lengthBits, BitArray bits, int bitOffset, bool rawDataInPacket = true)
        {
            Name = name;
            Value = value;
            Type = type;
            Bits = bits;
            BitLength = lengthBits;
            BitOffset = bitOffset;
            ByteLength = lengthBits / 8;
            ByteOffset = (float)bitOffset / 8;
            RawDataInPacket = rawDataInPacket;

            BitString = "";
            foreach (bool bit in Bits)
                BitString += (bit ? "1" : "0");
        }

        // Raw data is directly from packet.
        // Set this to false when the data is
        // simply constructed from pieces of data in a packet.
        public bool RawDataInPacket
        {
            get;
            set;
        }

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
                return string.Format("{0} ({1} bits)", ByteLength, BitLength);
            }
        }

        public BitArray Bits
        {
            get;
            set;
        }

        public string BitString
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
