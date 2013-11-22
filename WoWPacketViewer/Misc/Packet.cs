using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CustomExtensions;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Misc
{
    public sealed partial class Packet : BinaryReader
    {
        public Packet(uint opcodeValue, byte[] buffer, Direction direction)
            : base(new MemoryStream(buffer))
        {
            OpcodeValue = opcodeValue;
            Direction = direction;
            Length = buffer.Length;
            ReadList = new List<PacketReadInfo>();
        }

        public void Reset()
        {
            BaseStream.Position = 0;
            ReadList.Clear();
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
            get;
            private set;
        }

        public List<PacketReadInfo> ReadList
        {
            get;
            set;
        }

        public void AddRead(string name, string data, Type type, int length, BitArray bits, bool lengthInBits, params object[] nameArgs)
        {
            if (!lengthInBits)
                length *= 8;

            ReadList.Add(new PacketReadInfo(string.Format(name, nameArgs), data, type, length, bits));
        }

        public void AddIgnoredRead(string name, string data, Type type, int length, BitArray bits, bool lengthInBits, params object[] nameArgs)
        {
            if (!lengthInBits)
                length *= 8;

            ReadList.Add(new PacketReadInfo(string.Format(name, nameArgs), data, type, length, bits, true));
        }

        public void SetLastDataField(string format, params object[] args)
        {
            ReadList[ReadList.Count - 1].Data = string.Format(format, args);
        }

        private byte _bitpos = 8;
        private byte _curbitval;

        public Bit ReadBit(string name, params object[] args)
        {
            var result = ReadBit();
            var bits = new BitArray(1, result);
            AddRead(name, result ? "true" : "false", typeof(Bit), 1, bits, true, args);
            return result;
        }

        public Bit ReadBit()
        {
            ++_bitpos;

            if (_bitpos > 7)
            {
                _bitpos = 0;
                _curbitval = ReadByte();
            }

            var bit = ((_curbitval >> (7 - _bitpos)) & 1) != 0;
            return bit;
        }

        public uint ReadBits(int bitCount, string name, params object[] args)
        {
            var bits = new BitArray(bitCount);
            var result = ReadBits(bitCount, ref bits);
            AddRead(name, result.ToString(), typeof(uint), bitCount, bits, true, args);
            return result;
        }

        public uint ReadBits(int bits)
        {
            uint value = 0;
            for (var i = bits - 1; i >= 0; --i)
                if (ReadBit())
                    value |= (uint)(1 << i);

            return value;
        }

        public uint ReadBits(int bits, ref BitArray bitArray)
        {
            uint value = 0;
            for (var i = bits - 1; i >= 0; --i)
            {
                if (ReadBit())
                {
                    value |= (uint)(1 << i);
                    bitArray[i] = true; // does this need to be flipped?
                }
            }

            return value;
        }

        public int BitsRemaining()
        {
            return (7 - _bitpos);
        }

        public void ResetBitReader()
        {
            _bitpos = 8;
        }

        public sbyte ReadSByte(string name, params object[] args)
        {
            var result = (sbyte)base.ReadByte();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(sbyte), sizeof(sbyte), bits, false, args);
            return result;
        }

        public byte ReadByte(string name, params object[] args)
        {
            var result = base.ReadByte();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(byte), sizeof(byte), bits, false, args);
            return result;
        }

        public ushort ReadUInt16(string name, params object[] args)
        {
            var result = base.ReadUInt16();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(ushort), sizeof(ushort), bits, false, args);
            return result;
        }

        public short ReadInt16(string name, params object[] args)
        {
            var result = base.ReadInt16();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(short), sizeof(short), bits, false, args);
            return result;
        }

        public uint ReadUInt32(string name, params object[] args)
        {
            var result = base.ReadUInt32();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(uint), sizeof(uint), bits, false, args);
            return result;
        }

        public int ReadInt32(string name, params object[] args)
        {
            var result = base.ReadInt32();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(int), sizeof(int), bits, false, args);
            return result;
        }

        public float ReadSingle(string name, params object[] args)
        {
            var result = base.ReadSingle();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(float), sizeof(float), bits, false, args);
            return result;
        }

        public ulong ReadUInt64(string name, params object[] args)
        {
            var result = base.ReadUInt64();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(ulong), sizeof(ulong), bits, false, args);
            return result;
        }

        public long ReadInt64(string name, params object[] args)
        {
            var result = base.ReadInt64();
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(long), sizeof(long), bits, false, args);
            return result;
        }

        public string ReadCString(string name, params object[] args)
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = base.ReadByte()) != 0)
                bytes.Add(b);

            var result = Encoding.UTF8.GetString(bytes.ToArray());
            var bits = new BitArray(bytes.ToArray());
            AddRead(name, result, typeof(string), bytes.Count + 1, bits, false, args);
            return result;
        }

        public string ReadString(int len, string name, params object[] args)
        {
            var bytes = base.ReadBytes(len);
            var result = Encoding.UTF8.GetString(bytes);
            var bits = new BitArray(bytes);
            AddRead(name, result, typeof(string), bytes.Length + 1, bits, false, args);
            return result;
        }

        public Guid ReadPackedGUID(out byte byteCount, out byte[] bytes)
        {
            byte mask = ReadByte();

            bytes = new byte[1+8];
            bytes[0] = mask;
            byteCount = 1;

            if (mask == 0)
                return new Guid(0);

            ulong result = 0;

            var i = 0;
            while (i < 8)
            {
                if ((mask & 1 << i) != 0)
                {
                    var b = ReadByte();
                    result += (ulong)b << (i * 8);
                    bytes[byteCount++] = b;
                }

                i++;
            }

            return new Guid(result);
        }

        public Guid ReadPackedGUID(string name, params object[] args)
        {
            byte[] bytes;
            byte byteCount;

            var guid = ReadPackedGUID(out byteCount, out bytes);
            var bits = new BitArray(bytes.SubArray(0, byteCount));
            AddRead(name, guid.Value.ToString(), typeof(Guid), byteCount, bits, false, args);

            return guid;
        }

        public byte ReadXORByte(ref byte[] bytes, byte index)
        {
            if (bytes[index] != 0)
                bytes[index] ^= base.ReadByte();

            return bytes[index];
        }

        public byte ReadXORByte(ref byte[] bytes, byte index, string name, params object[] args)
        {
            if (bytes[index] == 0)
                return 0;

            var result = ReadXORByte(ref bytes, index);
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(byte), sizeof(byte), bits, false, args);
            return result;
        }

        public void ReadXORBytes(ref byte[] bytes, params byte[] values)
        {
            foreach (var value in values)
                ReadXORByte(ref bytes, value);
        }
    }
}