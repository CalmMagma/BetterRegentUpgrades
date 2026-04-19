using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BetterRegentUpgrades.BetterRegentUpgradesCode.Patches;

[HarmonyPatch(typeof(Conqueror), "OnUpgrade")]
public static class ConquerorUpgrade 
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance) {
        __instance.AddKeyword(CardKeyword.Retain);
        return false;
    }
}

[HarmonyPatch(typeof(SeekingEdge), "OnUpgrade")]
public static class SeekingEdgeUpgrade
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance) {
        
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}