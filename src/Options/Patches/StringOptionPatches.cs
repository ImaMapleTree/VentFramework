using System;
using System.Linq;
using HarmonyLib;

namespace VentLib.Options.Patches;

public class StringOptionPatches
{
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    public class StringOptionEnablePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            var option = OptionManager.AllOptions.FirstOrDefault(opt => opt.Behaviour == __instance);
            if (option == null) return true;
            __instance.OnValueChanged = new Action<OptionBehaviour>(_ => { });
            __instance.TitleText.text = option.ColorName;
            __instance.ValueText.text = option.GetValueAsString();
            return false;
        }
    }
    
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            var option = OptionManager.AllOptions.FirstOrDefault(opt => opt.Behaviour == __instance);
            if (option == null) return true;

            option.Increment();
            __instance.ValueText.text = option.GetValueAsString();

            return false;
        }
    }
    
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            var option = OptionManager.AllOptions.FirstOrDefault(opt => opt.Behaviour == __instance);
            if (option == null) return true;

            option.Decrement();
            __instance.ValueText.text = option.GetValueAsString();

            return false;
        }
    }
}