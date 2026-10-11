using System.Collections.Generic;
using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UnityEngine;

namespace MeowMeowFan
{
    public sealed class Mod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            // UserMod2.OnLoad already calls harmony.PatchAll(assembly).
            // Calling PatchAll again here registers every Harmony patch twice.
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(MeowMeowFanOptions));
            Localization.RegisterForTranslation(typeof(STRINGS));
            LocString.CreateLocStringKeys(typeof(STRINGS), null);
        }
    }

    [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    internal static class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        internal static void Prefix()
        {
            // Be idempotent even if another loader/mod invokes this registration path twice.
            foreach (var planInfo in TUNING.BUILDINGS.PLANORDER)
            {
                var entries = planInfo.buildingAndSubcategoryData;
                if (entries == null)
                    continue;

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].Key == SelectiveVentilationFanConfig.ID)
                        return;
                }
            }

            ModUtil.AddBuildingToPlanScreen("HVAC", SelectiveVentilationFanConfig.ID);
        }
    }

    [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.IsValidForTarget))]
    internal static class FilterSideScreen_IsValidForTarget_Patch
    {
        internal static void Postfix(FilterSideScreen __instance, GameObject target, ref bool __result)
        {
            if (__result || target == null)
                return;

            // This mirrors the current Smart Pumps compatibility approach: the selector
            // uses ONI's built-in Filterable side screen instead of a custom UI.
            if (__instance.isLogicFilter &&
                target.GetComponent<SelectiveVentilationFan>() != null &&
                target.GetComponent<Filterable>() != null)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Filterable), nameof(Filterable.GetTagOptions))]
    internal static class Filterable_GetTagOptions_Patch
    {
        internal static void Postfix(Filterable __instance, ref Dictionary<Tag, HashSet<Tag>> __result)
        {
            if (__instance == null || __result == null ||
                __instance.GetComponent<SelectiveVentilationFan>() == null ||
                !MeowMeowFanOptions.Instance.OnlyDiscoveredGases ||
                DiscoveredResources.Instance == null)
            {
                return;
            }

            Tag selected = __instance.SelectedTag;
            foreach (var entry in __result)
            {
                entry.Value.RemoveWhere(tag =>
                    tag != GameTags.Void &&
                    tag != selected &&
                    !DiscoveredResources.Instance.IsDiscovered(tag));
            }
        }
    }
}
