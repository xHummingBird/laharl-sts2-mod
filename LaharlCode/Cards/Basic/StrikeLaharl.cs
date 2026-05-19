using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Character;
using Laharl.LaharlCode.Extensions;

namespace Laharl.LaharlCode.Cards.Basic;

public class StrikeLaharl() : LaharlCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // ✅ Owner is on the card model, not on CardPlay
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // Play your animation (hard-coded)
            float duration = laharl.PlayAnimation(ownerCreature, "attack_laharl").total;
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)0.35f);
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        else
        {
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        // Normal attack execution
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}

    
    
