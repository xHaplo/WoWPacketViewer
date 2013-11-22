using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WoWPacketViewer.Misc
{
    public static class ClientVersion
    {
        public static SortedDictionary<uint, string> AvailableVersions = new SortedDictionary<uint, string>();

        public static void AddAvailableVersion(uint clientBuild)
        {
            // If this version is already added, no point in doing so again.
            if (AvailableVersions.ContainsKey(clientBuild))
                return;

            // Attempt to obtain the corresponding client version.
            // If it's unknown, no point adding it.
            string clientVersion;
            if (GetVersionForBuild(clientBuild, out clientVersion))
            {
                Debug.Print("Identified version {0} for build {1}.", clientVersion, clientBuild);
                AvailableVersions.Add(clientBuild, clientVersion);
            }
        }

        private static bool GetVersionForBuild(uint clientBuild, out string version)
        {
            switch (clientBuild)
            {
                case 17399: 
                    version = "5.4.0";
                    break;

                case 17538: 
                    version = "5.4.1";
                    break;

                default:
                    Debug.Print("Client build {0} is unknown.", clientBuild);
                    version = "<unknown>";
                    return false;
            }

            return true;
        }
    }
}
