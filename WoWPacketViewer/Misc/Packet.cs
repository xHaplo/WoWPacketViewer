using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using CustomExtensions;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Misc
{
    public sealed partial class Packet : BinaryReader
    {
        public Packet(uint opcodeValue, byte[] buffer, Direction direction)
            : base(new MemoryStream(buffer, 0, buffer.Length, false, true))
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
            ResetBitReader();
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

        public void ReadBitArray(ref byte[] bytes, string name, params int[] values)
        {
            foreach (var value in values)
                bytes[value] = ReadBit(name, value);
        }

        public void ReadBit2DArray(ref byte[] byteArray, string name, int arrayIndex, params int[] byteArrayPositions)
        {
            var arrayName = string.Format("{0}[{1}]", name, arrayIndex);
            ReadBitArray(ref byteArray, arrayName, byteArrayPositions);
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

        public byte[] ReadBytes(int len, string name, params object[] args)
        {
            var result = base.ReadBytes(len);
            var bits = new BitArray(result);
            AddRead(name, result.ToHexString(), typeof(byte[]), sizeof(byte) * len, bits, false, args);
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

        public byte ReadXORByte(ref byte[] bytes, int index)
        {
            if (bytes[index] != 0)
                bytes[index] ^= base.ReadByte();

            return bytes[index];
        }

        public byte ReadXORByte(ref byte[] bytes, int index, string name, params object[] args)
        {
            if (bytes[index] == 0)
                return 0;

            var result = ReadXORByte(ref bytes, index);
            var bits = new BitArray(BitConverter.GetBytes(result));
            AddRead(name, result.ToString(), typeof(byte), sizeof(byte), bits, false, args);
            return result;
        }

        public void ReadXORBytes(ref byte[] bytes, params int[] values)
        {
            foreach (var value in values)
                ReadXORByte(ref bytes, value);
        }

        public void ReadXORBytes(ref byte[] bytes, string name, params int[] values)
        {
            foreach (var value in values)
                ReadXORByte(ref bytes, value, name, value);
        }

        public byte[] UnpackGuid(byte[] maskOrder, byte[] byteOrder, out ulong guid)
        {
            var valueMask = new bool[maskOrder.Length];
            var valueBytes = new byte[byteOrder.Length];
            var byteSize = 1;
            byte maskByte = 0;

            for (var i = 0; i < valueMask.Length; i++)
            {
                valueMask[i] = ReadBit();
                if (valueMask[i])
                {
                    maskByte |= (byte)(1 << (7 - i));
                    ++byteSize;
                }
            }

            var bytes = new byte[byteSize];

            bytes[0] = maskByte;
            var n = 1;
            for (var i = 0; i < valueBytes.Length; i++)
            {
                if (valueMask[maskOrder[i]])
                {
                    bytes[n] = ReadByte();
                    valueBytes[byteOrder[i]] = (byte)(bytes[n++] ^ 1);
                }
            }

            guid = BitConverter.ToUInt64(valueBytes, 0);
            return bytes;
        }

        public byte[] UnpackGuid(byte[] maskOrder, byte[] byteOrder, out ulong guid, string name, params object[] args)
        {
            var bytes = UnpackGuid(maskOrder, byteOrder, out guid);
            var bits = new BitArray(bytes); // we want bits of the original packed data, not the unpacked data
            AddRead(name, guid.ToString(), typeof(ulong), bytes.Length, bits, false, args);
            return bytes;
        }

        private long ReadValue(TypeCode code)
        {
            long rawValue = 0;
            switch (code)
            {
                case TypeCode.SByte:
                    rawValue = ReadSByte();
                    break;
                case TypeCode.Byte:
                    rawValue = ReadByte();
                    break;
                case TypeCode.Int16:
                    rawValue = ReadInt16();
                    break;
                case TypeCode.UInt16:
                    rawValue = ReadUInt16();
                    break;
                case TypeCode.Int32:
                    rawValue = ReadInt32();
                    break;
                case TypeCode.UInt32:
                    rawValue = ReadUInt32();
                    break;
                case TypeCode.Int64:
                    rawValue = ReadInt64();
                    break;
                case TypeCode.UInt64:
                    rawValue = (long)ReadUInt64();
                    break;
            }
            return rawValue;
        }

        public T ReadEnum<T>(TypeCode code)
        {
            long rawValue = ReadValue(code);
            return (T)Enum.ToObject(typeof(T), rawValue);
        }

        public T ReadEnum<T>(TypeCode code, string name, params object[] args)
        {
            long rawValue = ReadValue(code);
            var result = (T)Enum.ToObject(typeof(T), rawValue);
            var size = 0;

            switch (code)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                    size = 1;
                    break;

                case TypeCode.UInt16:
                case TypeCode.Int16:
                    size = 2;
                    break;

                case TypeCode.UInt64:
                case TypeCode.Int64:
                    size = 8;
                    break;

                default:
                    size = 4;
                    break;
            }

            var bits = new BitArray(BitConverter.GetBytes(rawValue).SubArray(0, size));
            AddRead(name, result.GetFullName(), typeof(T), size, bits, false, args);
            return result;
        }
    }
}