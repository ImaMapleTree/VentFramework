using System;
using HarmonyLib;
using Hazel;
using VentLib.Networking.Interfaces;
using VentLib.Networking.Managers;
using VentLib.Utilities;
using VentLib.Version.Assembly;
using VentLib.Version.Git;

namespace VentLib.Version;

public abstract class Version: IRpcSendable<Version>
{
    static Version()
    {
        AbstractConstructors.Register(typeof(Version), ReadStatic);
    }

    private static Version ReadStatic(MessageReader reader)
    {
        VersionType type = (VersionType)reader.ReadByte();
        switch (type)
        {
            case VersionType.None:
                return AccessTools.CreateInstance<NoVersion>();
            case VersionType.Git:
                return AccessTools.CreateInstance<GitVersion>().Read(reader);
            case VersionType.Assembly:
                return AccessTools.CreateInstance<AssemblyVersion>().Read(reader);
            case VersionType.Custom:
                string assemblyName = reader.ReadString();
                var containingAssembly = AssemblyUtils.FindAssemblyFromFullName(assemblyName);
                if (containingAssembly == null) throw new NullReferenceException($"Could not find assembly \"{assemblyName}\" for Custom type");
                string customTypeName = reader.ReadString();
                Type? versionType = containingAssembly.GetType(customTypeName);
                if (versionType == null)
                    throw new NullReferenceException($"Could not find Version class \"{customTypeName}\" in assembly \"{assemblyName}\"");
                object? constructed = AccessTools.CreateInstance(versionType);
                if (constructed is not Version customVersion)
                    throw new ArgumentException($"Constructed type \"{constructed?.GetType()}\" does not inherit VentLib.Version");
                return customVersion.Read(reader);
            default:
                throw new ArgumentOutOfRangeException($"Unexpected VersionType {type}");
        }
    }
    
    public abstract Version Read(MessageReader reader);
    protected abstract void WriteInfo(MessageWriter writer);
    
    public void Write(MessageWriter writer)
    {
        VersionType type = this switch
        {
            GitVersion => VersionType.Git,
            AssemblyVersion => VersionType.Assembly,
            _ => VersionType.Custom
        };
        writer.Write((byte)type);
        WriteInfo(writer);
    }

    public abstract string ToSimpleName();
}

public enum VersionType: byte
{
    None,
    Git,
    Assembly,
    Custom
}