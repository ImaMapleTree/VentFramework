using System.Collections.Generic;
using VentLib.Logging.Appenders;
using VentLib.Options;
using VentLib.Options.IO;
using static VentLib.Logging.LogLevel;

namespace VentLib.Logging.Default;

public class DefaultLogConfig
{
    public List<ILogAccumulator> DefaultAccumulators = new(NoDepLogger.UninitAccumulators);
    
    public LogLevel ConsoleLevel;
    
    public bool SupplyCallerInfo;

    public FileLogConfig FileConfig;

    public DefaultLogConfig()
    {
        OptionManager manager = OptionManager.GetManager(file: "logging.txt");
        ConsoleLevel = new OptionBuilder()
            .Name("Console Level")
            .Description("The minimum level for logs written to the console.\nValues = [ALL, TRACE, DEBUG, INFO, WARN, ERROR, FATAL]")
            .Values(3, All, Trace, Debug, Info, Warn, Error, Fatal)
            .BuildAndRegister(manager)
            .GetValue<LogLevel>();

        SupplyCallerInfo = new OptionBuilder()
            .Name("Supply Runtime Caller Info")
            .Description("Whether the invoking method and assembly should be retrieved by the logger.\nThis is option very costly. (Default=OFF)")
            .Values(1, true, false)
            .BuildAndRegister(manager)
            .GetValue<bool>();

        FileConfig = new FileLogConfig(manager);
    }
    
    public class FileLogConfig
    {
        private const string DefaultLogPattern = "yyyy-mm-dd-##.txt";
        public bool Enabled;
        
        public LogLevel FileLevel;

        public string LogDirectory;

        public string LogPattern;

        public ILogAppender CreateAppender()
        {
            return new FlushingMemoryAppender(LogDirectory, LogPattern, FileLevel);
        }

        public FileLogConfig(OptionManager manager)
        {
            var fileLogOptions = new OptionBuilder()
                .Name("Enable File Logging")
                .Description("Whether logs should be written to a file.")
                .Values(0, true, false)
                .SubOption(sub => sub
                    .Name("File Level")
                    .Description("The minimum level for logs written to a file.\nValues = [ALL, TRACE, DEBUG, INFO, WARN, ERROR, FATAL]")
                    .Values(1, All, Trace, Debug, Info, Warn, Error, Fatal)
                    .Build())
                .SubOption(sub => sub
                    .Name("Log Directory")
                    .Description("Directory for storing log files.")
                    .Value("logs")
                    .IOSettings(settings => settings.UnknownValueAction = ADEAnswer.Allow)
                    .Build())
                .SubOption(sub => sub
                    .Name("Log Pattern")
                    .Description("The pattern used for naming log files.")
                    .Value(DefaultLogPattern)
                    .IOSettings(settings => settings.UnknownValueAction = ADEAnswer.Allow)
                    .Build())
                .BuildAndRegister(manager);

            Enabled = fileLogOptions.GetValue<bool>();
            FileLevel = fileLogOptions.Children[0].GetValue<LogLevel>();
            LogDirectory = fileLogOptions.Children[1].GetValue<string>();
            LogPattern = fileLogOptions.Children[2].GetValue<string>();
        }
    }
}
