using System.IO;

namespace VentLib.Utilities.Extensions;

public static class FileExtensions
{
    public static string ReadAll(this FileInfo file, bool create = false)
    {
        if (file.Exists || !create)
            return File.ReadAllText(file.FullName);
        
        file.Create().Close();
        return "";
    }

    public static StreamReader ReadText(this FileInfo file, bool create = false)
    {
        if (file.Exists || !create)
            return file.OpenText();
        file.Create().Close();
        return new StreamReader(file.FullName);
    }

    public static StreamWriter OpenWriter(this FileInfo file, bool create = false, FileMode fileMode = FileMode.Append)
    {
        if (file.Exists || !create)
            return new StreamWriter(file.Open(fileMode));
        return new StreamWriter(file.Create());
    }

    public static FileInfo GetFile(this DirectoryInfo directory, string path)
    {
        return new FileInfo(Path.Join(directory.FullName, path));
    }
    
    public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string path)
    {
        return new DirectoryInfo(Path.Join(directory.FullName, path));
    }
}