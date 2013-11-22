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
        [Parser(Opcode.SMSG_CLIENTCACHE_VERSION, 0x1037)]
        public static void HandleAuthResponse(Packet packet)
        {
            uint cacheVersion = packet.ReadUInt32("CacheVersion");
        }
    }
}