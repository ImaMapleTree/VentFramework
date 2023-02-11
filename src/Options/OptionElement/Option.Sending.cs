using Hazel;
using VentLib.RPC.Interfaces;

namespace VentLib.Options.OptionElement;

public partial class Option: IRpcSendable<Option>
{
    public Option Read(MessageReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void Write(MessageWriter writer)
    {
        throw new System.NotImplementedException();
    }
}