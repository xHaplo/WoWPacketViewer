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
    public static class MiscHandlers
    {
        [Parser(Opcode.MSG_VERIFY_CONNECTIVITY, 0x4F57, Direction.ServerToClient)]
        public static void HandleClientVerifyConnectivity(Packet packet)
        {
            var message = packet.ReadCString("Message");
        }

        [Parser(Opcode.MSG_VERIFY_CONNECTIVITY, 0x4F57, Direction.ClientToServer)]
        public static void HandleServerVerifyConnectivity(Packet packet)
        {
            var message = packet.ReadCString("Message");
        }

        [Parser(Opcode.SMSG_CLIENTCACHE_VERSION, 0x1037)]
        public static void HandleAuthResponse(Packet packet)
        {
            uint cacheVersion = packet.ReadUInt32("CacheVersion");
        }

        [Parser(Opcode.CMSG_LOAD_SCREEN, 0x1148, Direction.ClientToServer)]
        public static void HandleLoadingScreenOpcode(Packet packet)
        {
            uint mapId = packet.ReadUInt32("Map ID");
            bool loadingScreenEnabled = packet.ReadBit("loadingScreenEnabled");
        }

        [Parser(Opcode.CMSG_VIOLENCE_LEVEL, 0x13CD, Direction.ClientToServer)]
        public static void HandleViolenceLevelOpcode(Packet packet)
        {
            byte violenceLevel = packet.ReadByte("Violence level");
        }

        [Parser(Opcode.CMSG_SET_ACTIVE_MOVER, 0x1A4D, Direction.ClientToServer)]
        public static void HandleActivePlayer(Packet packet)
        {
            byte active = packet.ReadByte("Active");
        }
    }
}