using Laharl.LaharlCode.Cards.Ancient;
using Laharl.LaharlCode.Cards.Basic;
using Laharl.LaharlCode.Powers;
using Laharl.LaharlCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Laharl.LaharlCode.Relics;

public class HellfireKnuckleRelic() : LaharlRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BurnPower>(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>(),
        HoverTipFactory.FromCard<BlazingKnuckle>(),
        HoverTipFactory.FromCard<HellfireKnuckle>()
    ];
    
    public override bool HasUponPickupEffect => true;
    
    public override async Task AfterObtained()
    {
        CardModel? original = base.Owner.Deck.Cards
            .FirstOrDefault(c => c is BlazingKnuckle);

        CardModel card = base.Owner.RunState.CreateCard<HellfireKnuckle>(base.Owner);
        if (original != null) await CardCmd.Transform(original, card);
        if (original != null && original.IsUpgraded) CardCmd.Upgrade(card);
        else CardCmd.PreviewCardPileAdd((await CardPileCmd.Add(card, PileType.Deck)), 2f);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        ICombatState combatState)
    {
        if (side != base.Owner.Creature.Side || combatState.RoundNumber > 1)
        {
            return;
        }

        Flash();
        foreach (Creature hittableEnemy in base.Owner.Creature.CombatState.HittableEnemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(hittableEnemy, VfxColor.Red));
        }
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        foreach (Creature hittableEnemy2 in base.Owner.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BurnPower>(choiceContext, hittableEnemy2, base.DynamicVars["BurnPower"].IntValue, base.Owner.Creature, null);
        }
    }
}