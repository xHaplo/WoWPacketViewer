using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using WoWPacketViewer.Enums;
using CustomExtensions;

namespace WoWPacketViewer.Misc
{
    public static class PacketDump
    {
        public static bool Load(string filename, ref List<Packet> packets)
        {
            try
            {
                using (var sr = new StreamReader(filename))
                {
                    string line;
                    bool isReadingPacket = false;
                    int lineBreakCounter = 0;
                    uint opcode = 0;
                    int[] borderPositions = new int[3];
                    var packetBuffer = new List<byte>();
                    Direction direction = Direction.ServerToClient;

                    const string lineBreak = "---";
                    const string arctiumOpcodeIndicator = "Value: 0x";
                    const string arctiumClientToServerIndicator = "Type: ClientMessage";
                    const string arctiumServerToClientIndicator = "Type: ServerMessage";
                    const string ascentServerOpcodeIndicator = "{SERVER} Packet: (0x";
                    const string ascentClientOpcodeIndicator = "{CLIENT} Packet: (0x";

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length == 0)
                            continue;

                        // Attempt to identify the start of a packet
                        if (!isReadingPacket)
                        {
                            // If we hit a linebreak, we've reached the header block.
                            // After the second one (end of the header block), we can start reading the packet.
                            if (line.Contains(lineBreak))
                            {
                                if (++lineBreakCounter == 2)
                                    isReadingPacket = true;

                                continue;
                            }

                            if (line.StartsWith(arctiumOpcodeIndicator))
                            {
                                // isn't necessarily 4 digits long, so go to the space.
                                var startPos = arctiumOpcodeIndicator.Length;
                                var endPos = line.IndexOf(' ', startPos);
                                var length = endPos - startPos;
                                var s = line.Substring(startPos, length);

                                opcode = Convert.ToUInt32(s, 16);
                            }
                            else if (line.StartsWith(ascentClientOpcodeIndicator))
                            {
                                var s = line.Substring(ascentClientOpcodeIndicator.Length, 4);
                                opcode = Convert.ToUInt32(s, 16);
                                direction = Direction.ClientToServer;
                            }
                            else if (line.StartsWith(ascentServerOpcodeIndicator))
                            {
                                var s = line.Substring(ascentServerOpcodeIndicator.Length, 4);
                                opcode = Convert.ToUInt32(s, 16);
                                direction = Direction.ServerToClient;
                            }
                            else if (line.StartsWith(arctiumClientToServerIndicator))
                            {
                                direction = Direction.ClientToServer;
                            }
                            else if (line.StartsWith(arctiumServerToClientIndicator))
                            {
                                direction = Direction.ServerToClient;
                            }
                        }
                        // Keep reading until we're not
                        else
                        {
                            // If we hit the linebreak, stop reading the packet.
                            if (line.Contains(lineBreak))
                            {
                                if (++lineBreakCounter == 3)
                                {
                                    packets.Add(new Packet(opcode, packetBuffer.ToArray(), direction));
                                    isReadingPacket = false;
                                    opcode = 0;
                                    lineBreakCounter = 0;
                                    packetBuffer.Clear();
                                }

                                continue;
                            }

                            // How many borders do we have?
                            uint borderCount = 0;
                            int borderPos = 0, n = 0;
                            while ((borderPos = line.IndexOf('|', borderPos)) != -1)
                            {
                                if (n < borderPositions.Length)
                                    borderPositions[n++] = borderPos;

                                ++borderCount;
                                ++borderPos;
                            }

                            // 3 borders means ascii dump, 2 doesn't. 
                            // We don't really care either way, but it's unusual for it to be anything different.
                            if (borderCount == 1)
                                borderPositions[1] = line.Length;

                            var startPos = borderPositions[0] + 1;
                            var endPos = borderPositions[1] - 1;
                            var length = endPos - startPos;

                            // Border all by itself.
                            if (length <= 0)
                                continue;

                            var hexBytes = line.Substring(startPos, length);

                            // Filter out spaces. Arctium separates more with 2 spaces, Ascent 1.
                            // Arctium also pads against the borders, so we want to make them the same.
                            hexBytes = hexBytes.RemoveWhitespace();

                            // Convert the string to uppercase, so we can handle it faster.
                            hexBytes = hexBytes.ToUpper();

                            // Convert the hex string into a byte array.
                            for (var i = 0; i < hexBytes.Length >> 1; i++)
                                packetBuffer.Add(Utils.ConvertUppercaseHexByte(hexBytes, i << 1));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }
    }
}
