using System;

namespace WoWPacketViewer
{
    public static class Utils
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime GetDateTimeFromUnixTime(double unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static double GetUnixTimeFromDateTime(DateTime time)
        {
            return (time - Epoch).TotalSeconds;
        }

        public static byte ConvertUppercaseHexByte(string hexString, int byteOffset)
        {
            byte b = (byte)(GetHexValueFromUppercaseChar(hexString[byteOffset]) << 4);
            b += GetHexValueFromUppercaseChar(hexString[byteOffset + 1]);
            return b;
        }

        private static byte GetHexValueFromUppercaseChar(char charValue)
        {
            byte val = (byte)charValue;
            return (byte)(val - (val < 58 ?  48 : 55));
        }
    }
}
