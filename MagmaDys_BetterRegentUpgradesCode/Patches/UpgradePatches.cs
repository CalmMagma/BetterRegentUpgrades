using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;


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

                    await CardPileCmd.AddGeneratedCardToCombat(cardToGive, PileType.Hand, __instance.Owner);
                }
            }
            
        });
        return false;
    }
    
    // -- ROYALTIES --
    [HarmonyPostfix, HarmonyPatch(typeof(Royalties), "OnPlay")]
    static void RoyaltiesUpgrade(Royalties __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay,
        ref Task __result)
    {
        __result = Task.Run(async () =>
            {
                if (__instance.IsUpgraded)
                { 
                    await PowerCmd.Apply<UpgradedRoyaltiesPower>(choiceContext, __instance.Owner.Creature, 1, __instance.Owner.Creature, __instance, true);
                }
            }
        );
    }
    
    // -- FALLING STAR --
    [HarmonyPrefix, HarmonyPatch(typeof(FallingStar), "OnUpgrade")]
    static bool FallingStarUpgrade(FallingStar __instance)
    {
        __instance.DynamicVars.Weak.UpgradeValueBy(1);
        __instance.DynamicVars.Vulnerable.UpgradeValueBy(1);
        return false;
    }
    
    // -- GAMMA BLAST--
    [HarmonyPrefix, HarmonyPatch(typeof(GammaBlast), "OnUpgrade")]
    static bool GammaBlastUpgrade(GammaBlast __instance)
    {
        __instance.DynamicVars.Weak.UpgradeValueBy(1);
        __instance.DynamicVars.Vulnerable.UpgradeValueBy(1);
        return false;
    }
    
    // -- MANIFEST AUTHORITY

    [HarmonyPrefix, HarmonyPatch(typeof(ManifestAuthority), "OnUpgrade")]
    static bool ManifestAuthorityUpgrade(ManifestAuthority __instance)
    {
        __instance.DynamicVars.Block.UpgradeValueBy(4);
        return false;
    }
    
    // -- BUNDLE OF JOY --
    
    [HarmonyPrefix, HarmonyPatch(typeof(BundleOfJoy), "OnUpgrade")]
    static bool BundleOfJoyUpgrade(BundleOfJoy __instance)
    {
        __instance.RemoveKeyword(CardKeyword.Exhaust);
        return false;
    }
    
    // -- SPOILS OF BATTLE --
    
    [HarmonyPostfix, HarmonyPatch(typeof(SpoilsOfBattle), "OnPlay")]
    static void SpoilsOfBattleUpgrade(SpoilsOfBattle __instance)
    {
        Task.Run(async()=>
        {
            if (__instance.IsUpgraded)
                await PlayerCmd.GainStars(__instance.DynamicVars.Stars.BaseValue, __instance.Owner);
          
        });
        
    } 
    
    [HarmonyPrefix, HarmonyPatch(typeof(SpoilsOfBattle), "CanonicalVars", MethodType.Getter)]
    static bool SpoilsOfBattleUpgrade(SpoilsOfBattle __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result = new List<DynamicVar>
        {
            new ForgeVar(5),
            new CardsVar(2),
            new StarsVar(1)
        };
        return false;
    }
    
    // -- SPECTRUM SHIFT --

    [HarmonyPrefix, HarmonyPatch(typeof(SpectrumShift), "OnPlay")]
    static bool SpectrumShiftOnPlay(SpectrumShift __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = Task.Run(async () =>
        {
            await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
            if (__instance.IsUpgraded)
            {
              await PowerCmd.Apply<bruSpectrumShiftPower>(choiceContext, __instance.Owner.Creature, __instance.DynamicVars.Cards.BaseValue, __instance.Owner.Creature, __instance);
            }
            else
            {
              await PowerCmd.Apply<SpectrumShiftPower>(choiceContext, __instance.Owner.Creature, __instance.DynamicVars.Cards.BaseValue, __instance.Owner.Creature, __instance);  
            }
        });

        return false;
    }
    
    // -- FOREGONE CONCLUSION

    [HarmonyPrefix, HarmonyPatch(typeof(ForegoneConclusion), "OnUpgrade")]
    static bool ForgoneConclusionUpgrade(ForegoneConclusion __instance)
    {
        return false;
    }

     [HarmonyPrefix, HarmonyPatch(typeof(ForegoneConclusion), "OnPlay")]
    static bool ForegoneConclusionOnPlay(ForegoneConclusion __instance, PlayerChoiceContext choiceContext, ref Task __result)
     {
        __result = Task.Run(async () =>
        {
            await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
            if (__instance.IsUpgraded)
                await PowerCmd.Apply<bruForegoneConclusionPower>(choiceContext, __instance.Owner.Creature, __instance.DynamicVars.Cards.BaseValue, __instance.Owner.Creature, __instance);
            else
                await PowerCmd.Apply<ForegoneConclusionPower>(choiceContext, __instance.Owner.Creature, __instance.DynamicVars.Cards.BaseValue, __instance.Owner.Creature, __instance);
        });
        return false;
    }
    
    [HarmonyPrefix, HarmonyPatch(typeof(ForegoneConclusion), "CanonicalVars", MethodType.Getter)]
    static bool ForegoneConclusionKeywords(ForegoneConclusion __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result = new List<DynamicVar>
        {
            new CardsVar(2),
            new EnergyVar(1)
        };
        return false;
    }

    // -- CRASH LANDING -- 
    /*
    [HarmonyPrefix, HarmonyPatch(typeof(CrashLanding), "OnUpgrade")]
    static bool CrashLandingUpgrade(CrashLanding __instance)
    {
         __instance.DynamicVars.ExtraDamage.UpgradeValueBy(2);
        return false;
    } 

    [HarmonyPrefix, HarmonyPatch(typeof(CrashLanding), "CanonicalVars", MethodType.Getter)]
    static bool CrashLandingVars(CrashLanding __instance, ref IEnumerable<DynamicVar> __result)
    {
        __result = new List<DynamicVar>
        {
            new CalculationBaseVar(21),
            new ExtraDamageVar(0),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
                card.Owner.PlayerCombatState.AllCards.Count((CardModel c) => c is Debris))
        };
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(typeof(CrashLanding), "OnPlay")]
    static bool CrashLandingOnPlay(CrashLanding __instance, ref Task __result, PlayerChoiceContext choiceContext)
    {
        __result = new Task(async () =>
        {
            try {
                await DamageCmd.Attack(__instance.DynamicVars.CalculatedDamage.BaseValue).FromCard(__instance).TargetingAllOpponents(__instance.CombatState)
                    .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
                    .WithHitVfxSpawnedAtBase()
                    .Execute(choiceContext);
                int num = 10 - CardPile.GetCards(__instance.Owner, PileType.Hand).Count();
                List<CardModel> list = new List<CardModel>();
                for (int i = 0; i < num; i++)
                {       
                    list.Add(__instance.CombatState.CreateCard<Debris>(__instance.Owner));
                }       
                await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
            } catch (Exception e) {
                MainFile.Logger.LogMessage(LogLevel.Warn, e.ToString(), 0);
            }
        });
        return false;
    }*/
}   
