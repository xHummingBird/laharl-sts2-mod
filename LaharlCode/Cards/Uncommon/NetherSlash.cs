using BaseLib.Utils;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class NetherSlash() : LaharlCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<BurnPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // ✅ Owner is on the card model, not on CardPlay
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // Play your animation (hard-coded)
            float duration = laharl.PlayAnimation(ownerCreature, "attack_laharl").total;
            
            SfxCmd.Play("res://Laharl/sfx/kurae.mp3");
            
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (play.Target != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(play.Target));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            await PowerCmd.Apply<BurnPower>(choiceContext, play.Target, base.DynamicVars["BurnPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        base.DynamicVars["BurnPower"].UpgradeValueBy(1m);
    }
}