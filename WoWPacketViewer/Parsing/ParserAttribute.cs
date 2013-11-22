using System;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Parsing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ParserAttribute : Attribute
    {
        public ParserAttribute(Opcode opcode, uint opcodeValue, Direction direction = Direction.ServerToClient)
        {
            Opcode = opcode;
            OpcodeValue = opcodeValue;
            Direction = direction;
        }

        public Opcode Opcode { get; private set; }
        public uint OpcodeValue { get; private set; }
        public Direction Direction { get; private set; }
    }
}
