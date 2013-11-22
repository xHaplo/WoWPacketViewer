using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using CustomExtensions;
using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    using Guid = WoWPacketViewer.Misc.Guid;

    [ClientBuild(17538)]
    public static class AuthHandlers
    {
        [Parser(Opcode.SMSG_AUTH_CHALLENGE, 0x0C5D)]
        public static void HandleAuthChallenge(Packet packet)
        {
            byte response = packet.ReadByte("Response");
            var key = new uint[8];
            uint seed;

            for (int i = 0; i < key.Length; i++)
                key[i] = packet.ReadUInt32("Key {0}", i + 1);

            seed = packet.ReadUInt32("Seed");
        }

        [Parser(Opcode.CMSG_AUTH_SESSION, 0x14DA, Direction.ClientToServer)]
        public static void HandleAuthSession(Packet packet)
        {
            uint unk0, unk2, unk3, unk4, unk7, addonSize, accountNameLength;
            byte[] digest = new byte[20], addonData;
            byte unk1, unk6;
            ulong unk5;
            ushort clientBuild;
            string accountName;

            // NOTE: Digest order is most likely changed.

            unk0 = packet.ReadUInt32("Unk0");

            digest[4] = packet.ReadByte("digest[4]");
            unk1 = packet.ReadByte("Unk1");

            unk2 = packet.ReadUInt32("Unk2");

            digest[19] = packet.ReadByte("digest[19]");
            digest[12] = packet.ReadByte("digest[12]");
            digest[9] = packet.ReadByte("digest[9]");
            digest[6] = packet.ReadByte("digest[6]");
            digest[18] = packet.ReadByte("digest[18]");
            digest[17] = packet.ReadByte("digest[17]");
            digest[8] = packet.ReadByte("digest[8]");
            digest[13] = packet.ReadByte("digest[13]");

            unk3 = packet.ReadUInt32("Unk3");

            digest[1] = packet.ReadByte("digest[1]");
            digest[10] = packet.ReadByte("digest[10]");

            clientBuild = packet.ReadUInt16("clientBuild");

            digest[11] = packet.ReadByte("digest[11]");

            unk4 = packet.ReadUInt32("Unk4");

            digest[15] = packet.ReadByte("digest[15]");
            digest[3] = packet.ReadByte("digest[3]");
            digest[14] = packet.ReadByte("digest[14]");

            unk5 = packet.ReadUInt64("Unk5");

            digest[7] = packet.ReadByte("digest[7]");
            digest[5] = packet.ReadByte("digest[5]");
            digest[0] = packet.ReadByte("digest[0]");
            digest[16] = packet.ReadByte("digest[16]");
            digest[2] = packet.ReadByte("digest[2]");

            unk6 = packet.ReadByte("Unk6");
            unk7 = packet.ReadUInt32("Unk6");

            addonSize = packet.ReadUInt32("addonSize");
            addonData = packet.ReadBytes((int)addonSize, "addonData");

            accountNameLength = packet.ReadBits(11, "accountNameLength");
            accountName = packet.ReadString((int) accountNameLength, "accountName");
        }

        [Parser(Opcode.SMSG_AUTH_RESPONSE, 0x0D05)]
        public static void HandleAuthResponse(Packet packet)
        {
            var authCode = (AuthCodes) packet.ReadByte("authCode");
            packet.SetLastDataField(authCode.GetFullName());

            if (authCode != AuthCodes.AUTH_OK)
                return;

            Bit isInQueue, hasQueuePosition, hasAccountData,
                unk0, unk3, unk4, unk5;
            uint unk1, unk2, realmRaceCount = 0, realmClassCount = 0,
                unk6, unk7, unk8, unk9, unk10, queuePosition;
            byte accountExpansion1, accountExpansion2;

            var raceMap = new Dictionary<byte, byte>();
            var classMap = new Dictionary<byte, byte>();

            isInQueue = packet.ReadBit("IsInQueue");
            if (isInQueue)
                hasQueuePosition = packet.ReadBit("HasQueuePosition"); // seems that way, not 100% sure

            hasAccountData = packet.ReadBit("HasAccountData");
            if (hasAccountData)
            {
                unk0 = packet.ReadBit("Unk0");
                unk1 = packet.ReadBits(21, "Unk1");
                unk2 = packet.ReadBits(21, "Unk2");
                realmRaceCount = packet.ReadBits(23, "RealmRaceCount");
                unk3 = packet.ReadBit("Unk3");
                unk4 = packet.ReadBit("Unk4");
                unk5 = packet.ReadBit("Unk5");
                realmClassCount = packet.ReadBits(23, "RealmClassCount");
            }

            if (packet.BitsRemaining() > 0)
                packet.ReadBits(packet.BitsRemaining());

            if (hasAccountData)
            {
                unk6 = packet.ReadUInt32("Unk6");
                unk7 = packet.ReadUInt32("Unk7");
                accountExpansion1 = packet.ReadByte("AccountExpansion1");

                for (int r = 0; r < realmRaceCount; r++)
                {
                    byte expansion = packet.ReadByte("r{0}: expansion", r);
                    byte race = packet.ReadByte("r{0}: race", r);
                }

                accountExpansion2 = packet.ReadByte("AccountExpansion2");
                unk8 = packet.ReadUInt32("Unk8");

                for (int c = 0; c < realmClassCount; c++)
                {
                    byte class_ = packet.ReadByte("c{0}: class", c);
                    byte expansion = packet.ReadByte("c{0}: expansion", c);
                }

                unk9 = packet.ReadUInt32("Unk9");
                unk10 = packet.ReadUInt32("Unk10");
            }

            if (isInQueue)
                queuePosition = packet.ReadUInt32("Queue position");
        }
    }
}