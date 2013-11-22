using System;
using System.Collections;
using System.Diagnostics;
using CustomExtensions;
using WoWPacketViewer.Enums;
using WoWPacketViewer.Misc;

namespace WoWPacketViewer.Parsing.Handlers.V541_17538
{
    using Guid = WoWPacketViewer.Misc.Guid;

    [ClientBuild(17538)]
    public static class UpdateHandlers
    {
        public enum UpdateType
        {
            Values = 0,
            CreateObject1 = 1,
            CreateObject2 = 2,
            DestroyObjects = 3,
        }
        
        [Parser(Opcode.SMSG_UPDATE_OBJECT, 0x0C22)]
        public static void HandleUpdateObject(Packet packet)
        {
            ushort mapId = packet.ReadUInt16("Map ID");
            uint blockCount = packet.ReadUInt32("Block count");

            for (uint blockId = 0; blockId < blockCount; blockId++)
            {
                byte updateType = packet.ReadByte("{0}: Update type", blockId);
                UpdateType type = (UpdateType)updateType;

                packet.SetLastDataField("UpdateType.{0}", type.ToString());
                Debug.Print("Block {0}: {1}", blockId, type.ToString());

                switch (type)
                {
                    case UpdateType.CreateObject1:
                    case UpdateType.CreateObject2:
                        HandleCreateUpdateBlock(blockId, packet, type);
                        break;

                    case UpdateType.Values:
                    case UpdateType.DestroyObjects:
                        Debug.Print("{0} is not yet handled.", type.ToString());
                        break;
                }
            }
        }

        private static void HandleCreateUpdateBlock(uint blockId, Packet packet, UpdateType updateType)
        {
            Guid guid = packet.ReadPackedGUID("{0}: Guid (packed)", blockId);
            ObjectType objectTypeId = (ObjectType) packet.ReadByte("{0}: Object type", blockId);
            packet.SetLastDataField("ObjectType.{0}", objectTypeId.ToString());

            HandleMovementUpdate(blockId, packet);
            HandleValuesUpdate(blockId, updateType, objectTypeId, packet);
        }

