using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    [ClientBuild(17538)]
    public static class TutorialHandlers
    {
        [Parser(Opcode.SMSG_TUTORIAL_FLAGS, 0x0D1B)]
        public static void HandleTutorialFlags(Packet packet)
        {
            var tutorialFlags = new uint[8];
            for (var i = 0; i < 8; i++)
                tutorialFlags[i] = packet.ReadUInt32("Flag {0}", i);
        }
    }
}
