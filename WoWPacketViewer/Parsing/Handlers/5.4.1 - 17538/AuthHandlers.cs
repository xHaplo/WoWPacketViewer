using System.Diagnostics;
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
    }
}