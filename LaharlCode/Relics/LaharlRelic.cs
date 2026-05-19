using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Laharl.LaharlCode.Character;
using Laharl.LaharlCode.Extensions;
using Godot;

namespace Laharl.LaharlCode.Relics;

[Pool(typeof(LaharlRelicPool))]
public abstract class LaharlRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}