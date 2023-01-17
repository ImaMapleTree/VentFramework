namespace VentLib.Version.Git;

public interface IGitVersionEmitter: IVersionEmitter
{
    Version IVersionEmitter.Version() => Version();

    new GitVersion Version();
}