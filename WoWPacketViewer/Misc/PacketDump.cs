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
            string line = "";
            int lineNo = 0;

            try
            {
                using (var sr = new StreamReader(filename))
                {
                    bool isReadingPacket = false;
                    int lineBreakCounter = 0;
                    uint opcode = 0;
                    int[] borderPositions = new int[3];
                    var packetBuffer = new List<byte>();
                    Direction direction = Direction.ServerToClient;

                    const string lineBreak = "---";
                    const string arctiumNewBlockIndicator = "Client: ";
                    const string arctiumOpcodeIndicator = "Value: 0x";
                    const string arctiumClientToServerIndicator = "Type: ClientMessage";
                    const string arctiumServerToClientIndicator = "Type: ServerMessage";
                    const string ascentServerOpcodeIndicator = "{SERVER} Packet: (0x";
                    const string ascentClientOpcodeIndicator = "{CLIENT} Packet: (0x";

                    while ((line = sr.ReadLine()) != null)
                    {
                        ++lineNo;
                        if (line.Length == 0)
                            continue;

#if PARSER_DEBUG
                        Debug.Print("Reading line {0}: {1}", lineNo, line);
#endif

                        // Attempt to identify the start of a packet
                        if (!isReadingPacket)
                        {
#if PARSER_DEBUG
                            Debug.Print("Not currently in the middle of a packet block.");
#endif

                            // If we hit a linebreak, we've reached the header block.
                            // After the second one (end of the header block), we can start reading the packet.
                            if (line.Contains(lineBreak))
                            {
                                if (++lineBreakCounter == 2)
                                    isReadingPacket = true;

#if PARSER_DEBUG
                                Debug.Print("Line break found (that makes {0}). Read packet now: {1}", lineBreakCounter, isReadingPacket);
#endif
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

#if PARSER_DEBUG
                                Debug.Print("Found Arctium opcode: {0:X4}", opcode);
#endif
                            }
                            else if (line.StartsWith(ascentClientOpcodeIndicator))
                            {
                                var s = line.Substring(ascentClientOpcodeIndicator.Length, 4);
                                opcode = Convert.ToUInt32(s, 16);
                                direction = Direction.ClientToServer;
#if PARSER_DEBUG
                                Debug.Print("Found Ascent client opcode: {0:X4}", opcode);
#endif
                            }
                            else if (line.StartsWith(ascentServerOpcodeIndicator))
                            {
                                var s = line.Substring(ascentServerOpcodeIndicator.Length, 4);
                                opcode = Convert.ToUInt32(s, 16);
                                direction = Direction.ServerToClient;
#if PARSER_DEBUG
                                Debug.Print("Found Ascent server opcode: {0:X4}", opcode);
#endif
                            }
                            else if (line.StartsWith(arctiumClientToServerIndicator))
                            {
                                direction = Direction.ClientToServer;
#if PARSER_DEBUG
                                Debug.Print("Found Arctium direction indicator: {0}", direction);
#endif
                            }
                            else if (line.StartsWith(arctiumServerToClientIndicator))
                            {
                                direction = Direction.ServerToClient;
#if PARSER_DEBUG
                                Debug.Print("Found Arctium direction indicator: {0}", direction);
#endif
                            }
                        }
                        // Keep reading until we're not
                        else
                        {
#if PARSER_DEBUG
                            Debug.Print("Currently reading packet.");
#endif

                            bool finishReadingBlock = false;

                            // If we hit the linebreak, stop reading the packet.
                            if (line.Contains(lineBreak))
                            {
#if PARSER_DEBUG
                                Debug.Print("Line break found (that makes {0}).", lineBreakCounter + 1);
#endif
                                if (++lineBreakCounter >= 3)
                                    finishReadingBlock = true;
                            }
                            else if (line.StartsWith(arctiumNewBlockIndicator))
                            {
#if PARSER_DEBUG
                                Debug.Print("Block wasn't terminated correctly by Arctium, starting new block.");
#endif
                                finishReadingBlock = true;
                            }

                            if (finishReadingBlock)
                            {
#if PARSER_DEBUG
                                Debug.Print("Finished reading block.");
#endif
                                packets.Add(new Packet(opcode, packetBuffer.ToArray(), direction));
                                isReadingPacket = false;
                                opcode = 0;
                                lineBreakCounter = 0;
                                packetBuffer.Clear();
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

#if PARSER_DEBUG
                            Debug.Print("Line has {0} border(s).", borderCount);
#endif

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

#if PARSER_DEBUG
                            Debug.Print("Hex: {0}", hexBytes);
#endif

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
                Debug.Print("Exception occurred at line {0}.", lineNo);
                Debug.Print("Line responsible: {0}", line);
                Debug.Print(ex.Message);
                return false;
            }
        }
    }
}
