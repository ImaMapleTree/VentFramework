using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using VentLib.Options;
using VentLib.Options.IO;
using VentLib.Options.Processors;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Version;

namespace VentLib.Logging;

public static class VentLogger
{
    public static StreamWriter? OutputStream;
    private static readonly string? LogFile;
    private static readonly DirectoryInfo? LogDirectory;
    private static readonly bool CreateNewLog;
    private static int _logLevel;
    private static int _fileLevel;

    static VentLogger()
    {
        OptionManager loggerManager = OptionManager.GetManager(file: "logger_options.txt");
        ValueTypeProcessors.AddTypeProcessor(new LogLevelValueTypeProcessor());
        var writeToFileOption = new OptionBuilder().Name("Write To File")
            .Description("If the logger should write to a file.\nDefault = true")
            .Values(0, true, false)
            .BuildAndRegister(loggerManager);
        
        var createNewLogOptions = new OptionBuilder().Name("Create New Log")
            .Description("If the logger should create a new log for every run. OR, if false, override a common log.\nDefault = true")
            .Values(0, true, false)
            .BuildAndRegister(loggerManager);

        var consoleLogLevel = new OptionBuilder().Name("Console Log Level")
            .Description("Minimum level for logs to display in the Console.\nValues = [ALL, TRACE, DEBUG, INFO, WARN, ERROR, FATAL]")
            .Values(3, LogLevel.All, LogLevel.Trace, LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal)
            .BuildAndRegister(loggerManager);

        var fileLogLevel = new OptionBuilder().Name("File Log Level")
            .Description("Minimum level for logs to display in the log file. Cannot be higher than Console level.\nValues = [SAME, ALL, TRACE, DEBUG, INFO, WARN, ERROR, FATAL]")
            .Values(0, LogLevel.All, LogLevel.Trace, LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal)
            .IOSettings(settings => settings.UnknownValueAction = ADEAnswer.Allow)
            .BuildAndRegister(loggerManager);

        var logDirectoryOption = new OptionBuilder().Name("Log Directory")
            .Description("Directory for storing log files.")
            .Value("logs")
            .IOSettings(settings => settings.UnknownValueAction = ADEAnswer.Allow)
            .BuildAndRegister(loggerManager);
        
        var writeToFile = writeToFileOption.GetValue<bool>();
        CreateNewLog = createNewLogOptions.GetValue<bool>();
        _logLevel = consoleLogLevel.GetValue<LogLevel>().Level;
        LogLevel level = fileLogLevel.GetValue<LogLevel>();
        _fileLevel = level.Name == "SAME" ? _logLevel : level.Level;
        
        if (!writeToFile) return;
        LogDirectory = new DirectoryInfo(logDirectoryOption.GetValue<string>());
        if (!LogDirectory.Exists) LogDirectory.Create();
        LogFile = CreateFullFilename(LogDirectory);
        ConfigureOutputStream(LogFile);
    }

    public static void Trace(string message, string? tag = null) => Log(LogLevel.Trace, message, tag, Assembly.GetCallingAssembly());
    public static void Old(string message, string? tag = null) => Log(LogLevel.Old, message, tag, Assembly.GetCallingAssembly());
    public static void Debug(string message, string? tag = null) => Log(LogLevel.Debug, message, tag, Assembly.GetCallingAssembly());
    public static void Info(string message, string? tag = null) => Log(LogLevel.Info, message, tag, Assembly.GetCallingAssembly());
    public static void High(string message, string? tag = null) => Log(LogLevel.High, message, tag, Assembly.GetCallingAssembly());
    public static void Warn(string message, string? tag = null) => Log(LogLevel.Warn, message, tag, Assembly.GetCallingAssembly());
    public static void Error(string message, string? tag = null) => Log(LogLevel.Error, message, tag, Assembly.GetCallingAssembly());
    public static void Exception(Exception exception, string? message = "", string? tag = null) => Log(LogLevel.Error, $"{message} Exception={exception}", tag, Assembly.GetCallingAssembly());
    public static void Fatal(string message, string? tag = null) => Log(LogLevel.Fatal, message, tag, Assembly.GetCallingAssembly());

