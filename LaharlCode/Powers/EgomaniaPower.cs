using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Laharl.LaharlCode.Powers;

public class EgomaniaPower : LaharlPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EgoPower>()
    ];
    
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext playerChoiceContext, Player player)
    {
        if (player == base.Owner.Player && base.Owner.Player.HasPower<EgoPower>())
        {
            await CardPileCmd.Draw(playerChoiceContext, 1, base.Owner.Player);
            Flash();
        }
    }
}
