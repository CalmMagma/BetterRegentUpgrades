using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using BaseLib.Abstracts;

namespace BetterRegentUpgrades.BetterRegentUpgradesCode;


public class UpgradedRoyaltiesPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
  
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Pool is ColorlessCardPool)
            await PowerCmd.Apply<RoyaltiesPower>(this.Owner, 3*this.Amount, this.Owner, cardPlay.Card, true);
    }

}