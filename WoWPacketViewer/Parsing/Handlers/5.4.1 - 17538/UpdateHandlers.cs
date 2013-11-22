using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    [ClientBuild(17538)]
    public static class UpdateHandlers
    {
        [Parser(Opcode.SMSG_UPDATE_OBJECT, 0x0C22)]
        public static void HandleUpdateObject(Packet packet)
        {
        }
    }
}
