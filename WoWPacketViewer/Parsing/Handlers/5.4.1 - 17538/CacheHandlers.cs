﻿using System;
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

            dbType = packet.ReadEnum<DBType, UInt32>(EnumType.FullName, "DBType");
            queryCount = packet.ReadBits(21, "QueryCount");

            var fullGuid = new ulong[queryCount];
            var guid = new byte[queryCount][];
            var id = new uint[queryCount];
            for (int i = 0; i < queryCount; i++)
                guid[i] = new byte[8];

            for (int i = 0; i < queryCount; i++)
                packet.ReadBit2DArray(ref guid[i], "guidMask", i, 1, 7, 2, 5, 0, 6, 3, 4);

            for (int i = 0; i < queryCount; i++)
            {
                packet.ReadXORBytes(ref guid[i], string.Format("guidBytes[{0}]", i) + "[{0}]", 4, 7, 6, 0, 2, 3);
                id[i] = packet.ReadUInt32("id[{0}]", i);
                packet.ReadXORBytes(ref guid[i], string.Format("guidBytes[{0}]", i) + "[{0}]", 5, 1);
            }

            for (int i = 0; i < queryCount; i++)
                packet.ReadUInt64(guid[i], "Guid[{0}]", i);

            // TODO: Handle rest of packet (for various DB types)
        }

        [Parser(Opcode.SMSG_DB_REPLY, 0x1406)]
        public static void HandleDBReply(Packet packet)
        {
            var unixTime = packet.ReadDateTime("UnixTime (last change)");
            var dbType = packet.ReadEnum<DBType, UInt32>(EnumType.FullName, "DBType");

            // TODO: Handle rest of packet (for various DB types)
        }

        [Parser(Opcode.CMSG_NAME_QUERY, 0x11E9, Direction.ClientToServer)]
        public static void HandleNameQueryRequest(Packet packet)
        {
            // TODO
        }

        [Parser(Opcode.SMSG_NAME_QUERY_RESPONSE, 0x1407)]
        public static void HandleNameQueryResponse(Packet packet)
        {
            // TODO
        }
    }
}
