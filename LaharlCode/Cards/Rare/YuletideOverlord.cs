using BaseLib.Utils;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Rare;

[Pool(typeof(LaharlCardPool))]
public class YuletideOverlord() : LaharlCard(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(15, ValueProp.Move),
        new HealVar(3m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {   
        // ✅ Owner is on the card model, not on CardPlay
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // Play your animation (hard-coded)
            float duration = laharl.PlayAnimation(ownerCreature, "punch_laharl").total;
            SfxCmd.Play("res://Laharl/sfx/magma_geyser.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        } 
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_starry_impact", null,"blunt_attack.mp3")
        .WithHitVfxSpawnedAtBase()
        .BeforeDamage(async delegate
        {
            var targets = base.CombatState.HittableEnemies;

            foreach (var target in targets)
            {
                var vfx = NGroundFireVfx.Create(target, VfxColor.Red);
                if (vfx != null)
                {
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                    SfxCmd.Play("event:/sfx/characters/attack_fire");
                }
            }
        })
        .Execute(choiceContext);
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3);
        base.DynamicVars.Heal.UpgradeValueBy(1m);
    }
}