using BaseLib.Extensions;
using BaseLib.Utils;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Character;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Rare;


[Pool(typeof(LaharlCardPool))]
public class VacuumSliceX() : LaharlCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<EgoPower>();
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EgoPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(7m),
        new ExtraDamageVar(3m),
        new RepeatVar(3),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel _, Creature? dealer) =>
                dealer?.HasPower<EgoPower>() == true ? 1 : 0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "attack_laharl").total;
            SfxCmd.Play("res://Laharl/sfx/hirefusugaii.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(3);
    }
}