        private static void HandleMovementUpdate(uint blockId, Packet packet)
        {
            ulong guid;
            long compressedQuaternion;
            byte[] objectGuid = new byte[8];
            bool[] unk = new bool[23];
            bool isSelf, hasTarget, hasStationaryPosition, isAlive, hasAnimKits, 
                isVehicle, hasGoTransportPosition, isGameObject, noRotation, isTransport;
            uint transportFrames, unkNumber, unkNumber2, unkNumber3;
            float positionX, positionY, positionZ, orientation;
            float flySpeed, runSpeed, walkSpeed, swimBackSpeed, flyBackSpeed, turnSpeed, pitchSpeed, 
                swimSpeed, runBackSpeed;

            unk[0] = packet.ReadBit("(MovementUpdate) {0}. Unk bit 0", blockId);
            unk[1] = packet.ReadBit("{0}. Unk bit 1", blockId);
            unk[2] = packet.ReadBit("{0}. Unk bit 2", blockId);
            unk[3] = packet.ReadBit("{0}. Unk bit 3", blockId);
            unk[4] = packet.ReadBit("{0}. Unk bit 4", blockId);
            isSelf = packet.ReadBit("{0}. IsSelf", blockId);
            unk[5] = packet.ReadBit("{0}. Unk bit 5", blockId);
            hasTarget = packet.ReadBit("{0}. HasTarget", blockId);
            transportFrames = packet.ReadBits(22, "{0}. TransportFrames", blockId);
            unk[6] = packet.ReadBit("{0}. Unk bit 6", blockId);
            unk[7] = packet.ReadBit("{0}. Unk bit 7", blockId);
            unk[8] = packet.ReadBit("{0}. Unk bit 8", blockId);
            unk[9] = packet.ReadBit("{0}. Unk bit 9", blockId);
            hasStationaryPosition = packet.ReadBit("{0}. HasStationaryPosition", blockId);
            unk[10] = packet.ReadBit("{0}. Unk bit 10", blockId);
            isAlive = packet.ReadBit("{0}. IsAlive", blockId);
            hasAnimKits = packet.ReadBit("{0}. HasAnimKits", blockId);
            isVehicle = packet.ReadBit("{0}. IsVehicle", blockId);
            hasGoTransportPosition = packet.ReadBit("{0}. HasGoTransportPosition", blockId);
            unk[11] = packet.ReadBit("{0}. Unk bit 11", blockId);
            isGameObject = packet.ReadBit("{0}. IsGameObject", blockId);

            if (hasGoTransportPosition)
            {
                // TODO: Implement this.
                Debug.Print("MovementUpdate broken for block {0}: HasGoTransportPosition is unsupported.");
            }

            if (isAlive)
            {
                objectGuid[4] = packet.ReadBit("{0}. guidMask[4]", blockId);
                objectGuid[1] = packet.ReadBit("{0}. guidMask[1]", blockId);
                unkNumber = packet.ReadBits(19, "{0}. unkNumber", blockId);
                objectGuid[5] = packet.ReadBit("{0}. guidMask[5]", blockId);
                noRotation = packet.ReadBit("{0}. noRotation", blockId);
                objectGuid[7] = packet.ReadBit("{0}. guidMask[7]", blockId);
                unkNumber2 = packet.ReadBits(22, "{0}. unkNumber2", blockId);
                unk[12] = packet.ReadBit("{0}. Unk bit 12", blockId);
                unk[13] = packet.ReadBit("{0}. Unk bit 13", blockId);
                unk[14] = packet.ReadBit("{0}. Unk bit 14", blockId);
                objectGuid[3] = packet.ReadBit("{0}. guidMask[3]", blockId);
                unk[15] = packet.ReadBit("{0}. Unk bit 15", blockId);
                unk[16] = packet.ReadBit("{0}. Unk bit 16", blockId);
                unk[17] = packet.ReadBit("{0}. Unk bit 17", blockId);
                unk[18] = packet.ReadBit("{0}. Unk bit 18", blockId);
                objectGuid[2] = packet.ReadBit("{0}. guidMask[2]", blockId);
                unk[19] = packet.ReadBit("{0}. Unk bit 19", blockId);
                objectGuid[0] = packet.ReadBit("{0}. guidMask[0]", blockId);
                isTransport = packet.ReadBit("{0}. isTransport", blockId);
                objectGuid[6] = packet.ReadBit("{0}. guidMask[6]", blockId);
                unk[20] = packet.ReadBit("{0}. Unk bit 20", blockId);
                unk[21] = packet.ReadBit("{0}. Unk bit 21", blockId);
                unk[22] = packet.ReadBit("{0}. Unk bit 22", blockId);
            }

            if (packet.BitsRemaining() > 0)
                packet.ReadBits(packet.BitsRemaining(), "{0}. Remaining bits in flushed byte.", blockId);

            if (isAlive)
            {
                positionY = packet.ReadSingle("{0}. PositionY", blockId);
                flySpeed = packet.ReadSingle("{0}. FlySpeed", blockId);
                runSpeed = packet.ReadSingle("{0}. RunSpeed", blockId);
                packet.ReadXORByte(ref objectGuid, 4, "{0}. guid[4]", blockId);
                walkSpeed = packet.ReadSingle("{0}. WalkSpeed", blockId);
                packet.ReadXORByte(ref objectGuid, 5, "{0}. guid[5]", blockId);
                unkNumber3 = packet.ReadUInt32("{0}. unkNumber3", blockId);
                packet.ReadXORByte(ref objectGuid, 1, "{0}. guid[1]", blockId);
                swimBackSpeed = packet.ReadSingle("{0}. SwimBackSpeed", blockId);
                flyBackSpeed = packet.ReadSingle("{0}. FlyBackSpeed", blockId);
                packet.ReadXORByte(ref objectGuid, 6, "{0}. guid[6]", blockId);
                turnSpeed = packet.ReadSingle("{0}. TurnSpeed", blockId);
                positionX = packet.ReadSingle("{0}. PositionX", blockId);
                orientation = packet.ReadSingle("{0}. Orientation", blockId);
                pitchSpeed = packet.ReadSingle("{0}. PitchSpeed", blockId);
                swimSpeed = packet.ReadSingle("{0}. SwimSpeed", blockId);
                packet.ReadXORByte(ref objectGuid, 3, "{0}. guid[3]", blockId);
                runBackSpeed = packet.ReadSingle("{0}. RunBackSpeed", blockId);
                packet.ReadXORByte(ref objectGuid, 7, "{0}. guid[7]", blockId);
                packet.ReadXORByte(ref objectGuid, 2, "{0}. guid[2]", blockId);
                positionZ = packet.ReadSingle("{0}. PositionZ", blockId);
                packet.ReadXORByte(ref objectGuid, 0, "{0}. guid[0]", blockId);

                guid = System.BitConverter.ToUInt64(objectGuid, 0);

                var bits = new BitArray(objectGuid);
                packet.AddIgnoredRead("{0}. Guid (constructed)", guid.ToString(), typeof(ulong), sizeof(ulong), bits, false, blockId);
            }

            if (hasStationaryPosition)
            {
                positionX = packet.ReadSingle("{0}. [stationary] PositionX", blockId);
                positionZ = packet.ReadSingle("{0}. [stationary] PositionZ", blockId);
                positionY = packet.ReadSingle("{0}. [stationary] PositionY", blockId);
                orientation = packet.ReadSingle("{0}. [stationary] Orientation", blockId);
            }

            if (isGameObject)
            {
                compressedQuaternion = packet.ReadInt64("{0}. Compressed quaternion", blockId);

                // TODO
                // var quaternion = new Quaternion(compressedQuaternion);
            }

        }

