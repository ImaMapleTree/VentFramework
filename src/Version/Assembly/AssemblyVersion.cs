using System;
using Hazel;
using VentLib.Utilities;
// ReSharper disable MemberCanBePrivate.Global

namespace VentLib.Version.Assembly;

public class AssemblyVersion: Version
{
    public readonly System.Reflection.Assembly? Assembly;
    
    public readonly int MajorVersion;
    public readonly int MinorVersion;
    public readonly int Build;

    public readonly int Revision;
    public readonly short MajorRevision;
    public readonly short MinorRevision;

    public AssemblyVersion(System.Reflection.Assembly assembly)
    {
        Assembly = assembly;
        System.Version? v = assembly.GetName().Version;
        if (v == null) return;
        MajorVersion = v.Major;
        MinorVersion = v.Minor;
        Build = v.Build;
        Revision = v.Revision;
        MajorRevision = v.MajorRevision;
        MinorRevision = v.MinorRevision;
    }
    
    public AssemblyVersion(): this(System.Reflection.Assembly.GetCallingAssembly()) { }
    
    private AssemblyVersion(MessageReader reader)
    {
        Assembly = AssemblyUtils.FindAssemblyFromFullName(reader.ReadString());
        MajorVersion = reader.ReadInt32();
        MinorVersion = reader.ReadInt32();
        Build = reader.ReadInt32();
        Revision = reader.ReadInt32();
        MajorRevision = reader.ReadInt16();
        MinorRevision = reader.ReadInt16();
    }

    public override Version Read(MessageReader reader) => new AssemblyVersion(reader);

    protected override void WriteInfo(MessageWriter writer)
    {
        writer.Write(Assembly?.GetName().FullName ?? "NULL");
        writer.Write(MajorVersion);
        writer.Write(MinorVersion);
        writer.Write(Build);
        writer.Write(Revision);
        writer.Write(MajorRevision);
        writer.Write(MinorRevision);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not AssemblyVersion other) return false;
        return Equals(Assembly, other.Assembly) && MajorVersion == other.MajorVersion && MinorVersion == other.MinorVersion && Build == other.Build && Revision == other.Revision && MajorRevision == other.MajorRevision && MinorRevision == other.MinorRevision;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Assembly, MajorVersion, MinorVersion, Build, Revision, MajorRevision, MinorRevision);
    }

    public override string ToSimpleName() => $"{MajorRevision}.{MinorVersion}.{Build}";

    public override string ToString()
    {
        return $"AssemblyVersion({MajorVersion}.{MinorRevision}.{Build}.{Revision}. {MajorRevision}/{MinorRevision})";
    }
}