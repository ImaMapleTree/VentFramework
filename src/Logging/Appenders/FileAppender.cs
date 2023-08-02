using System.Data;
using System.IO;
using System.Text;

namespace VentLib.Logging.Appenders;

public class FileAppender: ILogAppender
{
    private DirectoryInfo TargetDirectory { get; }
    public string FileNamePattern;
    public LogLevel MinLevel { get; set; }
    public FileStream? FileStream { get; private set; }
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    
    public FileAppender(string directoryPath, string filenamePattern, LogLevel? minLevel = null)
    {
        FileNamePattern = filenamePattern;
        this.TargetDirectory = new DirectoryInfo(directoryPath);
        if (!TargetDirectory.Exists) throw new ConstraintException($"Logging directory \"{directoryPath}\" does not exist!");
        this.MinLevel = minLevel ?? LogLevel.Info;
    }
    
    public FileInfo CreateNewStream()
    {
        FileInfo logFile = LogDirectory.CreateLog(FileNamePattern, TargetDirectory);
        FileStream = logFile.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        return logFile;
    }

    public void CloseStream()
    {
        FileStream?.Close();
    }

    public virtual void Receive(LogComposite composite, LogArguments arguments)
    {
        if (composite.Level.Level < MinLevel.Level) return;
        string compositeText = composite.ToString()!;
        FileStream?.Write(Encoding.GetBytes(compositeText));
    }
}