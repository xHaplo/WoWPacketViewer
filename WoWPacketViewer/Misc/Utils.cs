namespace WoWPacketViewer
{
    public static class Utils
    {
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
