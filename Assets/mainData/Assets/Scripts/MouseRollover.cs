using UnityEngine;

[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(BehaviorManager))]
[RequireComponent(typeof(CharacterMotionController))]
public class MouseRollover
{
	public CharacterGlobals charGlobals;

	public bool allowUserInput;

	public GameObject character;

	public MouseRollover(CharacterGlobals charGlobals, bool allowUserInput, GameObject character)
	{
		this.charGlobals = charGlobals;
		this.allowUserInput = allowUserInput;
		this.character = character;
	}
}
