using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VentLib.Logging;
using VentLib.Options.Meta;
using VentLib.Utilities;

namespace VentLib.Options;

public class OptionManager
{
    public static string OptionPath { get; } = "BepInEx/config/";
    internal static Dictionary<Assembly, OptionManager> Managers = new();

    internal static HashSet<Option> NewRegisters = new();
    public static List<Option> AllOptions = new();
    private Metadata metadata;
    private Assembly assembly;
    internal readonly List<Option> Options = new();

    internal OptionManager(Assembly assembly)
    {
        this.assembly = assembly;
        string name = AssemblyUtils.GetAssemblyRefName(assembly);
        string optionPath = name == "root" ? OptionPath : Path.Join(OptionPath, name);
        DirectoryInfo optionDirectory = new DirectoryInfo(optionPath);
        if (!optionDirectory.Exists) optionDirectory.Create();
        metadata = Metadata.Parse(new FileInfo(Path.Join(optionPath, "options.txt")));
    }

    internal void Save()
    {
        metadata.DumpAll(Options);
    }

    public static OptionManager GetManager(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Managers[assembly];
    }
    
    public static void Load(Assembly? assembly)
    {
        VentLogger.Trace($"Loading Option Manager for: {assembly}");
        assembly ??= Vents.RootAssemby;
        Managers[assembly] = new OptionManager(assembly);
    }

    public void LoadAndAdd(Option option, bool suboption = false, bool isRendered = true)
    {
        Option.OptionStub? stub = metadata.GetStub(option.Qualifier());
        option.LoadOrCreate(stub);
        AllOptions.Add(option);
        if (suboption) return;
        Options.Add(option);
        if (!isRendered) return;
        NewRegisters.Add(option);
    }
}