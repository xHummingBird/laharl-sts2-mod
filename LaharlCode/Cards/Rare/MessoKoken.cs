using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Laharl.LaharlCode.Cards.Rare;

public class MessoKoken() : LaharlCard(1, CardType.Power,
CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<MessoKokenPower>(25m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>()
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
    await PowerCmd.Apply<MessoKokenPower>(choiceContext, base.Owner.Creature, base.DynamicVars["MessoKokenPower"].BaseValue, base.Owner.Creature, this);
}

protected override void OnUpgrade()
{
    base.DynamicVars["MessoKokenPower"].UpgradeValueBy(25m);
}
}