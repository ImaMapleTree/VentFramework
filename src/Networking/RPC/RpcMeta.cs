using System;
using System.Collections.Generic;
using Hazel;

namespace VentLib.Networking.RPC;

public struct RpcMeta
{
    public static List<Action<RpcMeta>> Subscribers = new();
    public static void AddSubscriber(Action<RpcMeta> metaConsumer) => Subscribers.Add(metaConsumer);
    
    public int Recipient { get; internal set; }
    public uint NetId { get; internal set; }
    public byte CallId { get; internal set; }
    public bool Immediate { get; internal set; }
    public bool RequiresHost { get; internal set; }
    public SendOption SendOption { get; internal set; }

    public void Notify()
    {
        foreach (Action<RpcMeta> subscriber in Subscribers)
        {
            subscriber.Invoke(this);
        }
    }
}