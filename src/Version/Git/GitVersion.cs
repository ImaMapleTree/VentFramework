using System;
using System.Linq;
using Hazel;
using RlAssembly = System.Reflection.Assembly;

namespace VentLib.Version.Git;

public class GitVersion: Version
{
    public readonly string MajorVersion;
    public readonly string MinorVersion;
    public readonly string PatchNumber;

    public readonly string CommitNumber;
    public readonly string Branch;

    public readonly string Sha;
    public readonly string Tag;

    private static Type? _thisAssembly;

    internal GitVersion()
    {
        MajorVersion = null!;
        MinorVersion = null!;
        PatchNumber = null!;
        CommitNumber = null!;
        Branch = null!;
        Sha = null!;
        Tag = null!;
    }
    
    public GitVersion(RlAssembly? targetAssembly = null)
    {
        targetAssembly ??= Vents.RootAssemby;
        _thisAssembly ??= AppDomain.CurrentDomain.GetAssemblies()
           .Where(assembly => targetAssembly == null || assembly.FullName == targetAssembly.FullName)
           .SelectMany(assembly =>
           {
               try { return assembly.GetTypes(); }
               catch (Exception) { return Array.Empty<Type>(); }
           })
           .FirstOrDefault(type => type.Name == "ThisAssembly");

        if (_thisAssembly == null)
            throw new Exception("Assemblies relying on GitVersion must include GitInfo as a package dependency.");

        Type git = _thisAssembly.GetNestedType("Git")!;
        Type baseVersion = git.GetNestedType("BaseVersion")!;

        MajorVersion = StaticValue(baseVersion, "Major");
        MinorVersion = StaticValue(baseVersion, "Minor");
        PatchNumber = StaticValue(baseVersion, "Patch");

        CommitNumber = StaticValue(git, "Commit");
        Branch = StaticValue(git, "Branch");

        Sha = StaticValue(git, "Sha");
        Tag = StaticValue(git, "Tag");
    }

    // TODO: Custom Git interface and Mod updater
    private GitVersion(MessageReader reader)
    {
        MajorVersion = reader.ReadString();
        MinorVersion = reader.ReadString();
        PatchNumber = reader.ReadString();

        CommitNumber = reader.ReadString();
        Branch = reader.ReadString();

        Sha = reader.ReadString();
        Tag = reader.ReadString();
    }
    public override Version Read(MessageReader reader) => new GitVersion(reader);

    protected override void WriteInfo(MessageWriter writer)
    {
        writer.Write(MajorVersion);
        writer.Write(MinorVersion);
        writer.Write(PatchNumber);
        writer.Write(CommitNumber);
        writer.Write(Branch);
        writer.Write(Sha);
        writer.Write(Tag);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not GitVersion other) return false;
        return MajorVersion == other.MajorVersion && MinorVersion == other.MinorVersion && PatchNumber == other.PatchNumber && CommitNumber == other.CommitNumber && Branch == other.Branch && Sha == other.Sha && Tag == other.Tag;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MajorVersion, MinorVersion, PatchNumber, CommitNumber, Branch, Sha, Tag);
    }

    public override string ToSimpleName() => $"{MajorVersion}.{MinorVersion}.{PatchNumber}";
    
    public override string ToString()
    {
        return $"GitVersion({MajorVersion}.{MinorVersion}.{PatchNumber} Branch: {Branch} Commit: {CommitNumber})";
    }

    private static string StaticValue(Type type, string fieldName) => (string)type.GetField(fieldName)!.GetValue(null)!;
}