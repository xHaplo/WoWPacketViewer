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
    public static class CharacterHandlers
    {
        [Parser(Opcode.CMSG_CHAR_ENUM, 0x0848, Direction.ClientToServer)]
        public static void HandleCharacterEnumRequest(Packet packet)
        {
            // packet contains no data
        }

        [Parser(Opcode.SMSG_CHAR_ENUM, 0x040E)]
        public static void HandleCharacterEnumResult(Packet packet)
        {
            uint characterCount = packet.ReadBits(16, "Character count");

            for (int c = 0; c < characterCount; c++)
            {
                // TODO
            }
        }
    }
}