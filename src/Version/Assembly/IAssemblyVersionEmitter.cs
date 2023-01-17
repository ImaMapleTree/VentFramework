namespace VentLib.Version.Assembly;

public interface IAssemblyVersionEmitter: IVersionEmitter
{
    Version IVersionEmitter.Version() => Version();

    new AssemblyVersion Version();
}