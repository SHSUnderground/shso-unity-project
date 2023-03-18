using UnityEngine;

public class CombatEffectScale : CombatEffectBase
{
	protected enum ScaleState
	{
		ScaleNormal,
		ScaleRampUp,
		ScaleMax,
		ScaleRampDown
	}

	protected Vector3 scaleToApply;

	private CharacterMotionController motionController;

	protected Transform scaleTarget;

	protected Vector3 scaleOffsetToApply;

	protected Vector3 currentScale;

	protected Vector3 scaleIncrement;

	protected Vector3 scaleOffsetIncrement;

	protected Vector3 scaleNorm;

	protected float scaleRampUpStart;

	protected float scaleRampDownStart;

	protected ScaleState currentScaleState;

	protected override void ReleaseEffect()
	{
		switch (currentScaleState)
		{
		case ScaleState.ScaleRampUp:
		case ScaleState.ScaleRampDown:
			ApplyScale(ref currentScale, false);
			break;
		case ScaleState.ScaleMax:
			ApplyScale(ref scaleToApply, false);
			break;
		}
		currentScaleState = ScaleState.ScaleNormal;
		if (scaleTarget != null)
		{
			Utils.AttachGameObject(scaleTarget.parent, scaleTarget.GetChild(0));
			Utils.DetachGameObject(scaleTarget.gameObject);
			Object.Destroy(scaleTarget.gameObject);
			scaleTarget = null;
		}
		base.ReleaseEffect();
	}

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		scaleToApply = newCombatEffectData.scale;
		scaleOffsetToApply = newCombatEffectData.scaleOffset;
		GameObject gameObject = base.transform.root.gameObject;
		motionController = (gameObject.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController);
		scaleRampUpStart = 0f;
		scaleRampDownStart = 0f;
		scaleNorm.x = (scaleNorm.y = (scaleNorm.z = 1f));
		if (!string.IsNullOrEmpty(newCombatEffectData.scaleBone))
		{
			Transform transform = Utils.FindNodeInChildren(gameObject.transform, newCombatEffectData.scaleBone, true);
			if (transform != null)
			{
				GameObject gameObject2 = new GameObject(transform.name + "_transformer");
				Utils.AttachGameObject(transform.parent, gameObject2.transform);
				Utils.AttachGameObject(gameObject2.transform, transform);
				scaleTarget = gameObject2.transform;
			}
		}
		if (combatEffectData.scaleRampDuration > 0f)
		{
			scaleRampUpStart = Time.time;
			scaleRampDownStart = combatEffectData.scaleDuration - combatEffectData.scaleRampDuration * 2f;
			if (scaleRampDownStart > 0f)
			{
				scaleRampDownStart = Time.time + scaleRampDownStart;
			}
			else
			{
				scaleRampDownStart = Time.time + combatEffectData.scaleDuration;
				CspUtils.DebugLog("CombatEffectScale: no scale ramp down at end of scale time <" + combatEffectData.scaleDuration + "> with ramp time <" + combatEffectData.scaleRampDuration + "> on game object <" + base.gameObject.name + ">");
			}
			scaleIncrement = (scaleToApply - scaleNorm) / combatEffectData.scaleRampDuration;
			if (scaleTarget != null)
			{
				scaleOffsetIncrement = scaleOffsetToApply / combatEffectData.scaleRampDuration;
			}
			currentScaleState = ScaleState.ScaleNormal;
		}
		else
		{
			ApplyScale(ref scaleToApply, true);
			ApplyScaleOffset(ref scaleOffsetToApply);
			currentScaleState = ScaleState.ScaleMax;
		}
	}

	private void Update()
	{
		if (!(combatEffectData.scaleRampDuration > 0f))
		{
			return;
		}
		if (currentScale != Vector3.zero)
		{
			ApplyScale(ref currentScale, false);
			currentScale = Vector2.zero;
		}
		ScaleState scaleState = GetCurrentScaleState();
		switch (scaleState)
		{
		case ScaleState.ScaleRampUp:
		{
			currentScale = scaleNorm + scaleIncrement * (Time.time - scaleRampUpStart);
			ApplyScale(ref currentScale, true);
			Vector3 scaleOffset = scaleOffsetIncrement * (Time.time - scaleRampUpStart);
			ApplyScaleOffset(ref scaleOffset);
			break;
		}
		case ScaleState.ScaleRampDown:
		{
			currentScale = scaleToApply + scaleIncrement * (0f - (Time.time - scaleRampDownStart));
			ApplyScale(ref currentScale, true);
			Vector3 scaleOffset = scaleOffsetToApply + scaleOffsetIncrement * (0f - (Time.time - scaleRampDownStart));
			ApplyScaleOffset(ref scaleOffset);
			break;
		}
		}
		if (currentScaleState != scaleState)
		{
			if (scaleState == ScaleState.ScaleMax)
			{
				ApplyScale(ref scaleToApply, true);
				ApplyScaleOffset(ref scaleOffsetToApply);
			}
			else if (currentScaleState == ScaleState.ScaleMax)
			{
				ApplyScale(ref scaleToApply, false);
			}
			currentScaleState = scaleState;
		}
	}

	protected ScaleState GetCurrentScaleState()
	{
		float num = Time.time - scaleRampUpStart;
		float num2 = Time.time - scaleRampDownStart;
		if (num < combatEffectData.scaleRampDuration)
		{
			return ScaleState.ScaleRampUp;
		}
		if (num >= combatEffectData.scaleRampDuration && num2 < 0f)
		{
			return ScaleState.ScaleMax;
		}
		if (num2 < combatEffectData.scaleRampDuration)
		{
			return ScaleState.ScaleRampDown;
		}
		return ScaleState.ScaleNormal;
	}

	protected void ApplyScale(ref Vector3 scale, bool adding)
	{
		if (scaleTarget != null)
		{
			if (adding)
			{
				scaleTarget.localScale = Vector3.Scale(scaleTarget.localScale, scale);
				return;
			}
			Vector3 localScale = scaleTarget.localScale;
			localScale.x /= scale.x;
			localScale.y /= scale.y;
			localScale.z /= scale.z;
			scaleTarget.localScale = localScale;
		}
		else
		{
			motionController.changeScale(scale, adding);
		}
	}

	protected void ApplyScaleOffset(ref Vector3 scaleOffset)
	{
		if (scaleTarget != null)
		{
			scaleTarget.localPosition = scaleOffset;
		}
	}
}
