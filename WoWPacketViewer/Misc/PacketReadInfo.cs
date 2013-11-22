using System;
using System.Collections;

namespace WoWPacketViewer.Misc
{
    public class PacketReadInfo
    {
        public string Name;
        public string Data;
        public Type Type;
        public int LengthBits;
        public int Length;
        public BitArray Bits;

        // To ignore data that isnt part of the packet, but created as a result of parts of it.
        // e.g. Some guid construction.
        public bool Ignored; 

        public PacketReadInfo(string name, string data, Type type, int lengthBits, BitArray bits, bool ignored = false)
        {
            Name = name;
            Data = data;
            Type = type;
            LengthBits = lengthBits;
            Length = lengthBits / 8;
            Bits = bits;
            Ignored = ignored;
        }
    }
}
