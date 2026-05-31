using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Laharl.LaharlCode.Character;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;
using Godot;
using Laharl.LaharlCode.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Color = Godot.Color;

namespace Laharl.LaharlCode.Powers;

public class BurnPower : LaharlPower
{
    private const string _damageIncrease = "DamageIncrease";
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Same structure as Poison: number of times to trigger per turn.
    // Keeping AccelerantPower interaction "as-is" per your request.
    
    // Optional helper: expected damage next tick (respects block, so this is just "amount modified")
    public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageIncrease", 1m)
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return 1m;
        }
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        decimal num = base.DynamicVars["DamageIncrease"].BaseValue;
        if (dealer != null)
        {
            InfernalSword infernalSword = dealer.Player?.GetRelic<InfernalSword>();
            if (infernalSword != null)
            {
                num = infernalSword.ModifyBurnMultiplier(target, num, props, dealer, cardSource);
            }
            MessoKokenPower power = dealer.GetPower<MessoKokenPower>();
            if (power != null)
            {
                num = power.ModifyBurnMultiplier(target, num, props, dealer, cardSource);
            }
        }
        return num;
    }
    
    
    private int TriggerCount
    {
        get
        {
            int extra = base.Owner.HasPower<WeakPower>() ? 1 : 0;
            return Math.Min(base.Amount, 1 + extra);
        }
    }

    public int CalculateTotalDamageNextTurn()
    {
        decimal num = default(decimal);
        int num2 = Math.Min(base.Amount, TriggerCount);
        for (int i = 0; i < num2; i++)
        {
            decimal damage = base.Amount - i;
            damage = Hook.ModifyDamage(base.Owner.CombatState.RunState, base.Owner.CombatState, base.Owner, null, damage, ValueProp.Unblockable | ValueProp.Unpowered, null, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
            num += damage;
        }
        return (int)num;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != base.Owner.Side)
        {
            return;
        }

        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
        }
        int iterations = TriggerCount;
        for (int i = 0; i < iterations; i++)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }

        if (base.Owner.IsAlive)
        {
            await PowerCmd.Decrement(this);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }
    }
