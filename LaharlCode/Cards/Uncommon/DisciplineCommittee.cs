using BaseLib.Utils;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class DisciplineCommittee() : LaharlCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new RepeatVar(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Aye>(),
        HoverTipFactory.FromCard<Nay>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {   
        var ownerCreature = Owner?.Creature;
        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            SfxCmd.Play("res://Laharl/sfx/kakugou.mp3");
        }
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        List<CardModel> cards = new()
        {
            CombatState.CreateCard<Aye>(base.Owner),
            CombatState.CreateCard<Nay>(base.Owner),
        };
        CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards.ToList(), base.Owner, canSkip: false);
        await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2);
    }
}