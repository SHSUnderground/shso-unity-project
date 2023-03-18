using System.Collections.Generic;
using UnityEngine;

public class ExpendablesPrerequisiteHandlerDefault : ExpendablesPrerequisiteHandlerBase
{
	public override PrerequisiteCheckResult Check(ExpendablesManager manager, ExpendableDefinition def)
	{
		if (manager.activeExpendQueue.Count > 0)
		{
			foreach (KeyValuePair<int, IExpendHandler> item in manager.activeExpendQueue)
			{
				if (item.Value.OwnableTypeId == def.OwnableTypeId)
				{
					PrerequisiteCheckResult result = default(PrerequisiteCheckResult);
					result.State = PrerequisiteCheckStateEnum.AlreadyInUseInvalid;
					result.StateExplanation = "#EXP_INUSE_INVALID";
					return result;
				}
				if (item.Value.ExpendableDefinition.Exclusive)
				{
					PrerequisiteCheckResult result2 = default(PrerequisiteCheckResult);
					result2.State = PrerequisiteCheckStateEnum.UsingExclusiveInvalid;
					result2.StateExplanation = "#EXP_EXCLUSIVE_INUSE_INVALID";
					return result2;
				}
			}
		}
		if (manager.NextExpendableStartTime > Time.time)
		{
			PrerequisiteCheckResult result3 = default(PrerequisiteCheckResult);
			result3.State = PrerequisiteCheckStateEnum.CooldownStateInvalid;
			result3.StateExplanation = "#EXP_COOLDOWN_INVALID";
			return result3;
		}
		CharacterGlobals currentCharGlobals = null;
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			currentCharGlobals = localPlayer.GetComponent<CharacterGlobals>();
		}
		if (CheckBehavior(currentCharGlobals))
		{
			PrerequisiteCheckResult result4 = default(PrerequisiteCheckResult);
			result4.StateExplanation = string.Empty;
			return result4;
		}
		PrerequisiteCheckResult result5 = default(PrerequisiteCheckResult);
		result5.State = PrerequisiteCheckStateEnum.CharacterStateInvalid;
		result5.StateExplanation = "#EXP_CHARACTER_INVALID";
		return result5;
	}

	public bool CheckBehavior(CharacterGlobals CurrentCharGlobals)
	{
		if (CurrentCharGlobals == null || CurrentCharGlobals.motionController == null || CurrentCharGlobals.motionController.IsJumping())
		{
			return false;
		}
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager == null || behaviorManager.getBehavior() == null || !behaviorManager.currentBehaviorInterruptible(typeof(BehaviorEffectExpendable)))
		{
			return false;
		}
		if (!behaviorManager.allowUserInput())
		{
			return false;
		}
		BehaviorMovement behaviorMovement = behaviorManager.getBehavior() as BehaviorMovement;
		if (behaviorMovement != null)
		{
			if (behaviorMovement is BehaviorApproach)
			{
				return false;
			}
			switch (behaviorMovement.CurrentMovementState)
			{
			case BehaviorMovement.MovementState.Undefined:
			case BehaviorMovement.MovementState.Jumping:
			case BehaviorMovement.MovementState.Airborne:
			case BehaviorMovement.MovementState.Landing:
				return false;
			}
		}
		return true;
	}
}
