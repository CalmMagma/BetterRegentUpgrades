using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.CardSelection;


namespace BetterRegentUpgrades.BetterRegentUpgradesCode;


public class UpgradedRoyaltiesPower : CustomPowerModel
{
    protected override bool IsVisibleInternal => false;
    public override PowerType Type => PowerType.Buff;
  
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Pool is ColorlessCardPool)
            await PowerCmd.Apply<RoyaltiesPower>(context, this.Owner, 3*this.Amount, this.Owner, cardPlay.Card, true);
    }
}

public class bruSpectrumShiftPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"res://MagmaDys_BetterRegentUpgrades/images/powers/bruspectrum_shift_power.png";
    public override string CustomBigIconPath => $"res://MagmaDys_BetterRegentUpgrades/images/powers/big/bruspectrum_shift_power.png";
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            List<CardModel> cards = CardFactory.GetDistinctForCombat(player, ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint), base.Amount, player.RunState.Rng.CombatCardGeneration).ToList();
            foreach (var card in cards)
            {
                CardCmd.Upgrade(card);
            }
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, player);
            Flash();
        }
    }
}

public class bruForegoneConclusionPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"res://MagmaDys_BetterRegentUpgrades/images/powers/bruforegone_conclusion_power.png";
    public override string CustomBigIconPath => $"res://MagmaDys_BetterRegentUpgrades/images/powers/big/bruforegone_conclusion_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == this.Owner.Player)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, this.Owner.Player);
            var list = await CardSelectCmd.FromSimpleGrid(choiceContext,
                (from c in PileType.Draw.GetPile(this.Owner.Player).Cards
                    orderby c.Rarity, c.Id
                    select c).ToList(), base.Owner.Player, new CardSelectorPrefs(this.SelectionScreenPrompt, this.Amount));
            foreach (var card in list)
                card.EnergyCost.AddThisTurn(-1);
            await CardPileCmd.Add(list, PileType.Hand);
            await PowerCmd.Remove(this);
        }
    }
    
    
}    
