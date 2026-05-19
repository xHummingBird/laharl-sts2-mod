using BaseLib.Extensions;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Common;

public class OverwhelmingOdds() : LaharlCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal =>
        base.Owner.Creature.CurrentHp * 2 < base.Owner.Creature.MaxHp;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(6, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            SfxCmd.Play("res://Laharl/sfx/nigeru.mp3");
        }
        
        int blockGains = ((base.Owner.Creature.CurrentHp * 2 < base.Owner.Creature.MaxHp) ? 2 : 1);
        for (int i = 0; i < blockGains; i++)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
    }
}