using System;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer.Parsing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ClientBuildAttribute : Attribute
    {
        public ClientBuildAttribute(uint clientBuild)
        {
            ClientBuild = clientBuild;
        }

        public uint ClientBuild { get; private set; }
    }
}
