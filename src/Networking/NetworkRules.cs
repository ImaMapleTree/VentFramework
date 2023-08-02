using UnityEngine;
using VentLib.Options;
using VentLib.Options.IO;
using VentLib.Utilities.Attributes;

namespace VentLib.Networking;

[LoadStatic]
public class NetworkRules
{
    // TODO: make configurable
    public const int AbsoluteMaxPacketSize = 1024;
    public const int AbsoluteMinPacketSize = 256;
    internal const int AmongUsMenuCallId = 42069 & 255;
    
    
    public static int MaxPacketSize
    {
        get => _maxPacketSize;
        set => _packetSizeOption.SetHardValue(_maxPacketSize = Mathf.Clamp(value, AbsoluteMinPacketSize, AbsoluteMaxPacketSize));
    }
    private static int _maxPacketSize;

    public static bool AllowRoomDiscovery
    {
        get => _allowRoomDiscovery;
        set => _roomDiscoveryOption.SetHardValue(_allowRoomDiscovery = value);
    }
    private static bool _allowRoomDiscovery;
    
    
    private static Option _packetSizeOption;
    private static Option _roomDiscoveryOption;
    

    static NetworkRules()
    {
        OptionManager networkManager = OptionManager.GetManager(file: "network.config");
        _packetSizeOption = new OptionBuilder()
            .Key("Max Packet Size")
            .Value(AbsoluteMaxPacketSize)
            .IOSettings(io => io.UnknownValueAction = ADEAnswer.Allow)
            .BindInt(_ => _packetSizeOption?.Manager?.DelaySave())
            .BuildAndRegister(networkManager);

        _maxPacketSize = _packetSizeOption.GetValue<int>();

        _roomDiscoveryOption = new OptionBuilder()
            .Key("Allow Room Discovery")
            .Values(0, true, false)
            .BindBool(_ => _roomDiscoveryOption?.Manager?.DelaySave())
            .BuildAndRegister(networkManager);

        _allowRoomDiscovery = _roomDiscoveryOption.GetValue<bool>();
    }
}