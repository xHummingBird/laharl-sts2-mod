using BaseLib.Abstracts;
using Laharl.LaharlCode.Extensions;
using Godot;

namespace Laharl.LaharlCode.Character;

public class LaharlRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Laharl.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}