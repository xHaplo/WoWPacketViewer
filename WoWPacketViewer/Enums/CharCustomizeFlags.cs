using System;

namespace WoWPacketViewer.Enums
{
    [FlagsAttribute]
    public enum CharCustomizeFlags
    {
        CHAR_CUSTOMIZE_NONE = 0x00000000,
        CHAR_CUSTOMIZE_NAME_GENDER_APPEARANCE = 0x00000001,       // name/gender/appearance
        CHAR_CUSTOMIZE_FACTION = 0x00010000,       // name/gender/appearance/race of opposite faction
        CHAR_CUSTOMIZE_RACE = 0x00100000,       // name/gender/appearance/race of same faction
    };
}
