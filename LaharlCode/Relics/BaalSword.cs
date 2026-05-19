using Laharl.LaharlCode.Cards.Ancient;
using Laharl.LaharlCode.Powers;
using Laharl.LaharlCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Laharl.LaharlCode.Relics;

public class BaalSword() : LaharlRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigorPower>(10m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromCard<OverlordsDimension>()
    ];
    
    public override bool HasUponPickupEffect => true;
    
    public override async Task AfterObtained()
    {
        var relic = base.Owner.Relics.FirstOrDefault(r => r is Yoshitsuna);
        if (relic != null)
        {
            await RelicCmd.Remove(relic);
        }
        CardModel card = base.Owner.RunState.CreateCard<OverlordsDimension>(base.Owner);
        CardCmd.PreviewCardPileAdd((await CardPileCmd.Add(card, PileType.Deck)), 2f);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["VigorPower"].IntValue, base.Owner.Creature, null);
        }
    }
}