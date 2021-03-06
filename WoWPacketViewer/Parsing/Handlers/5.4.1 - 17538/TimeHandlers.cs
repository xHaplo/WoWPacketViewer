﻿using System;
using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    [ClientBuild(17538)]
    public static class TimeHandlers
    {
        [Parser(Opcode.CMSG_READY_FOR_ACCOUNT_DATA_TIMES, 0x144C, Direction.ClientToServer)]
        public static void HandleReadyForAccountDataTimes(Packet packet)
        {
            // packet has no data.
        }

        [Parser(Opcode.CMSG_UI_TIME_REQUEST, 0x04EC, Direction.ClientToServer)]
        public static void HandleUITimeRequest(Packet packet)
        {
            // packet has no data.
        }

        [Parser(Opcode.SMSG_UI_TIME, 0x05AC)]
        public static void HandleUITimeResponse(Packet packet)
        {
            var unixTime = packet.ReadDateTime("Unix time");
        }

        [Parser(Opcode.SMSG_ACCOUNT_DATA_TIMES, 0x1486)]
        public static void HandleAccountDataTimes(Packet packet)
        {
            var unixTime = packet.ReadDateTime("Unix time");
            var mask = packet.ReadUInt32("Mask");

            // do we still need to apply the mask?
            var accountTimes = new DateTime[8];
            for (int i = 0; i < accountTimes.Length; i++)
                accountTimes[i] = packet.ReadDateTime("{0}: Unix time", i);

            packet.ReadBit("Unk0");
            packet.ReadBits(packet.BitsRemaining(), "Bits remaining in flushed byte.");
        }

        [Parser(Opcode.CMSG_REALM_SPLIT, 0x0449, Direction.ClientToServer)]
        public static void HandleRealmSplitRequest(Packet packet)
        {
            var clientSplitState = packet.ReadEnum<ClientSplitState, UInt32>(EnumType.Name, "ClientSplitState"); 
        }

        [Parser(Opcode.SMSG_REALM_SPLIT, 0x0884)]
        public static void HandleRealmSplitResponse(Packet packet)
        {
            var clientSplitState = packet.ReadEnum<ClientSplitState, UInt32>(EnumType.Name, "ClientSplitState");
            var clientPendingState = packet.ReadEnum<PendingSplitState, UInt32>(EnumType.Name, "PendingSplitState");
            var length = packet.ReadBits(7, "Date length");
            var date = packet.ReadString((int) length, "Date");
        }
    }
}
