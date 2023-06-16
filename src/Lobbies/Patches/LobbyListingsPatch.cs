using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using InnerNet;
using UnityEngine;
using VentLib.Logging;
using VentLib.Utilities.Extensions;
using PButton = PassiveButton;

namespace VentLib.Lobbies.Patches;

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.HandleList))]
internal class LobbyListingsPatch
{
    internal static List<(int, MatchMakerGameButton?)> ModdedGames = new();

    internal static void Prefix(FindAGameManager __instance)
    {
        LobbyChecker.GETModdedLobbies();
    }
    
    internal static void Postfix(FindAGameManager __instance, [HarmonyArgument(0)] InnerNetClient.TotalGameData totalGames, [HarmonyArgument(1)] Il2CppSystem.Collections.Generic.List<GameListing> availableGames)
    {
        ModdedGames.Clear();
        availableGames.ToArray().Do(game =>
        {
            VentLogger.Debug($"Game: {game.HostName} | {game.GameId} | {game.IPString}");
            var button = __instance.buttonPool.activeChildren
                .ToArray()
                .ToList()
                .SelectWhere(b => b.TryCast<MatchMakerGameButton>())
                .First(b => b.NameText.text == game.HostName);
            ModdedGames.Add((game.GameId, button));
        });
        LobbyChecker.HandleResponse(null);
    }
}

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.Start))]
internal class LobbyListingUIPatch
{
    private static bool _option = true;

    internal static void Postfix(FindAGameManager __instance)
    {
        if (_option) {
            CustomFinderMenu(__instance);
            return;
        }
        
        var areaTransform = __instance.TargetArea.transform;
        var refreshTransform = __instance.RefreshSpinner.transform;
        Vector3 refreshScale = refreshTransform.localScale;
        
        areaTransform.localScale += new Vector3(0.4f, 0, 0);
        areaTransform.localPosition += new Vector3(0.2f, 0, 0);
        
        refreshTransform.localScale = refreshScale - new Vector3(0.25f, 0.25f, 0);
        refreshTransform.localPosition += new Vector3(-1f, 0.7f, 0);
    }

    private static void CustomFinderMenu(FindAGameManager manager)
    {
        List<PassiveButton> buttons = manager.ControllerSelectable.ToArray()
            .SelectWhere(b => b.TryCast<PassiveButton>()).ToList();

        PButton mapButton = buttons[0];
        PButton any = buttons[1]; 
        PButton one = buttons[2]; 
        PButton two = buttons[3];
        PButton three = buttons[4];
        PButton langButton = buttons[5];
        PButton gameTypeButton = buttons[9];
        PButton filterButton = buttons[11];
        
        
        var textComponents = ControllerManager.Instance.GetMenu(manager.name).SelectableUiElements
            .ToArray()
            .SelectMany(a => a.GetComponentsInParent<Component>())
            .Where(comp => comp.TypeName() == "UnityEngine.Transform")
            .Distinct()
            .ToDict(comp => comp.name, comp => comp.Cast<Transform>());

        foreach (var kv in textComponents)
        {
            VentLogger.Fatal($"Name: {kv.Key} | Value: {kv.Value.TypeName()}");
        }

        var c = textComponents["FilterTags"].GetComponentsInChildren<Component>().Select(c => (c.name, c.TypeName())).StrJoin();
        VentLogger.Fatal($"C: {c}");
        // Moving map to left side
        textComponents["MapPicker"].FindChild("Title_TMP").localPosition -= new Vector3(4.7f, 0);
        mapButton.transform.localPosition -= new Vector3(6.5f, 0.48f);

        // Moving game type to left side
        textComponents["Game Mode"].FindChild("Title_TMP").localPosition -= new Vector3(0.9f, 0.38f);
        gameTypeButton.transform.localPosition -= new Vector3(2.7f, 0.83f);
        
        // Moving filters to left side
        textComponents["FilterTags"].FindChild("Title_TMP").localPosition -= new Vector3(1.25f, 1.7f);
        textComponents["FilterTags"].GetComponentsInChildren<Component>().Where(c1 => c1.name == "HelpButton").Do(c2 => c2.gameObject.SetActive(false));
        filterButton.transform.localPosition -= new Vector3(2.65f, 2.15f);
        
        // Moving impostor count
        textComponents["Impostors"].localPosition -= new Vector3(4.65f, 2.02f);
        const float impNumX = 1.85f; const float impNumY = 0.34f;
        any.transform.localPosition -= new Vector3(impNumX, impNumY);
        one.transform.localPosition -= new Vector3(impNumX, impNumY);
        two.transform.localPosition -= new Vector3(impNumX, impNumY);
        three.transform.localPosition -= new Vector3(impNumX, impNumY);

        // Moving chat type
        textComponents["ChatTypeOptionsCompact"].localPosition -= new Vector3(2.425f, 2.3f);

        // Moving language settings
        const float langX = 5.25f; const float langY = 2.93f;
        textComponents["Language Selection"].FindChild("Title_TMP").localPosition -= new Vector3(langX, langY);
        langButton.transform.localPosition -= new Vector3(langX, langY);

        var areaTransform = manager.TargetArea.transform;
        var refreshTransform = manager.RefreshSpinner.transform;
        Vector3 refreshScale = refreshTransform.localScale;
        
        areaTransform.localScale += new Vector3(0, 0.4f);
        areaTransform.localPosition += new Vector3(1.25f, 0.6f, 0);
        
        refreshTransform.localScale = refreshScale - new Vector3(3.5f, 4.1f, 0);
        refreshTransform.localPosition += new Vector3(-9f, -3, 0);
    }
}
