using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VentLib.Logging;
using VentLib.Options.Game.Events;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Utilities;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;

namespace VentLib.Options;

public class OptionManager
{
    public static string OptionPath { get; } = "BepInEx/config/";
    public static string DefaultFile = "options.txt";
    internal static Dictionary<Assembly, List<OptionManager>> Managers = new();
    internal static Dictionary<String, Option> AllOptions = new();

    private readonly OrderedDictionary<string, Option> options = new();
    private FileInfo file;
    private OptionReader optionReader;
    private OrderedSet<Action<IOptionEvent>> optionEventHandlers = new();
    private bool saving;
    private string filePath;

    internal OptionManager(Assembly assembly, string path)
    {
        string name = AssemblyUtils.GetAssemblyRefName(assembly);
        string optionPath = name == "root" ? OptionPath : Path.Join(OptionPath, name);
        DirectoryInfo optionDirectory = new(optionPath);
        if (!optionDirectory.Exists) optionDirectory.Create();
        filePath = path;
        file = optionDirectory.GetFile(path);
        optionReader = new OptionReader(file.ReadText(true));
        optionReader.ReadToEnd();
    }

    public static OptionManager GetManager(Assembly? assembly = null, string? file = null)
    {
        file ??= DefaultFile;
        assembly ??= Assembly.GetCallingAssembly();
        List<OptionManager> managers = Managers.GetOrCompute(assembly, () => new List<OptionManager>());
        OptionManager? manager = managers.FirstOrDefault(m => m.filePath == file);
        if (manager != null) return manager;
        manager = new OptionManager(assembly, file);
        managers.Add(manager);
        return manager;
    }

    public static List<OptionManager> GetAllManagers(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Managers.GetOrCompute(assembly, () => new List<OptionManager>());
    }

    public Option? GetOption(string qualifier)
    {
        return GetOptions().FirstOrDefault(opt => opt.Qualifier() == qualifier);
    }

    public List<Option> GetOptions() => options.GetValues().ToList();

    public void Register(Option option)
    {
        AllOptions[option.Qualifier()] = option;
        
        option.RegisterEventHandler(ev =>
        {
            if (ev is OptionValueIncrementEvent or OptionValueDecrementEvent) DelaySave();
        });

        if (!option.HasParent())
            options[option.Qualifier()] = option;
        option.Manager = this;
        option.Load(false);
        option.RegisterEventHandler(ChangeCallback);
        OptionHelpers.GetChildren(option).ForEach(Register);
    }
    
    public void RegisterEventHandler(Action<IOptionEvent> eventHandler) => optionEventHandlers.Add(eventHandler);

    internal void Load(Option option, bool create = false)
    {
        try
        {
            optionReader.Update(option);
        }
        catch (ArgumentNullException)
        {
            if (!create) return;
            OptionWriter writer = new(file.OpenWriter());
            writer.Write(option, addNewLine: true);
            writer.Close();
        }
    }

    internal void SaveAll()
    {
        VentLogger.Trace($"Saving Options to \"{filePath}\"", "OptionSave");
        OptionWriter writer = new(file.OpenWriter(create: true, fileMode: FileMode.Create));
        writer.WriteAll(GetOptions());
        writer.Close();
        VentLogger.Trace("Saved Options", "OptionSave");
    }

    private void DelaySave()
    {
        if (saving) return;
        saving = true;
        Async.ScheduleThreaded(() =>
        {
            SaveAll();
            saving = false;
        }, 10f);
    }
    
    private void ChangeCallback(IOptionEvent optionEvent)
    {
        optionEventHandlers.ForEach(eh => eh(optionEvent));
    }
}

[Flags]
public enum OptionSortType
{
    MainOption = 1,
    SubOption = 2,
    All = 3
}

public enum OptionLoadMode
{
    None,
    Load,
    LoadOrCreate
}