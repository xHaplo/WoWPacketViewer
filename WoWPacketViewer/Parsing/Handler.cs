using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using WoWPacketViewer.Parsing;
using WoWPacketViewer.Misc;
using WoWPacketViewer.Enums;

namespace WoWPacketViewer
{
    using PacketHandler = Action<Packet>;
    using HandlerMap = Dictionary<KeyValuePair<uint /* client build */, uint /* opcode value */>, Action<Packet>>;
    using OpcodeMap = Dictionary<KeyValuePair<uint /* client build */, uint /* opcode value */>, Opcode>;

    public static class Handler
    {
        private static HandlerMap _serverPacketHandlers;
        private static HandlerMap _clientPacketHandlers;
        private static OpcodeMap _serverOpcodeMap;
        private static OpcodeMap _clientOpcodeMap;

        public static void LoadHandlers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var identifiedClientBuilds = new List<uint>();

            _serverPacketHandlers = new HandlerMap();
            _clientPacketHandlers = new HandlerMap();
            _serverOpcodeMap = new OpcodeMap();
            _clientOpcodeMap = new OpcodeMap();

            foreach (var type in types)
            {
                if (!type.IsAbstract
                    || !type.IsPublic)
                    continue;

                // Require each handler class to define the client build.
                var classAttributes = (ClientBuildAttribute[])type.GetCustomAttributes(typeof(ClientBuildAttribute), false);
                if (classAttributes.Length == 0)
                    continue;

                var clientBuild = classAttributes[0].ClientBuild;

                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (!method.IsPublic)
                        continue;

                    var parameters = method.GetParameters();
                    if (parameters.Length == 0
                        || parameters[0].ParameterType != typeof(Packet))
                        continue;

                    var attributes = (ParserAttribute[])method.GetCustomAttributes(typeof(ParserAttribute), false);
                    if (attributes.Length == 0)
                        continue;

                    var attribute = attributes[0];
                    if (attribute.OpcodeValue == 0)
                        continue;

                    var key = CreateKey(clientBuild, attribute.OpcodeValue);
                    var handler = (PacketHandler)Delegate.CreateDelegate(typeof(PacketHandler), method);

                    OpcodeMap opcodeMap;
                    HandlerMap handlerMap;

                    if (attribute.Direction == Direction.ClientToServer)
                    {
                        opcodeMap = _clientOpcodeMap;
                        handlerMap = _clientPacketHandlers;
                    }
                    else
                    {
                        opcodeMap = _serverOpcodeMap;
                        handlerMap = _serverPacketHandlers;
                    }

                    if (handlerMap.ContainsKey(key))
                    {
                        Debug.Print("Handler already found for opcode 0x{0:X4} ({1}) of client build {2}.", 
                            attribute.OpcodeValue, attribute.Opcode, clientBuild);
                        continue;
                    }

                    if (!identifiedClientBuilds.Contains(clientBuild))
                        identifiedClientBuilds.Add(clientBuild);

                    handlerMap[key] = handler;
                    opcodeMap[key] = attribute.Opcode;

                    Debug.Print("Handler added for opcode 0x{0:X4} ({1}) of client build {2}.",
                        attribute.OpcodeValue, attribute.Opcode, clientBuild);
                }
            }

            // Now look through the builds we've discovered and identify their versions.
            foreach (var clientBuild in identifiedClientBuilds)
                ClientVersion.AddAvailableVersion(clientBuild);
        }

        private static KeyValuePair<uint, uint> CreateKey(uint clientBuild, uint opcodeValue)
        {
            return new KeyValuePair<uint,uint>(clientBuild, opcodeValue);
        }

        public static Opcode LookupOpcode(uint clientBuild, uint opcodeValue, Direction direction)
        {
            var key = CreateKey(clientBuild, opcodeValue);
            var opcodeMap = (direction == Direction.ClientToServer ? _clientOpcodeMap : _serverOpcodeMap);

            if (!opcodeMap.ContainsKey(key))
                return Opcode.NO_REGISTERED_HANDLER;

            return opcodeMap[key];
        }

        public static bool HandlePacket(uint clientBuild, Packet packet)
        {
            var key = CreateKey(clientBuild, packet.OpcodeValue);
            var handlerMap = (packet.Direction == Direction.ClientToServer ? _clientPacketHandlers : _serverPacketHandlers);

            if (packet.Opcode == Opcode.NO_REGISTERED_HANDLER
                || !handlerMap.ContainsKey(key))
            {
                Debug.Print("No handler for opcode {0} (0x{1:X4}) under client build {2}.",
                    packet.Opcode, packet.OpcodeValue, clientBuild);
                return false;
            }

            // Reset packet
            packet.Reset();

            // Load the handler.
            try
            {
                handlerMap[key](packet);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }

            return true;
        }
    }
}