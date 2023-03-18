using System;
using UnityEngine;

public class BehaviorRecoilAttach : BehaviorRecoil
{
	public class RecoilInitParams
	{
		public GameObject source;

		public GameObject target;

		public Vector3 impactPosition;

		public CombatController.ImpactResultData impactResultData;

		public RecoilInitParams(GameObject source, GameObject target, Vector3 impactPosition, CombatController.ImpactResultData impactResultData)
		{
			this.source = source;
			this.target = target;
			this.impactPosition = impactPosition;
			this.impactResultData = impactResultData;
		}
	}

	protected GameObject attachObject;

	protected GameObject sourceObject;

	protected bool useDuration;

	protected bool forcePosition;

	protected float originalY;

	protected bool dismounted;

	protected bool useRotation;

	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.AttachMin;
		maxAnimLength = ActionTimesDefinition.Instance.AttachMax;
		sourceObject = source;
		animName = getAnimationName(newImpactResultData);
		base.Initialize(source, newImpactPosition, newImpactResultData);
		if (!string.IsNullOrEmpty(newImpactResultData.attachPrefabName))
		{
			EffectSequenceList component = source.GetComponent<EffectSequenceList>();
			if (component == null)
			{
				CspUtils.DebugLog("Tried to instantiate attachment prefab with name <" + newImpactResultData.attachPrefabName + ">, but the attacker does not have an effects list");
			}
			else
			{
				UnityEngine.Object effectSequencePrefabByName = component.GetEffectSequencePrefabByName(newImpactResultData.attachPrefabName);
				if (effectSequencePrefabByName == null)
				{
					CspUtils.DebugLog("Failed to instantiate attachment prefab with name <" + newImpactResultData.attachPrefabName + ">; is the object missing from one of <" + source.name + ">'s bundles?");
				}
				else
				{
					attachObject = (UnityEngine.Object.Instantiate(effectSequencePrefabByName) as GameObject);
					if (attachObject == null)
					{
						CspUtils.DebugLog("Failed to instantiate attachment prefab with name <" + newImpactResultData.attachPrefabName + ">; Is the object not a GameObject?");
					}
					else
					{
						attachObject.transform.position = charGlobals.combatController.TargetPosition;
						attachObject.SendMessage("InitializeFromRecoil", new RecoilInitParams(source, owningObject, newImpactPosition, newImpactResultData), SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		else
		{
			CombatController combatController = source.GetComponent(typeof(CombatController)) as CombatController;
			attachObject = combatController.getColliderObject(newImpactResultData.colliderName);
		}
		useDuration = newImpactResultData.useRecoilDurationOnAttach;
		forcePosition = newImpactResultData.forceAttach;
		useRotation = newImpactResultData.attachUsesRotation;
		Vector3 position = charGlobals.transform.position;
		originalY = position.y;
		charGlobals.motionController.setIsOnGround(false);
		charGlobals.motionController.LockTargetCamera(false, true, false, true, 8f);
		updateAttachPosition();
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		if (!dismounted && forcePosition)
		{
			Vector3 position = charGlobals.transform.position;
			position.y = originalY;
			charGlobals.transform.position = position;
		}
	}

	public override void behaviorUpdate()
	{
		if (useDuration)
		{
			float num = 0f;
			if (impactResultData != null && impactResultData.knockdownDuration != null)
			{
				num = impactResultData.knockdownDuration.getValue(sourceCombatController);
			}
			if (attachObject == null || elapsedTime > num)
			{
				dismount();
				return;
			}
		}
		else if (attachObject == null || !attachObject.active)
		{
			dismount();
			return;
		}
		updateAttachPosition();
		base.behaviorUpdate();
	}

	protected void dismount()
	{
		dismounted = true;
		if (sourceObject == null)
		{
			sourceObject = owningObject;
		}
		Vector3 a = charGlobals.gameObject.transform.position - sourceObject.transform.position;
		a.y = 0f;
		a.Normalize();
		if (forcePosition)
		{
			Vector3 position = charGlobals.transform.position;
			position.y = originalY;
			charGlobals.transform.position = position;
		}
		float num = impactResultData.pushbackVelocity.getValue(charGlobals.combatController);
		float num2 = impactResultData.launchVelocity.getValue(charGlobals.combatController);
		float num3 = impactResultData.pushbackDuration.getValue(charGlobals.combatController);
		if (sourceObject != null)
		{
			CombatController component = sourceObject.GetComponent<CombatController>();
			if (component != null)
			{
				if (num > component.GetMaxPushbackVelocity())
				{
					num = component.GetMaxPushbackVelocity();
				}
				if (num2 > component.GetMaxLaunchVelocity())
				{
					num2 = component.GetMaxLaunchVelocity();
				}
				if (num3 > component.GetMaxPushbackDuration())
				{
					num3 = component.GetMaxPushbackDuration();
				}
			}
		}
		if (num > 0f)
		{
			charGlobals.motionController.setForcedVelocity(a * num, num3);
		}
		if (num2 > 0f)
		{
			charGlobals.motionController.setVerticalVelocity(num2);
			BehaviorRecoilLaunch behaviorRecoilLaunch = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilLaunch)) as BehaviorRecoilLaunch;
			behaviorRecoilLaunch.Initialize(sourceObject, sourceObject.transform.position, impactResultData);
		}
		else
		{
			BehaviorRecoilSmall behaviorRecoilSmall = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilSmall)) as BehaviorRecoilSmall;
			behaviorRecoilSmall.Initialize(sourceObject, sourceObject.transform.position, impactResultData);
		}
	}

	protected void updateAttachPosition()
	{
		Vector3 position = attachObject.transform.position;
		Vector3 vector = position - charGlobals.combatController.TargetPosition;
		if (useDuration && impactResultData.pushbackVelocity.getValue(charGlobals.combatController) > 0f)
		{
			float num = impactResultData.pushbackVelocity.getValue(charGlobals.combatController) * Time.deltaTime;
			if (vector.magnitude > num)
			{
				vector = vector.normalized * num;
			}
		}
		if (!forcePosition)
		{
			charGlobals.characterController.Move(vector);
		}
		else
		{
			charGlobals.transform.position += vector;
		}
		charGlobals.motionController.setDestination(charGlobals.gameObject.transform.position);
		if (useRotation)
		{
			charGlobals.gameObject.transform.rotation = attachObject.transform.rotation;
		}
	}

	protected string getAnimationName(CombatController.ImpactResultData impact)
	{
		if (!string.IsNullOrEmpty(impact.attachAnimName))
		{
			return impact.attachAnimName;
		}
		string[] array = new string[5]
		{
			"jump_fall_loop",
			"jump_run_fall_loop",
			"jump_fall",
			"jump_run_fall",
			"movement_idle"
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (animationComponent[text] != null)
			{
				return text;
			}
		}
		return null;
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Attach;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return base.allowInterrupt(newBehaviorType);
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}

	public override bool checkRecoilEnd()
	{
		return false;
	}
}
