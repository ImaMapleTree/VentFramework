using System;
using Hazel;

namespace VentLib.Version.Git;

public class GitVersion: Version
{
    public readonly string MajorVersion = ThisAssembly.Git.BaseVersion.Major;
    public readonly string MinorVersion = ThisAssembly.Git.BaseVersion.Minor;
    public readonly string PatchNumber = ThisAssembly.Git.BaseVersion.Patch;

    public readonly string CommitNumber = ThisAssembly.Git.Commit;
    public readonly string Branch = ThisAssembly.Git.Branch;

    public readonly string Sha = ThisAssembly.Git.Sha;
    public readonly string Tag = ThisAssembly.Git.Tag;

    public GitVersion() { }

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

    public override string ToString()
    {
        return $"GitVersion({MajorVersion}.{MinorVersion}.{PatchNumber} Branch: {Branch} Commit: {CommitNumber})";
    }
}