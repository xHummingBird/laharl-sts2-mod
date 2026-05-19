using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Cards.Token;
using Laharl.LaharlCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using static MegaCrit.Sts2.Core.Models.CardModel;

namespace Laharl.LaharlCode.Powers;

public class DarkAssemblyPower : LaharlPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            List<CardModel> cards = new()
            {
                CombatState.CreateCard<Aye>(base.Owner.Player),
                CombatState.CreateCard<Nay>(base.Owner.Player),
            };
            CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards.ToList(), base.Owner.Player, canSkip: false);
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, base.Owner.Player);

            Flash();
        }
    }
}