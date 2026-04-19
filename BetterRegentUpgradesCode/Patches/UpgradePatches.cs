using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BetterRegentUpgrades.BetterRegentUpgradesCode.Patches;

[HarmonyPatch]
public static class UpgradePatches 
{
    // -- CONQUEROR --
    [HarmonyPrefix, HarmonyPatch(typeof(Conqueror), "OnUpgrade")]
    static bool ConquerorUpgrade(Conqueror __instance) {
        __instance.AddKeyword(CardKeyword.Retain);
        return false;
    }
    
    // -- SEEKING EDGE --
    [HarmonyPrefix, HarmonyPatch(typeof(SeekingEdge), "OnUpgrade")]
    static bool SeekingEdgeUpgrade(SeekingEdge __instance) {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}
