using AmongUs.GameOptions;
using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class GameOptionsInserter: IRpcInserter<IGameOptions>
{
    public void Insert(IGameOptions value, MessageWriter writer)
    {
        writer.WriteBytesAndSize(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(value));
    }
}