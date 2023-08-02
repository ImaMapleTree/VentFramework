using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using VentLib.Logging.Default;
using VentLib.Options.Game.Events;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Utilities;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;

namespace VentLib.Options;

public class OptionManager
{
    public static string OptionPath => "BepInEx/config/";
    public static string DefaultFile = "options.txt";
    internal static Dictionary<Assembly, List<OptionManager>> Managers = new();
    internal static Dictionary<String, Option> AllOptions = new();

    private readonly EssFile essFile = new();
    private readonly OrderedDictionary<string, Option> options = new();
    private FileInfo file;
    
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
        essFile.ParseFile(file.FullName);
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

    // ReSharper disable once ReturnTypeCanBeNotNullable
    public Option? GetOption(string qualifier)
    {
        return GetOptions().FirstOrOptional(opt => opt.Qualifier() == qualifier)
            .CoalesceEmpty(() => AllOptions.GetOptional(qualifier))
            .OrElse(null!);
    }

    public List<Option> GetOptions() => options.GetValues().ToList();

    public void Register(Option option, OptionLoadMode loadMode = OptionLoadMode.Load)
    {
        AllOptions[option.Qualifier()] = option;
        
        option.RegisterEventHandler(ev =>
        {
            if (ev is not (OptionValueIncrementEvent or OptionValueDecrementEvent)) return;
            essFile.WriteToCache(ev.Source());
            DelaySave(updateAll: false);
        });

        if (!option.HasParent())
            options[option.Qualifier()] = option;
        option.Manager = this;
        option.Load(loadMode is OptionLoadMode.LoadOrCreate);
        option.RegisterEventHandler(ChangeCallback);
        OptionHelpers.GetChildren(option).ForEach(o => Register(o, loadMode));
    }
    
    public void RegisterEventHandler(Action<IOptionEvent> eventHandler) => optionEventHandlers.Add(eventHandler);

    internal void Load(Option option, bool create = false)
    {
        try
        {
            essFile.ApplyToOption(option);
        }
        catch (DataException)
        {
            string createString = create ? ". Attempting to recreate in file." : ".";
            NoDepLogger.Warn($"Failed to load option ({option.Qualifier()})" + createString);
            if (!create) return;
            essFile.WriteToCache(option);
            DelaySave();
        }
        catch (Exception exception)
        {
            NoDepLogger.Exception($"Error loading option ({option.Qualifier()}).", exception);
        }
    }

    internal void SaveAll(bool updateAll = true)
    {
        NoDepLogger.Trace($"Saving Options to \"{filePath}\"", "OptionSave");
        if (updateAll) GetOptions().ForEach(o => essFile.WriteToCache(o));
        essFile.Dump(file.FullName);
        NoDepLogger.Trace("Saved Options", "OptionSave");
    }

    public void DelaySave(float delay = 10f, bool updateAll = true)
    {
        if (saving) return;
        saving = true;
        Async.ScheduleThreaded(() =>
        {
            SaveAll(updateAll);
            saving = false;
        }, delay);
    }
    
    private void ChangeCallback(IOptionEvent optionEvent)
    {
        optionEventHandlers.ForEach(eh => eh(optionEvent));
    }
}

public enum OptionLoadMode
{
    None,
    Load,
    LoadOrCreate
}