        private static void HandleValuesUpdate(uint blockId, UpdateType updateType, ObjectType objectType, Packet packet)
        {
            var maskSizeBlocks = packet.ReadByte("(ValuesUpdate) {0}. MaskSize", blockId);
            var maskSizeBytes = maskSizeBlocks * 4;
            var maskSizeBits = maskSizeBytes * 8;
            var bits = new BitArray(maskSizeBits);
            var fields = new Hashtable();
            byte dynamicUpdateCount;

            for (int i = 0, b = 0; i < maskSizeBlocks; i++)
            {
                uint mask = packet.ReadUInt32("{0}. Mask[{1}]", blockId, i);
                packet.SetLastDataField("{0:X8}", mask);

                var bytes = BitConverter.GetBytes(mask);
                var blockBits = new BitArray(bytes);

                for (int x = 0; x < 32; x++)
                    bits[b++] = blockBits[x];
            }

            for (int i = 0; i < maskSizeBits; i++)
            {
                if (!bits.Get(i))
                    continue;

                ObjectFields objectField = (ObjectFields)i;
                UnitFields unitField = (UnitFields)i;
                PlayerFields playerField = (PlayerFields)i;

                if (Enum.IsDefined(typeof(ObjectFields), i))
                    Debug.Print(objectField.GetFullName());
                else if (Enum.IsDefined(typeof(UnitFields), i))
                    Debug.Print(unitField.GetFullName());
                else if (Enum.IsDefined(typeof(PlayerFields), i))
                    Debug.Print(playerField.GetFullName());
                else
                    Debug.Print("Unknown: {0}", i);

                if (i < (int)ObjectFields.End)
                {
                    switch (objectField)
                    {
                        case ObjectFields.Scale:
                            fields[i] = packet.ReadSingle(objectField.GetFullName());
                            break;

                        default:
                            fields[i] = packet.ReadUInt32(objectField.GetFullName());
                            break;
                    }
                }
                else
                {
                    // NOTE: This assumes a unit or player of some kind.
                    // It does not allow for game objects or such (yet).
                    switch (objectType)
                    {
                        case ObjectType.Player:
                        case ObjectType.Unit:
                            switch (i)
                            {
                                case (int)UnitFields.BoundingRadius:
                                case (int)UnitFields.CombatReach:
                                case (int)UnitFields.MinDamage:
                                case (int)UnitFields.MaxDamage:
                                case (int)UnitFields.MinOffHandDamage:
                                case (int)UnitFields.MaxOffHandDamage:
                                case (int)UnitFields.ModCastingSpeed:
                                case (int)UnitFields.ModHaste:
                                case (int)UnitFields.ModRangedHaste:
                                case (int)UnitFields.AttackPowerMultiplier:
                                case (int)UnitFields.RangedAttackPowerMultiplier:
                                case (int)UnitFields.MinRangedDamage:
                                case (int)UnitFields.MaxRangedDamage:
                                case (int)UnitFields.PowerCostMultiplier:
                                case (int)PlayerFields.BlockPercentage:
                                case (int)PlayerFields.DodgePercentage:
                                case (int)PlayerFields.ParryPercentage:
                                case (int)PlayerFields.CritPercentage:
                                case (int)PlayerFields.RangedCritPercentage:
                                case (int)PlayerFields.OffhandCritPercentage:
                                case (int)PlayerFields.Mastery:
                                case (int)PlayerFields.ModHealingPercent:
                                case (int)PlayerFields.ModHealingDonePercent:
                                case (int)PlayerFields.WeaponDmgMultipliers:
                                case (int)PlayerFields.ModSpellPowerPercent:
                                case (int)PlayerFields.ModResiliencePercent:
                                case (int)PlayerFields.OverrideSpellPowerByAPPercent:
                                case (int)PlayerFields.OverrideAPBySpellPowerPercent:
                                case (int)PlayerFields.RuneRegen:
                                case (int)PlayerFields.RuneRegen1:
                                case (int)PlayerFields.RuneRegen2:
                                case (int)PlayerFields.RuneRegen3:
                                case (int)PlayerFields.UiHitModifier:
                                case (int)PlayerFields.UiSpellHitModifier:
                                case (int)PlayerFields.HomeRealmTimeOffset:
                                case (int)PlayerFields.ModPetHaste:
                                    fields[i] = packet.ReadSingle(i < (int)UnitFields.End ? unitField.GetFullName() : playerField.GetFullName());
                                    break;

                                default:
                                    fields[i] = packet.ReadUInt32(i < (int) UnitFields.End ? unitField.GetFullName() : playerField.GetFullName());
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            dynamicUpdateCount = packet.ReadByte("Dynamic update count");
        }
    }
}
