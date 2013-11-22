using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Misc
{
    public class Guid
    {
        public Guid(ulong guid)
        {
            Value = guid;
        }

        public ulong Value
        {
            get;
            set;
        }

        public HighGuidType HighType
        {
            get
            {
                if (Value == 0)
                    return HighGuidType.None;

                var highGUID = (HighGuidType)((Value & 0xF0F0000000000000) >> 52);
                return highGUID == 0 ? HighGuidType.Player : highGUID;
            }
        }

        public ObjectType ObjectType
        {
            get
            {
                switch (HighType)
                {
                    case HighGuidType.Player:
                        return ObjectType.Player;
                    case HighGuidType.DynObject:
                        return ObjectType.DynamicObject;
                    case HighGuidType.Item:
                        return ObjectType.Item;
                    case HighGuidType.GameObject:
                    case HighGuidType.Transport:
                    case HighGuidType.MOTransport:
                        return ObjectType.GameObject;
                    case HighGuidType.Vehicle:
                    case HighGuidType.Unit:
                    case HighGuidType.Pet:
                        return ObjectType.Unit;
                    default:
                        return ObjectType.Object;
                }
            }
        }

        public static bool operator ==(Guid lhs, Guid rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(Guid lhs, Guid rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Guid && Equals((Guid)obj);
        }

        public bool Equals(Guid other)
        {
            return other.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
