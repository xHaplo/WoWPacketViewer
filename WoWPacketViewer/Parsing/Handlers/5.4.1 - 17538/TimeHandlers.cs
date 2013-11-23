using System;
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
            uint unixTime = packet.ReadUInt32("Unix time");
            var dt = Utils.GetDateTimeFromUnixTime(unixTime);
            packet.SetLastDataField(dt.ToString());
        }

        [Parser(Opcode.SMSG_ACCOUNT_DATA_TIMES, 0x1486)]
        public static void HandleAccountDataTimes(Packet packet)
        {
            var unixTime = packet.ReadUInt32("Unix time");
            var dateTime = Utils.GetDateTimeFromUnixTime(unixTime);
            packet.SetLastDataField("{0} ({1})", dateTime, unixTime);

            var mask = packet.ReadUInt32("Mask");

            // do we still need to apply the mask?
            var accountTimes = new uint[8];
            var dateTimes = new DateTime[8];
            for (int i = 0; i < accountTimes.Length; i++)
            {
                accountTimes[i] = packet.ReadUInt32("{0}: Unix time", i);
                dateTimes[8] = Utils.GetDateTimeFromUnixTime(unixTime);
                packet.SetLastDataField("{0} ({1})", dateTimes[8], unixTime);
            }
        }

        [Parser(Opcode.CMSG_REALM_SPLIT, 0x0449, Direction.ClientToServer)]
        public static void HandleRealmSplitRequest(Packet packet)
        {
            var clientSplitState = packet.ReadEnum<ClientSplitState>(TypeCode.Int32, "ClientSplitState"); 
        }

        [Parser(Opcode.SMSG_REALM_SPLIT, 0x0884)]
        public static void HandleRealmSplitResponse(Packet packet)
        {
            packet.ReadEnum<ClientSplitState>(TypeCode.Int32, "ClientSplitState");
            packet.ReadEnum<PendingSplitState>(TypeCode.Int32, "PendingSplitState");
            uint length = packet.ReadBits(7, "Date length");
            string date = packet.ReadString((int) length, "Date");
        }
    }
}
