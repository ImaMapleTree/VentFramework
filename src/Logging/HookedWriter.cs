using System.IO;
using System.Text;

namespace VentLib.Logging;

public class HookedWriter : StreamWriter
{
    public StreamWriter TextWriter;

    public HookedWriter(FileStream fileStream) : base(fileStream)
    {
        TextWriter = new StreamWriter(System.Console.OpenStandardOutput());
        TextWriter.AutoFlush = true;
        System.Console.SetOut(TextWriter);
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

    public override Encoding Encoding => TextWriter.Encoding;
}