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
        private static HandlerMap _packetHandlers;
        private static OpcodeMap _opcodeMap;

        public static void LoadHandlers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var identifiedClientBuilds = new List<uint>();

            _packetHandlers = new HandlerMap();
            _opcodeMap = new OpcodeMap();

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

                    foreach (var attribute in attributes)
                    {
                        if (attribute.OpcodeValue == 0)
                            continue;

                        var key = new KeyValuePair<uint, uint>(clientBuild, attribute.OpcodeValue);
                        var handler = (PacketHandler)Delegate.CreateDelegate(typeof(PacketHandler), method);
                        if (_packetHandlers.ContainsKey(key))
                        {
                            Debug.Print("Handler already found for opcode 0x{0:X4} ({1}) of client build {2}.", 
                                attribute.OpcodeValue, attribute.Opcode, clientBuild);
                            continue;
                        }

                        if (!identifiedClientBuilds.Contains(clientBuild))
                            identifiedClientBuilds.Add(clientBuild);

                        _packetHandlers[key] = handler;
                        _opcodeMap[key] = attribute.Opcode;

                        Debug.Print("Handler added for opcode 0x{0:X4} ({1}) of client build {2}.",
                            attribute.OpcodeValue, attribute.Opcode, clientBuild);
                    }
                }
            }

            // Now look through the builds we've discovered and identify their versions.
            foreach (var clientBuild in identifiedClientBuilds)
                ClientVersion.AddAvailableVersion(clientBuild);
        }

        public static Opcode LookupOpcode(uint clientBuild, uint opcodeValue)
        {
            var key = new KeyValuePair<uint, uint>(clientBuild, opcodeValue);
            if (!_opcodeMap.ContainsKey(key))
                return Opcode.NO_REGISTERED_HANDLER;

            return _opcodeMap[key];
        }

        public static void HandlePacket(uint clientBuild, Packet packet)
        {
            var key = new KeyValuePair<uint, uint>(clientBuild, packet.OpcodeValue);
            if (packet.Opcode == Opcode.NO_REGISTERED_HANDLER
                || !_packetHandlers.ContainsKey(key))
            {
                Debug.Print("No handler for opcode {0} (0x{1:X4}) under client build {2}.",
                    packet.Opcode, packet.OpcodeValue, clientBuild);
                return;
            }

            _packetHandlers[key](packet);
        }
    }
}