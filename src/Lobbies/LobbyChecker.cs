using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VentLib.Lobbies.Patches;
using VentLib.Logging;
using VentLib.Utilities;
using VentLib.Version;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VentLib.Lobbies;

public class LobbyChecker
{
    private static string LobbyEndpoint = "http://18.219.112.36:8080/lobbies";
    private static readonly HttpClient Client = new();
    private static Dictionary<int, ModdedLobby> _moddedLobbies = new();
    private static Regex specialCharacterRegex = new("[^A-Za-z-]*");

    internal static void POSTModdedLobby(string gameId, string host)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage();
        requestMessage.RequestUri = new Uri(LobbyEndpoint);
        requestMessage.Method = HttpMethod.Post;
        requestMessage.Headers.Add("game-id", gameId);
        Version.Version version = VersionControl.Instance.Version ?? new NoVersion();
        requestMessage.Headers.Add("version", version.ToSimpleName());
        requestMessage.Headers.Add("mod-name", Vents.AssemblyNames[Vents.RootAssemby]);
        requestMessage.Headers.Add("host", specialCharacterRegex.Replace(host.Replace(" ", "-"), ""));
        Client.SendAsync(requestMessage);
    }

    internal static void GETModdedLobbies()
    {
        Task<HttpResponseMessage> response = Client.GetAsync(LobbyEndpoint);
        SyncTaskWaiter waiter = new(response);
        Async.Schedule(() => WaitForResponse(waiter, 0), 0.25f);

    }

    private static void WaitForResponse(SyncTaskWaiter response, int times)
    {
        if (times > 20) VentLogger.Fatal("Failed to get modded lobbies");
        else if (!response.Finished)
            Async.Schedule(() => WaitForResponse(response, times + 1), 1f);
        else HandleResponse(response.Response);
    }

    internal static void HandleResponse(Task<HttpResponseMessage>? response)
    {
        if (response != null)
        {
            string result = new StreamReader(response.Result.Content.ReadAsStream()).ReadToEnd();
            VentLogger.Log(LogLevel.Fatal, $"Response from lobby server: {result}", "ModdedLobbyCheck");
            _moddedLobbies = JsonSerializer.Deserialize<Dictionary<int, ModdedLobby>>(result)!;
        }
        
        
        foreach (var kv in _moddedLobbies)
        {
            VentLogger.Fatal($"{kv.Key}: {kv.Value}");
        }
        
        LobbyListingsPatch.ModdedGames.ForEach(game =>
        {
            var button = game.Item2!;
            if (!_moddedLobbies.TryGetValue(game.Item1, out ModdedLobby? lobby))
            {
                button.LanguageText.text = "Vanilla";
                return;
            }

            button.LanguageText.text = lobby.Mod;
            button.NameText.text = $"{lobby.Host}'s Lobby";
        });
    }
}

internal class SyncTaskWaiter
{
    internal Task<HttpResponseMessage> Response;
    internal bool Finished;

    public SyncTaskWaiter(Task<HttpResponseMessage> response)
    {
        Response = response;
        Response.ContinueWith(_ => Finished = true);
    }
}