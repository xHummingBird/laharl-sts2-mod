using BaseLib.Abstracts;
using BaseLib.Utils;
using Laharl.LaharlCode.Character;

namespace Laharl.LaharlCode.Potions;

[Pool(typeof(LaharlPotionPool))]
public abstract class LaharlPotion : CustomPotionModel;