using UnityEngine;

public class SidekickSpecialAbilityMove : SidekickSpecialAbility
{
	public bool doubleJump;

	public bool superJump;

	public bool glide;

	public SidekickSpecialAbilityMove(PetUpgradeXMLDefinitionMove def)
		: base(def)
	{
		doubleJump = (def.doubleJump == 1);
		superJump = (def.superJump == 1);
		glide = (def.glide == 1);
		if (doubleJump)
		{
			icon = "shopping_bundle|double_jump";
		}
		if (superJump)
		{
			icon = "shopping_bundle|super_jump";
		}
		if (glide)
		{
			icon = "shopping_bundle|glide";
		}
	}

	public override void attachToPetObject(GameObject petObject)
	{
		CspUtils.DebugLog("SidekickSpecialAbilityMove attachToPet " + doubleJump + " " + superJump);
		CharacterGlobals component = petObject.GetComponent<CharacterGlobals>();
		if (doubleJump)
		{
			component.motionController.doubleJump = true;
		}
		if (superJump)
		{
			component.motionController.holdJump = true;
			component.motionController.jumpHeight = 20f;
		}
		if (glide)
		{
			component.motionController.CanGlide = true;
		}
	}
}