    public static void SendInGame(string message)
    {
        Debug($"Sending In Game: {message}");
        if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(message);
    }

    public static void Log(LogLevel level, string message, string? tag = null, Assembly? source = null)
    {
        
        if (level.Level < _logLevel && level.Level < _fileLevel) return;

        

        source ??= Assembly.GetCallingAssembly();
        string sourcePrefix = "";
        if (Configuration.ShowSourceName)
        {
            int longestSource = Vents.AssemblyNames.Values.Select(s => s.Length).Sorted(l => l).LastOrDefault(0);
            sourcePrefix = ":" + Vents.AssemblyNames.GetValueOrDefault(source, "Unknown").PadLeft(longestSource);
        }
        
        string levelPrefix = level.Name.PadRight(LogLevel.LongestName);
        string tagPrefix = tag == null ? "" : $"[{tag}]";
        
        string fullMessage = $"[{levelPrefix}{sourcePrefix}][{DateTime.Now:hh:mm:ss}]{tagPrefix} {message}";

        if (level.Level < _logLevel && level.Level >= _fileLevel && OutputStream is HookedWriter hookedWriter)
        {
            hookedWriter.WriteLineToFile(fullMessage);
            return;
        }

        ConsoleManager.SetConsoleColor(level.Color);
        
        if (Configuration.Output is LogOutput.StandardOut)
            ConsoleManager.StandardOutStream?.WriteLine(fullMessage);
        else
            ConsoleManager.ConsoleStream?.WriteLine(fullMessage);
        
        ConsoleManager.SetConsoleColor(Configuration.DefaultColor);
    }

    public static string? Dump()
    {
        OutputStream?.Close();
        if (LogFile == null) return LogFile;
        
        File.Copy(LogFile, LogDirectory!.GetFile("dump.log").FullName, true);
        ConfigureOutputStream(LogFile, FileMode.Append);
        return LogFile;
    }

    private static string CreateFullFilename(DirectoryInfo logDirectory)
    {
        if (!CreateNewLog) return Path.Join(logDirectory.FullName, "latest.log");
        string dateFilename = DateTime.Now.ToString("yy-MM-dd");
        FileInfo[] allLogs = logDirectory.GetFiles();
        int similarNames = 1;
        while (allLogs.Any(f => f.Name == $"{dateFilename}-{similarNames}.log")) similarNames++;
        return Path.Join(logDirectory.FullName, $"{dateFilename}-{similarNames}.log");
    }

    private static void ConfigureOutputStream(string path, FileMode mode = FileMode.Create)
    {
        try
        {
            HookedWriter writer = new HookedWriter(File.Open(path, mode));
            writer.SetFilePath(LogFile);
            OutputStream = writer;
            OutputStream.AutoFlush = true;
        }
        catch
        {
            OutputStream = new StreamWriter(System.Console.OpenStandardOutput());
            OutputStream.AutoFlush = true;
        }

        object driver = typeof(ConsoleManager).GetProperty("Driver", AccessFlags.StaticAccessFlags)!.GetValue(null)!;
        driver.GetType().GetProperty("StandardOut", AccessFlags.InstanceAccessFlags)!.SetValue(driver, OutputStream, AccessFlags.InstanceAccessFlags, null, null, null);
        driver.GetType().GetProperty("ConsoleOut", AccessFlags.InstanceAccessFlags)!.SetValue(driver, OutputStream, AccessFlags.InstanceAccessFlags, null, null, null);
    }
    
    public static class Configuration
    {
        public static ConsoleColor DefaultColor { get; private set; } = ConsoleColor.DarkGray;
        public static LogOutput Output { get; private set; } = LogOutput.ConsoleOut;
        public static bool ShowSourceName { get; private set; } = true;

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