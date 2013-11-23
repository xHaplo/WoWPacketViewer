using System;
using System.Collections;
using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    [ClientBuild(17538)]
    public static class CacheHandlers
    {
        [Parser(Opcode.CMSG_DB_QUERY_BULK, 0x01E4, Direction.ClientToServer)]
        public static void HandleDBQueryBulk(Packet packet)
        {
            DBType dbType;
            uint queryCount;

            dbType = packet.ReadEnum<DBType>(TypeCode.UInt32, "DBType");
            queryCount = packet.ReadBits(21, "QueryCount");

            var fullGuid = new ulong[queryCount];
            var guid = new byte[queryCount][];
            var id = new uint[queryCount];
            for (int i = 0; i < queryCount; i++)
                guid[i] = new byte[8];

            for (int i = 0; i < queryCount; i++)
                packet.ReadBitArray(ref guid[i], string.Format("guidMask[{0}]", i) + "[{0}]", 1, 7, 2, 5, 0, 6, 3, 4);

            for (int i = 0; i < queryCount; i++)
            {
                packet.ReadXORBytes(ref guid[i], string.Format("guidBytes[{0}]", i) + "[{0}]", 4, 7, 6, 0, 2, 3);
                id[i] = packet.ReadUInt32("id[{0}]", i);
                packet.ReadXORBytes(ref guid[i], string.Format("guidBytes[{0}]", i) + "[{0}]", 5, 1);
            }

            for (int i = 0; i < queryCount; i++)
            {
                var bits = new BitArray(guid[i]);
                fullGuid[i] = BitConverter.ToUInt64(guid[i], 0);
                packet.AddIgnoredRead("Guid[{0}]", fullGuid[i].ToString(), typeof(ulong), sizeof(ulong), bits, false, i);
            }

            // TODO: Handle rest of packet (for various DB types)
        }

        [Parser(Opcode.SMSG_DB_REPLY, 0x1406)]
        public static void HandleDBReply(Packet packet)
        {
            uint unixTime;
            DBType dbType;

            unixTime = packet.ReadUInt32("UnixTime (last change)");
            dbType = packet.ReadEnum<DBType>(TypeCode.UInt32, "DBType");

            // TODO: Handle rest of packet (for various DB types)
        }
    }
}
