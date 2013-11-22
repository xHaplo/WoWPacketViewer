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
            System.Diagnostics.Debug.Print("{0}", packet.Length);
            // packet has no data.
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
    }
}
