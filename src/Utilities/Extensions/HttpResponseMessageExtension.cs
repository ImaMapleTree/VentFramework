using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace VentLib.Utilities.Extensions;

public static class HttpResponseMessageExtension
{
    public static T Deserialize<T>(this HttpResponseMessage message)
    {
        StreamReader reader = new(message.Content.ReadAsStream());
        string result = reader.ReadToEnd();
        reader.Close();
        return JsonSerializer.Deserialize<T>(result)!;
    }
}