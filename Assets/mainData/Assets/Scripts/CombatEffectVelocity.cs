using UnityEngine;

public class CombatEffectVelocity : CombatEffectBase
{
	private CharacterMotionController motionController;

	protected Vector3 velocityToAdd;

	protected GameObject gravityWellObject;

	protected Vector3 gravityWellPoint;

	private float farVelocity;

	private float nearVelocity;

	protected Vector3 gravityVelocityApplied;

	private bool useGravityWell;

	private float rampUpTime;

	private float rampTimer;

	protected override void ReleaseEffect()
	{
		if (velocityToAdd.sqrMagnitude != 0f)
		{
			motionController.ExtraVelocity -= velocityToAdd;
		}
		if (useGravityWell)
		{
			motionController.ExtraVelocity -= gravityVelocityApplied;
		}
		base.ReleaseEffect();
	}

	public new void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = base.transform.root.gameObject;
		motionController = (gameObject.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController);
		if (newCombatEffectData.appliedVelocity.sqrMagnitude != 0f)
		{
			velocityToAdd = newCombatEffectData.appliedVelocity;
			motionController.ExtraVelocity += velocityToAdd;
		}
		if (newCombatEffectData.usesGravityWell)
		{
			useGravityWell = true;
			if (newCombatEffectData.gravityWellObject != null && newCombatEffectData.gravityWellObject != string.Empty)
			{
				gravityWellObject = Utils.FindNodeInChildren(sourceCombatController.transform, newCombatEffectData.gravityWellObject).gameObject;
			}
			gravityWellPoint = sourceCombatController.transform.position + newCombatEffectData.gravityWellOffset;
			farVelocity = newCombatEffectData.gravityFarStrength;
			nearVelocity = newCombatEffectData.gravityNearStrength;
			rampUpTime = newCombatEffectData.minimumDuration.getValue(sourceCombatController);
		}
	}

	private void Update()
	{
		if (!useGravityWell)
		{
			return;
		}
		motionController.ExtraVelocity -= gravityVelocityApplied;
		Vector3 vector = (!(gravityWellObject != null)) ? (motionController.transform.position - gravityWellPoint) : (motionController.transform.position - gravityWellObject.transform.position);
		float magnitude = vector.magnitude;
		if ((double)magnitude <= 0.01)
		{
			gravityVelocityApplied = new Vector3(0f, 0f, 0f);
			return;
		}
		gravityVelocityApplied = vector.normalized;
		float num = 1f;
		if (rampUpTime > 0f)
		{
			rampTimer += Time.deltaTime;
			num = Mathf.Min(rampTimer / rampUpTime, 1f);
		}
		float num2 = nearVelocity * (1f - num) + farVelocity * num;
		gravityVelocityApplied *= 0f - num2;
		motionController.ExtraVelocity += gravityVelocityApplied;
	}
}
