using System;
using Hazel;
using InnerNet;
using VentLib.Networking.Interfaces;

namespace VentLib.Networking.RPC.Interfaces;

// ReSharper disable once InconsistentNaming
public interface MonoRpc: IStrongRpc
{
    public MonoRpc SetBody(RpcBody body);

    public MonoRpc Write(object obj);

    public MonoRpc Write(IRpcWritable obj);

    public MonoRpc Write(bool b);
    
    public MonoRpc Write(byte b);
    
    public MonoRpc Write(float b);
    
    public MonoRpc Write(int b);
    
    public MonoRpc Write(sbyte b);
    
    public MonoRpc Write(string b);
    
    public MonoRpc Write(uint b);
    
    public MonoRpc Write(ulong b);
    
    public MonoRpc Write(ushort b);

    public MonoRpc Write(InnerNetObject innerNetObject);

    public MonoRpc WriteCustom<T>(T obj, Action<T, MessageWriter> writerFunction);

    public MonoRpc WritePacked(object obj);
    
    public MonoRpc WritePacked(int i);
    
    public MonoRpc WritePacked(uint ui);

    public MonoRpc Protected(bool isProtected);

    public MonoRpc ThreadSafe(bool threadSafe);

    public void Send(int clientId = -1);

    public void SendInclusive(params int[] clientIds);

    public void SendExcluding(params int[] clientIds);

    public MonoRpc Clone();
}