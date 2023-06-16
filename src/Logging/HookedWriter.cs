using System.IO;
using System.Text;
using System.Threading.Tasks;
using VentLib.Utilities.Extensions;

namespace VentLib.Logging;

public class HookedWriter : StreamWriter
{
    public StreamWriter TextWriter;
    private string? filepath;

    public HookedWriter(FileStream fileStream) : base(fileStream)
    {
        TextWriter = new StreamWriter(System.Console.OpenStandardOutput());
        TextWriter.AutoFlush = true;
        System.Console.SetOut(TextWriter);
    }

    internal void SetFilePath(string? path)
    {
        filepath = path;
    }

    public override void WriteLine(string? value)
    {
        TextWriter.WriteLine(value);
        base.WriteLine(value);
    }

    public override void Write(string? value)
    {
        TextWriter.Write(value);
        base.Write(value);
    }

    public void WriteLineToFile(string? value)
    {
        base.WriteLine(value);
    }

    public void WriteToFile(string? value)
    {
        base.Write(value);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (filepath == null) return;
        FileInfo mockFile = new(filepath);
        if (!mockFile.Exists) return;
        File.Copy(mockFile.FullName, mockFile.Directory!.GetFile("latest.log").FullName, true);
    }

    public override ValueTask DisposeAsync()
    { 
        if (filepath == null) return new ValueTask();
        FileInfo mockFile = new(filepath);
        if (!mockFile.Exists) return new ValueTask();
        File.Copy(mockFile.FullName, mockFile.Directory!.GetFile("latest.log").FullName, true);
        return base.DisposeAsync();
    }

    ~HookedWriter()
    {
        Dispose(true);
    }

    public override Encoding Encoding => TextWriter.Encoding;
}