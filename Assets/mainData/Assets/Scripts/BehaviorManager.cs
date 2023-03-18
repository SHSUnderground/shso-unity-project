using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected sealed class AnimationOverride
	{
		private string _override;

		private int _references;

		public string Override
		{
			get
			{
				return _override;
			}
		}

		public AnimationOverride(string name)
		{
			_override = name;
			_references = 1;
		}

		public void AddReference(string name)
		{
			if (_override != name)
			{
				CspUtils.DebugLog("BehaviorManager::OverrideAnimation() - attempting to add a reference for existing override <" + _override + "> with different override <" + name + ">");
			}
			else
			{
				_references++;
			}
		}

		public int RemoveReference(string name)
		{
			if (_override != name)
			{
				CspUtils.DebugLog("BehaviorManager::OverrideAnimation() - attempting to remove a reference for existing override <" + _override + "> with different override <" + name + ">");
				return _references;
			}
			return --_references;
		}
	}

	public string defaultBehaviorType = "BehaviorMovement";

	protected BehaviorBase defaultBehavior;

	protected BehaviorBase currentBehavior;

	public string currentBehaviorName;

	protected BehaviorBase queuedBehavior;

	public CharacterGlobals charGlobals;

	protected FacialAnimation face;

	protected Dictionary<string, AnimationOverride> animationOverrides;

	public string runAnimOverride = string.Empty;

	public string idleAnimOverride = string.Empty;

	protected bool motionEnabled = true;

	public bool MotionEnabled
	{
		get
		{
			return motionEnabled;
		}
	}

	private void Start()
	{
		charGlobals = (base.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		face = Utils.GetComponent<FacialAnimation>(base.gameObject);
		defaultBehavior = createBehavior(Type.GetType(defaultBehaviorType));
		pickBehavior();
	}

	private void Update()
	{
		if (currentBehavior != null)
		{
			currentBehavior.behaviorUpdate();
		}
		else
		{
			CspUtils.DebugLog("currentBehavior is null in Update");
		}
		if (motionEnabled && charGlobals != null && charGlobals.motionController != null && charGlobals.networkComponent == null && base.gameObject.active)
		{
			charGlobals.motionController.MotionUpdate();
		}
	}

	private void LateUpdate()
	{
		if (currentBehavior != null)
		{
			currentBehavior.behaviorLateUpdate();
		}
		else
		{
			CspUtils.DebugLog("currentBehavior is null in LateUpdate");
		}
		if (charGlobals != null && charGlobals.motionController != null)
		{
			charGlobals.motionController.behaviorLateUpdate();
		}
	}

	private void OnDisable()
	{
		if (currentBehavior != null)
		{
			currentBehavior.behaviorEnd();
		}
	}

	public BehaviorBase ChangeDefaultBehavior(string newDefaultBehavior)
	{
		defaultBehavior = createBehavior(Type.GetType(newDefaultBehavior));
		return defaultBehavior;
	}

	protected void pickBehavior()
	{
		if (queuedBehavior != null && defaultBehavior.allowInterrupt(queuedBehavior.GetType()))
		{
			changeBehavior(queuedBehavior);
		}
		else
		{
			changeBehavior(defaultBehavior);
		}
	}

	protected void changeBehavior(BehaviorBase newBehavior)
	{
		if (newBehavior == null)
		{
			CspUtils.DebugLog("changeBehavior requested a null behavior");
		}
		queuedBehavior = null;
		BehaviorBase behaviorBase = currentBehavior;
		currentBehavior = newBehavior;
		currentBehaviorName = newBehavior.ToString();
		if (behaviorBase != null)
		{
			behaviorBase.behaviorEnd(currentBehavior);
		}
		face.ResetToNormal();
		if (charGlobals.motionController.carriedThrowable != null && !(newBehavior is BehaviorMovement) && !(newBehavior is BehaviorAttackApproach) && !(newBehavior is BehaviorAttackBase) && !(newBehavior is BehaviorApproach))
		{
			charGlobals.motionController.dropThrowable();
		}
		currentBehavior.behaviorBegin(behaviorBase);
	}

	protected BehaviorBase createBehavior(Type newBehaviorType)
	{
		BehaviorBase behaviorBase = Activator.CreateInstance(newBehaviorType) as BehaviorBase;
		behaviorBase.behaviorSetup(base.gameObject, charGlobals);
		return behaviorBase;
	}

	public BehaviorBase forceChangeBehavior(Type newBehaviorType)
	{
		if (currentBehaviorForcedInterruptible(newBehaviorType))
		{
			BehaviorBase behaviorBase = createBehavior(newBehaviorType);
			changeBehavior(behaviorBase);
			return behaviorBase;
		}
		return null;
	}

	public void clearQueuedBehavior()
	{
		queuedBehavior = null;
	}

	public void cancelBehavior()
	{
		if (currentBehavior != null)
		{
			currentBehavior.behaviorCancel();
		}
	}

	public BehaviorBase requestChangeBehavior(Type newBehaviorType, bool canBeQueued)
	{
		HairTrafficController hairTrafficController = (HairTrafficController)base.gameObject.GetComponent(typeof(HairTrafficController));
		if (hairTrafficController != null)
		{
			AppShell.Instance.EventMgr.Fire(null, new RequestChangeBehaviorMessage(hairTrafficController.playerId));
		}
		if (currentBehaviorInterruptible(newBehaviorType))
		{
			BehaviorBase behaviorBase = createBehavior(newBehaviorType);
			changeBehavior(behaviorBase);
			return behaviorBase;
		}
		if (canBeQueued)
		{
			return queuedBehavior = createBehavior(newBehaviorType);
		}
		return null;
	}

	public T requestChangeBehavior<T>(bool canBeQueued) where T : BehaviorBase
	{
		return requestChangeBehavior(typeof(T), canBeQueued) as T;
	}

	public void setMotionEnabled(bool enableMotion)
	{
		motionEnabled = enableMotion;
	}

	public bool currentBehaviorInterruptible(Type newBehaviorType)
	{
		return currentBehavior == null || currentBehavior.allowInterrupt(newBehaviorType);
	}

	public bool currentBehaviorForcedInterruptible(Type newBehaviorType)
	{
		return currentBehavior == null || currentBehavior.allowForcedInterrupt(newBehaviorType);
	}

	public void endBehavior()
	{
		if (currentBehavior.allowForcedInterrupt(Type.GetType(defaultBehaviorType)))
		{
			pickBehavior();
		}
	}

	public BehaviorBase getBehavior()
	{
		return currentBehavior;
	}

	public BehaviorBase getQueuedBehavior()
	{
		return queuedBehavior;
	}

	public bool allowUserInput()
	{
		if (currentBehavior != null)
		{
			return currentBehavior.allowUserInput();
		}
		return true;
	}

	public void userInputOverride()
	{
		if (currentBehavior != null)
		{
			currentBehavior.userInputOverride();
		}
	}

	public bool useMotionController()
	{
		if (currentBehavior != null)
		{
			return currentBehavior.useMotionController();
		}
		return true;
	}

	public bool useMotionControllerRotate()
	{
		if (currentBehavior != null)
		{
			return currentBehavior.useMotionControllerRotate();
		}
		return true;
	}

	public bool useMotionControllerGravity()
	{
		if (currentBehavior != null)
		{
			return currentBehavior.useMotionControllerGravity();
		}
		return true;
	}

	public string getCurrentAttackName()
	{
		BehaviorAttackBase behaviorAttackBase = currentBehavior as BehaviorAttackBase;
		if (behaviorAttackBase != null)
		{
			return behaviorAttackBase.getAttackName();
		}
		return "attackInvalid";
	}

	public BehaviorRecoil getCurrentRecoil()
	{
		return currentBehavior as BehaviorRecoil;
	}

	public void OnCutSceneStart()
	{
		if (currentBehavior != null && currentBehavior.behaviorEndOnCutScene())
		{
			clearQueuedBehavior();
			currentBehavior.behaviorEnd();
			endBehavior();
		}
	}

	public bool HasTarget<T>() where T : BehaviorBase
	{
		T val = getBehavior() as T;
		return val != null && val.getTarget() != null;
	}

	public bool HasAwaitingTarget<T>() where T : BehaviorBase
	{
		T val = getQueuedBehavior() as T;
		return val != null && val.getTarget() != null;
	}

	public string GetAnimationName(string animationKey)
	{
		if (HasAnimationOverride(animationKey))
		{
			return GetAnimationOverride(animationKey);
		}
		return animationKey;
	}

	public string GetAnimationOverride(string animation)
	{
		return animationOverrides[animation].Override;
	}

	public void OverrideAnimation(string baseAnimation, string overrideAnimation)
	{
		if (string.IsNullOrEmpty(baseAnimation) || string.IsNullOrEmpty(overrideAnimation))
		{
			return;
		}
		if (charGlobals != null)
		{
			if (charGlobals.animationComponent[baseAnimation] == null)
			{
				CspUtils.DebugLog("BehaviorManager::OverrideAnimation() - base animation <" + baseAnimation + "> does not exist for character <" + base.gameObject.name + ">");
				return;
			}
			if (charGlobals.animationComponent[overrideAnimation] == null)
			{
				CspUtils.DebugLog("BehaviorManager::OverrideAnimation() - override animation <" + overrideAnimation + "> does not exist for character <" + base.gameObject.name + ">");
				return;
			}
			charGlobals.animationComponent[overrideAnimation].wrapMode = charGlobals.animationComponent[baseAnimation].wrapMode;
		}
		if (animationOverrides == null)
		{
			animationOverrides = new Dictionary<string, AnimationOverride>();
		}
		if (animationOverrides.ContainsKey(baseAnimation))
		{
			AnimationOverride animationOverride = animationOverrides[baseAnimation];
			animationOverride.AddReference(overrideAnimation);
			return;
		}
		animationOverrides.Add(baseAnimation, new AnimationOverride(overrideAnimation));
		if (currentBehavior != null)
		{
			currentBehavior.animationOverriden(baseAnimation, overrideAnimation);
		}
	}

	public void RemoveAnimationOverride(string baseAnimation, string overrideAnimation)
	{
		if (string.IsNullOrEmpty(baseAnimation) || string.IsNullOrEmpty(overrideAnimation) || animationOverrides == null)
		{
			return;
		}
		if (animationOverrides.ContainsKey(baseAnimation))
		{
			AnimationOverride animationOverride = animationOverrides[baseAnimation];
			if (animationOverride.RemoveReference(overrideAnimation) <= 0)
			{
				animationOverrides.Remove(baseAnimation);
				if (currentBehavior != null)
				{
					currentBehavior.animationOverriden(overrideAnimation, baseAnimation);
				}
			}
		}
		if (animationOverrides.Count <= 0)
		{
			animationOverrides = null;
		}
	}

	public bool HasAnimationOverride(string animation)
	{
		return animationOverrides != null && animationOverrides.ContainsKey(animation);
	}

	protected void HitByEnemy(CombatController enemy)
	{
		if (currentBehavior != null)
		{
			currentBehavior.HitByEnemy(enemy);
		}
	}
}
