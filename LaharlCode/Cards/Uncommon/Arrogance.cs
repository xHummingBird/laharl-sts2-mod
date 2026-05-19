using BaseLib.Utils;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Character;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Laharl.LaharlCode.Cards.Uncommon;

[Pool(typeof(LaharlCardPool))]
public class Arrogance() : LaharlCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EgoPower>(),
        HoverTipFactory.FromPower<VigorPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VigorPower>(5m),
        new PowerVar<EgoPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    
    {
        var ownerCreature = Owner?.Creature;
        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "cast").total;
            SfxCmd.Play("res://Laharl/sfx/maou_no_chikara.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, base.DynamicVars["VigorPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<EgoPower>(choiceContext, base.Owner.Creature, base.DynamicVars["EgoPower"].BaseValue, base.Owner.Creature, this);
    }
    
    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner.Creature));
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}