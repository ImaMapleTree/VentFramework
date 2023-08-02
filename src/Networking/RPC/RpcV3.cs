using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using VentLib.Logging;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Inserters;
using VentLib.Networking.RPC.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Debug.Profiling;

// ReSharper disable ParameterHidesMember

namespace VentLib.Networking.RPC;

public class RpcV3: MonoRpc, MassRpc, IChainRpc
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(RpcV3));
    private uint netId;
    private byte callId;
    private SendOption sendOption;
    private bool immediate;
    private RpcBody? rpcBody;
    private bool isProtected;
    private bool isThreadSafe;

    private List<RpcV3> chainedRpcs = null!;
    private MassRpc parentRpc = null!;

    private RpcMeta? lastMeta;
    
    private RpcV3()
    {
    }
    
    public static MonoRpc Immediate(uint netId, RpcCalls callId, SendOption sendOption = SendOption.Reliable, RpcBody? rpcBody = null, bool isProtected = false) => Immediate(netId, (byte)callId, sendOption, rpcBody);

    public static MonoRpc Standard(uint netId, RpcCalls callId, SendOption sendOption = SendOption.Reliable, RpcBody? rpcBody = null, bool isProtected = false) => Standard(netId, (byte)callId, sendOption, rpcBody);
    
    public static MonoRpc Immediate(uint netId, byte callId, SendOption sendOption = SendOption.Reliable, RpcBody? rpcBody = null, bool isProtected = false)
    {
        return new RpcV3
        {
            netId = netId,
            callId = callId,
            sendOption = sendOption,
            immediate = true,
            rpcBody = rpcBody ?? new RpcBody(),
            isProtected = isProtected
        };
    }
    
    public static MonoRpc Standard(uint netId, byte callId, SendOption sendOption = SendOption.Reliable, RpcBody? rpcBody = null, bool isProtected = false)
    {
        return new RpcV3
        {
            netId = netId,
            callId = callId,
            sendOption = sendOption,
            immediate = false,
            rpcBody = rpcBody ?? new RpcBody(),
            isProtected = isProtected
        };
    }

    public static MassRpc Mass(SendOption sendOption = SendOption.None, bool isProtected = false, params IStrongRpc[]? messages)
    {
        List<RpcV3> strongRpcs = (messages ?? Array.Empty<IStrongRpc>()).Cast<RpcV3>().SelectMany(r => r.chainedRpcs != null! ? r.FlattenChained() : new List<RpcV3> { r }).ToList();
        return new RpcV3
        {
            callId = 100,
            sendOption = sendOption,
            isProtected = isProtected,
            chainedRpcs = strongRpcs
        };
    }
    
    public MassRpc Add(IStrongRpc rpc)
    {
        RpcV3 v3 = (RpcV3)rpc;
        if (v3.chainedRpcs == null!) chainedRpcs.Add(v3);
        else chainedRpcs.AddRange(v3.FlattenChained());
        return this;
    }
    
    public IChainRpc Start(uint netId, byte callId)
    {
        RpcV3 childRpc = new()
        {
            netId = netId,
            callId = callId,
            rpcBody = new RpcBody(),
            parentRpc = this
        };
        chainedRpcs.Add(childRpc);
        return childRpc;
    }
    
    public MassRpc End() => parentRpc;

    public MonoRpc SetBody(RpcBody body)
    {
        rpcBody = body;
        return this;
    }
    
    public RpcBody ExtractBody()
    {
        return rpcBody!;
    }

    public MonoRpc Write(object obj)
    {
        rpcBody?.Write(obj);
        return this;
    }
    
    public MonoRpc Write(IRpcWritable obj)
    {
        rpcBody?.Write(obj);
        return this;
    }

    public MonoRpc Write(bool b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(byte b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(float b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(int b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(sbyte b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(string b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(uint b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(ulong b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(ushort b)
    {
        rpcBody?.Write(b);
        return this;
    }

    public MonoRpc Write(InnerNetObject innerNetObject)
    {
        rpcBody?.Write(innerNetObject);
        return this;
    }

    IChainRpc IChainRpc.Write(object obj)
    {
        rpcBody?.Write(obj);
        return this;
    }

    public MonoRpc WritePacked(object obj)
    {
        rpcBody?.WritePacked(obj);
        return this;
    }

    public MonoRpc WritePacked(int i)
    {
        rpcBody?.WritePacked(i);
        return this;
    }

    IChainRpc IChainRpc.WritePacked(object obj)
    {
        rpcBody?.WritePacked(obj);
        return this;
    }
    
    public MonoRpc WritePacked(uint ui)
    {
        rpcBody?.WritePacked(ui);
        return this;
    }
    
    public MonoRpc WriteCustom<T>(T obj, Action<T, MessageWriter> writerFunction)
    {
        rpcBody?.WriteCustom(obj, new CustomInserter<T>(writerFunction));
        return this;
    }

    public MonoRpc Protected(bool isProtected)
    {
        this.isProtected = isProtected;
        return this;
    }

    public MonoRpc ThreadSafe(bool threadSafe)
    {
        isThreadSafe = threadSafe;
        return this;
    }
    
    public MonoRpc Clone()
    {
        return new RpcV3
        {
            netId = netId,
            callId = callId,
            sendOption = sendOption,
            immediate = immediate,
            rpcBody = rpcBody?.Clone(),
            isProtected = isProtected,
            isThreadSafe = isThreadSafe
        };
    }

    public RpcMeta? LastMeta() => lastMeta;

    public void SendInclusive(params int[] clientIds)
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            try
            {
                int clientId = player.GetClientId();
                if (clientIds.Contains(clientId)) Send(clientId);
            }
            catch (Exception exception)
            {
                log.Exception("Failed to send RPC.", exception);
            }
        }
    }

    public void SendExcluding(params int[] clientIds)
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            try
            {
                int clientId = player.GetClientId();
                if (!clientIds.Contains(clientId)) Send(clientId);
            }
            catch (Exception exception)
            {
                log.Exception("Failed to send RPC.", exception);
            }
        }
    }

    public void Send(int clientId = -1)
    {
        if (isProtected && !AmongUsClient.Instance.AmHost) return;
        if (isThreadSafe && !MainThreadAnchor.IsMainThread())
        {
            MainThreadAnchor.ExecuteOnMainThread(() => Send(clientId));
            return;
        }
        if (chainedRpcs != null!)
        {
            SendMessages(clientId);
            return;
        }

        Profiler.Sample sample = Profilers.Global.Sampler.Sampled();

        MessageWriter writer = !immediate
            ? AmongUsClient.Instance.StartRpc(netId, callId, sendOption)
            : AmongUsClient.Instance.StartRpcImmediately(netId, callId, sendOption, clientId);
        
        rpcBody?.WriteAll(writer);
        
        lastMeta = GenerateMeta(clientId, writer.Length);
        lastMeta.Notify();

        if (!immediate) writer.EndMessage();
        else AmongUsClient.Instance.FinishRpcImmediately(writer);
        
        sample.Stop();
    }

    private RpcMeta GenerateMeta(int clientId, int packetSize)
    {
        return new RpcMeta
        {
            CallId = callId,
            Immediate = immediate,
            NetId = netId,
            Recipient = clientId,
            RequiresHost = isProtected,
            Arguments = rpcBody!.Arguments,
            SendOption = sendOption,
            PacketSize = packetSize
        };
    }

    private void SendMessages(int clientId)
    {
        MessageWriter writer = MessageWriter.Get(sendOption);
        if (clientId < 0)
        {
            writer.StartMessage(5);
            writer.Write(AmongUsClient.Instance.GameId);
        }
        else
        {
            writer.StartMessage(6);
            writer.Write(AmongUsClient.Instance.GameId);
            writer.WritePacked(clientId);
        }

        List<RpcMeta> childrenMeta = new();
        chainedRpcs.ForEach(chained =>
        {
            int originalSize = writer.Length;
            writer.StartMessage(2);
            writer.WritePacked(chained.netId);
            writer.Write(chained.callId);
            chained.rpcBody?.WriteAll(writer);
            writer.EndMessage();
            childrenMeta.Add(chained.GenerateMeta(clientId, writer.Length - originalSize));
        });

        lastMeta = new RpcMassMeta
        {
            ChildMeta = childrenMeta,
            CallId = callId,
            Immediate = immediate,
            NetId = netId,
            Recipient = clientId,
            RequiresHost = isProtected,
            Arguments = rpcBody?.Arguments ?? new List<object>(),
            SendOption = sendOption,
            PacketSize = writer.Length
        };
        lastMeta.Notify();
        
        writer.EndMessage();
        AmongUsClient.Instance.SendOrDisconnect(writer);
        writer.Recycle();
    }

    private List<RpcV3> FlattenChained()
    {
        if (chainedRpcs == null!) return new List<RpcV3>();
        List<RpcV3> flatList = new(chainedRpcs);
        flatList.AddRange(chainedRpcs.SelectMany(cr => cr.FlattenChained()));
        return flatList;
    }
}