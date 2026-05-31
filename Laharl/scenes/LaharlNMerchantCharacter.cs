using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[GlobalClass]
public partial class LaharlNMerchantCharacter : NMerchantCharacter
{
	public override void _Ready()
	{
		base._Ready();
		
		var premultMat = new CanvasItemMaterial
		{
			BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
		};
		var spineBody = new MegaSprite((Variant)(GodotObject)GetChild(0));
		spineBody.SetNormalMaterial(premultMat);
	}
}
