using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using VentLib.Options;
using VentLib.Utilities;

namespace VentLib.Logging;

public static class VentLogger
{
    public static StreamWriter? OutputStream;
    private static readonly bool CreateNewLog;

    static VentLogger()
    {
        var wfOpt = new OptionBuilder().Name("Write To File").Values(0, true, false).BuildAndRegister(isRendered: false);
        var cnlOpt = new OptionBuilder().Name("Create New Log").Values(0, true, false).BuildAndRegister(isRendered: false);
        wfOpt.Save();
        cnlOpt.Save();
        var writeToFile = (bool)wfOpt.GetValue();
        CreateNewLog = (bool)cnlOpt.GetValue();
        wfOpt.Delete();
        cnlOpt.Delete();
        if (!writeToFile) return;
        DirectoryInfo logDirectory = new DirectoryInfo("logs/");
        if (!logDirectory.Exists) logDirectory.Create();
        string fullFilename = CreateFullFilename(logDirectory);
        OutputStream = new StreamWriter(File.Open(fullFilename, FileMode.Create));
        OutputStream.AutoFlush = true;
    }

    public static void Trace(string message, string? tag = null) => Log(LogLevel.Trace, message, tag, Assembly.GetCallingAssembly());
    public static void Old(string message, string? tag = null) => Log(LogLevel.Old, message, tag, Assembly.GetCallingAssembly());
    public static void Debug(string message, string? tag = null) => Log(LogLevel.Debug, message, tag, Assembly.GetCallingAssembly());
    public static void Info(string message, string? tag = null) => Log(LogLevel.Info, message, tag, Assembly.GetCallingAssembly());
    public static void Warn(string message, string? tag = null) => Log(LogLevel.Warn, message, tag, Assembly.GetCallingAssembly());
    public static void Error(string message, string? tag = null) => Log(LogLevel.Error, message, tag, Assembly.GetCallingAssembly());
    public static void Exception(Exception exception, string? message = "", string? tag = null) => Log(LogLevel.Error, message + exception, tag, Assembly.GetCallingAssembly());
    public static void Fatal(string message, string? tag = null) => Log(LogLevel.Fatal, message, tag, Assembly.GetCallingAssembly());

    public static void SendInGame(string message)
    {
        Debug($"Sending In Game: {message}");
        if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(message);
    }

    public static void Log(LogLevel level, string message, string? tag = null, Assembly? source = null)
    {
        if (level.Level < Configuration.AllowedLevel.Level) return;
        ConsoleManager.SetConsoleColor(level.Color);
        
        source ??= Assembly.GetCallingAssembly();
        string sourcePrefix = !Configuration.ShowSourceName ? "" : ":" + Vents.AssemblyNames!.GetValueOrDefault(source, "Unknown");

        string levelPrefix = level.Name.PadRight(LogLevel.LongestName);
        string tagPrefix = tag == null ? "" : $"[{tag}]";
        
        string fullMessage = $"[{levelPrefix}{sourcePrefix}][{DateTime.Now:hh:mm:ss}]{tagPrefix} {message}";
        
        if (Configuration.Output is LogOutput.StandardOut)
            ConsoleManager.StandardOutStream?.WriteLine(fullMessage);
        else
            ConsoleManager.ConsoleStream?.WriteLine(fullMessage);
        
        OutputStream?.WriteLine(fullMessage);
        ConsoleManager.SetConsoleColor(Configuration.DefaultColor);
    }

    private static string CreateFullFilename(DirectoryInfo logDirectory)
    {
        if (!CreateNewLog) return Path.Join(logDirectory.FullName, "latest.txt");
        string dateFilename = DateTime.Now.ToString("yyyy-MM-dd");
        FileInfo[] allLogs = logDirectory.GetFiles();
        int similarNames = 1;
        while (allLogs.Any(f => f.Name == $"{dateFilename}-{similarNames}.txt")) similarNames++;
        return Path.Join(logDirectory.FullName, $"{dateFilename}-{similarNames}.txt");
    }
    
    public static class Configuration
    {
        public static LogLevel AllowedLevel { get; private set; } = LogLevel.All;
        public static ConsoleColor DefaultColor { get; private set; } = ConsoleColor.DarkGray;
        public static LogOutput Output { get; private set; } = LogOutput.ConsoleOut;
        public static bool ShowSourceName { get; private set; } = true;

        public static void SetAssemblyRefName(Assembly assembly, string name)
        {
            Vents.AssemblyNames[assembly] = name;
        }
        
        public static void SetLevel(LogLevel level)
        {
            AllowedLevel = level;
        }

        public static void SetDefaultColor(ConsoleColor color)
        {
            DefaultColor = color;
        }

        public static void SetOutput(LogOutput output)
        {
            Output = output;
        }

        public static void ShowSource(bool showSource)
        {
            ShowSourceName = showSource;
        }
    }
}