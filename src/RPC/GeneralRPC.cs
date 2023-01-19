using Hazel;

namespace VentLib.RPC;

public static class GeneralRPC
{
    public static void SendGameData(int clientId = -1)
    {
        MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
        // 書き込み {}は読みやすさのためです。
        writer.StartMessage((byte)(clientId == -1 ? 5 : 6)); //0x05 GameData
        {
            writer.Write(AmongUsClient.Instance.GameId);
            if (clientId != -1)
                writer.WritePacked(clientId);
            writer.StartMessage(1); //0x01 Data
            {
                writer.WritePacked(GameData.Instance.NetId);
                GameData.Instance.Serialize(writer, true);
            }
            writer.EndMessage();
        }
        writer.EndMessage();

        AmongUsClient.Instance.SendOrDisconnect(writer);
        writer.Recycle();
    }
}