using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BetterRegentUpgrades.BetterRegentUpgradesCode.Patches;

[HarmonyPatch(typeof(Conqueror), "OnUpgrade")]
public static class ConquerorUpgrade 
{
    public static bool Prefix(CardModel __instance) {
        __instance.AddKeyword(CardKeyword.Retain);
        return false;
    }
}