using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Reflection;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;


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
    
    // -- HEIRLOOM HAMMER --
    [HarmonyPrefix, HarmonyPatch(typeof(HeirloomHammer), "OnPlay")]
    static bool HeirloomHammerUpgrade(HeirloomHammer __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay,
        ref Task __result)
    {
        __result = Task.Run(async () =>
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue).FromCard(__instance)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
            CardModel selection = (await CardSelectCmd.FromHand(
                    prefs: new CardSelectorPrefs(__instance.SelectionScreenPrompt, 1), context: choiceContext,
                    player: __instance.Owner, filter: (CardModel c) => c.VisualCardPool.IsColorless,
                    source: __instance))
                .FirstOrDefault();
            if (selection != null)
            {
                for (int i = 0; i < __instance.DynamicVars.Repeat.IntValue; i++)
                {
                    CardModel cardToGive = selection.CreateClone();
                    if (__instance.IsUpgraded)
                    {

                        cardToGive.SetToFreeThisTurn();
                    }

                    await CardPileCmd.AddGeneratedCardToCombat(cardToGive, PileType.Hand, addedByPlayer: true);
                }
            }
            
        });
        return false;
    }

}
