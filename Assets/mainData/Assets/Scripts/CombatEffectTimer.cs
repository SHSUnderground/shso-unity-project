using UnityEngine;

public class CombatEffectTimer : CombatEffectBase
{
	protected float endTime;

	protected float startTime;

	protected Animation animationComponent;

	protected bool countdownReported;

	protected static readonly float CountdownTime = 2f;

	public new void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		CspUtils.DebugLog("Initialize for CombatEffectTimer " + newCombatEffectData.combatEffectName + " " + combatEffectData.minimumDuration.getValue(null, false) + " " + combatEffectData.minimumDuration.getValue(sourceCombatController) + " " + combatEffectData.maximumDuration.getValue(sourceCombatController));
		if (sourceCombatController != null)
		{
			CspUtils.DebugLog(" " + sourceCombatController.getSpecialPower() + " " + sourceCombatController.characterLevel);
		}
		startTime = Time.time;
		endTime = startTime + combatEffectData.minimumDuration.getValue(sourceCombatController);
		float num = combatEffectData.maximumDuration.getValue(sourceCombatController) - combatEffectData.minimumDuration.getValue(sourceCombatController);
		if (num > 0f)
		{
			endTime += Random.Range(0f, num);
		}
		if (sourceCombatController != null)
		{
			animationComponent = sourceCombatController.GetComponent<Animation>();
		}
	}

	private void Update()
	{
		if (!countdownReported && Time.time + CountdownTime >= endTime)
		{
			SendMessage("CountdownCombatEffect", SendMessageOptions.DontRequireReceiver);
			countdownReported = true;
		}
		if (combatEffectData.minimumDuration.getValue(null, false) > 0f && Time.time >= endTime && AnimationComplete() && (charGlobals.networkComponent == null || !charGlobals.networkComponent.IsOwnedBySomeoneElse()))
		{
			charGlobals.combatController.removeCombatEffect(combatEffectData.combatEffectName);
		}
	}

	private bool AnimationComplete()
	{
		return combatEffectData.animationDuration == string.Empty || animationComponent == null || !animationComponent.IsPlaying(combatEffectData.animationDuration);
	}
}
