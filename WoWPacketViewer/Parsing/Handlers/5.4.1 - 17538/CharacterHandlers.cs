using System;
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

        class CharacterEnumResult
        {
            public uint NameLength;
            public string Name;
            public byte[] Guid;
            public byte[] GuildGuid;
            public Bit LoginCinematic;
            public byte Skin;
            public uint PetDisplayId;
            public byte Level;
            public float X;
            public float Y;
            public float Z;
            public byte Face;
            public uint ZoneId;
            public CharLoginFlags CharacterFlags;
            public uint MapId;
            public byte Race;
            public byte Gender;
            public byte HairColour;
            public byte Class;
            public CharCustomizeFlags CustomizeFlags;
            public byte FacialHair;
            public byte HairStyle;
            public uint PetFamily;
            public uint PetLevel;

            public CharacterEnumResult()
            {
                Guid = new byte[8];
                GuildGuid = new byte[8];
            }
        }

        [Parser(Opcode.SMSG_CHAR_ENUM, 0x040E)]
        public static void HandleCharacterEnumResult(Packet packet)
        {
            Bit unk0;
            uint characterCount, unk1;
            
            characterCount = packet.ReadBits(16, "Character count");

            var characters = new List<CharacterEnumResult>((int)characterCount);

            for (int i = 0; i < characterCount; i++)
            {
                var c = new CharacterEnumResult();

                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 3);
                c.LoginCinematic = packet.ReadBit("loginCinematic[{0}]", i);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 6);
                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 1);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 1, 5);
                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 6);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 7, 0);
                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 5);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 2);
                c.NameLength = packet.ReadBits(6, "nameLength[{0}]", i);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 4);
                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 4, 2);
                packet.ReadBit2DArray(ref c.Guid, "guid", i, 3);
                packet.ReadBit2DArray(ref c.GuildGuid, "guildGuid", i, 0, 7);

                characters.Add(c);
            }

            unk0 = packet.ReadBit("unk0");
            unk1 = packet.ReadBits(21, "unk1");

            if (packet.BitsRemaining() > 0)
                packet.ReadBits(packet.BitsRemaining(), "Remaining bits in flushed byte.");

            for (int i = 0; i < characterCount; i++)
            {
                var c = characters[i];
                var charPrefix = "[" + i + "] ";

                c.Skin = packet.ReadByte(charPrefix + "Skin");
                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 2, 7);
                c.PetDisplayId = packet.ReadUInt32(charPrefix + "PetDisplayId");
                c.Name = packet.ReadString((int) c.NameLength, charPrefix + "Character name");

                for (int j = 0; j < 23; j++)
                {
                    packet.ReadUInt32(charPrefix + "[{0}] Item ID", j);
                    packet.ReadUInt32(charPrefix + "[{0}] Enchantment ID", j);
                    packet.ReadByte(charPrefix + "[{0}] Inventory type", j);
                }

                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 4, 6);

                c.Level = packet.ReadByte(charPrefix + "Level");
                c.Y = packet.ReadSingle(charPrefix + "Y");
                c.X = packet.ReadSingle(charPrefix + "X");
                c.Face = packet.ReadByte(charPrefix + "Face");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 0);

                packet.ReadByte(charPrefix + "unk2");
                c.ZoneId = packet.ReadUInt32(charPrefix + "ZoneId");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 7);

                c.CharacterFlags = packet.ReadEnum<CharLoginFlags>(TypeCode.UInt32, charPrefix + "CharacterFlags");
                c.MapId = packet.ReadUInt32(charPrefix + "MapId");
                c.Race = packet.ReadByte(charPrefix + "Race");
                c.Z = packet.ReadSingle(charPrefix + "Z");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 1);

                c.Gender = packet.ReadByte(charPrefix + "Gender");

                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 3);

                c.HairColour = packet.ReadByte(charPrefix + "HairColour");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 5);

                c.Class = packet.ReadByte(charPrefix + "Class");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 2);
                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 1);

                c.CustomizeFlags = packet.ReadEnum<CharCustomizeFlags>(TypeCode.UInt32, charPrefix + "CustomizeFlags");
                c.FacialHair = packet.ReadByte(charPrefix + "FacialHair");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 6);
                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 0);

                c.HairStyle = packet.ReadByte(charPrefix + "HairStyle");

                packet.ReadXORBytes(ref c.Guid, charPrefix + "Guid", 5);

                c.PetFamily = packet.ReadUInt32(charPrefix + "PetFamily");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 2);

                c.PetLevel = packet.ReadUInt32(charPrefix + "PetLevel");

                packet.ReadXORBytes(ref c.GuildGuid, charPrefix + "GuildGuid", 4);

                ulong charGuid = BitConverter.ToUInt64(c.Guid, 0),
                    guildGuid = BitConverter.ToUInt64(c.GuildGuid, 0);

                BitArray charGuidBits = new BitArray(c.Guid),
                    guildGuidBits = new BitArray(c.GuildGuid);

                packet.AddIgnoredRead(charPrefix + "Guid (unpacked)", charGuid.ToString(), typeof(ulong), sizeof(ulong), charGuidBits, false);
                packet.AddIgnoredRead(charPrefix + "GuildGuid (unpacked)", guildGuid.ToString(), typeof(ulong), sizeof(ulong), guildGuidBits, false);
            }
        }

        [Parser(Opcode.CMSG_PLAYER_LOGIN, 0x01E1, Direction.ClientToServer)]
        public static void HandlePlayerLogin(Packet packet)
        {
            byte[] maskOrder = { 1, 0, 7, 2, 5, 6, 4, 3 };
            byte[] byteOrder = { 7, 6, 0, 1, 4, 3, 2, 5 };

            var unk0 = packet.ReadSingle("Unk0");
            ulong guid;
            packet.UnpackGuid(maskOrder, byteOrder, out guid, "Guid");
        }
    }
}