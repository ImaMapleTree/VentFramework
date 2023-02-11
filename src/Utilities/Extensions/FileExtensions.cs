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

    public static FileInfo GetFile(this DirectoryInfo directory, string path)
    {
        return new FileInfo(Path.Join(directory.FullName, path));
    }
}