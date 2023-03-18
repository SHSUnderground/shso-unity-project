using System;
using UnityEngine;

internal class BehaviorTossTroubleBot : BehaviorBase, IAnimTagListener
{
	protected enum State
	{
		Attaching,
		Throwing,
		Done
	}

	public delegate void OnDoneThrowing(GameObject obj);

	private const float kIdealThrowElevation = 1.937362f;

	private const string kLogicalEffectName = "Throwable sequence";

	protected GameObject objectToThrow;

	protected Transform attachNode;

	protected OnDoneThrowing DoneThrowing;

	protected bool saveKinematicMode;

	protected bool saveUsesGravity;

	private Vector3 initialObjectLocalPos;

	private bool objectSquirming;

	private float originalObjectElevation;

	private float throwForce = 20f;

	private float throwForceY = -300f;

	private readonly Vector3 kDefaultAttachOffset = new Vector3(0f, -0.4f, 0f);

	private EffectSequence pickupSequence;

	private float pickupAnimationLength;

	protected State currentState;

	protected bool attached;

	protected bool throwing;

	protected float throwStartTime;

	void IAnimTagListener.OnMoveStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMoveEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnChainAttackAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionEnableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnCollisionDisableAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnPinballStartAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnPinballEndAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnMultishotInfoAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnTriggerEffectAnimTag(AnimationEvent evt)
	{
	}

	void IAnimTagListener.OnProjectileFireAnimTag(AnimationEvent evt)
	{
		if (currentState == State.Throwing)
		{
			LetGo();
		}
	}

	public bool Initialize(GameObject objToThrow, OnDoneThrowing doneCallback)
	{
		if (animationComponent["pickup"] == null)
		{
			CspUtils.DebugLog(owningObject.name + " does not have a pickup animation");
			return false;
		}
		CombatController component = Utils.GetComponent<CombatController>(owningObject, Utils.SearchChildren);
		if (component == null)
		{
			CspUtils.DebugLog("Cannot find CombatController for character.");
			return false;
		}
		attachNode = Utils.FindNodeInChildren(owningObject.transform, charGlobals.characterController.pickupBone);
		if (attachNode == null)
		{
			CspUtils.DebugLog("Cannot find attach node in BehaviorThrowSequence");
			return false;
		}
		pickupAnimationLength = animationComponent["pickup"].length;
		animationComponent.Play("pickup");
		pickupSequence = charGlobals.effectsList.PlaySequence("pickup_sequence");
		objectToThrow = objToThrow;
		VOManager.Instance.PlayVO("troublebot_throw", owningObject);
		DoneThrowing = doneCallback;
		currentState = State.Attaching;
		attached = false;
		throwing = false;
		return true;
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!(objectToThrow != null))
		{
			return;
		}
		switch (currentState)
		{
		case State.Attaching:
			if (!attached)
			{
				objectToThrow.transform.parent = attachNode.transform;
				Vector3 position = objectToThrow.transform.position;
				originalObjectElevation = position.y;
				if (objectToThrow.renderer != null)
				{
				}
				Quaternion rotation = objectToThrow.transform.rotation;
				objectToThrow.transform.parent = attachNode.transform;
				objectToThrow.transform.rotation = rotation;
				initialObjectLocalPos = objectToThrow.transform.localPosition;
				attached = true;
			}
			if (attached && elapsedTime > pickupAnimationLength)
			{
				currentState = State.Throwing;
			}
			if (elapsedTime > 0.5f)
			{
				TroubleBotActivityObject component = objectToThrow.GetComponent<TroubleBotActivityObject>();
				Vector3 to = (!(component != null)) ? kDefaultAttachOffset : component.attachOffset;
				objectToThrow.transform.localPosition = Vector3.Lerp(initialObjectLocalPos, to, (elapsedTime - 0.5f) / ((pickupAnimationLength - 0.5f) / 2f));
			}
			if (!objectSquirming && elapsedTime > pickupAnimationLength / 4f + 0.2f)
			{
				objectSquirming = true;
				Animation component2 = Utils.GetComponent<Animation>(objectToThrow, Utils.SearchChildren);
				if (component2 != null && component2.GetClip("throw_idle") != null)
				{
					component2.Play("throw_idle");
					component2.wrapMode = WrapMode.Loop;
				}
				TroubleBotLaserNode.DestroyLaser(objectToThrow);
				ShsAudioSource[] components = Utils.GetComponents<ShsAudioSource>(objectToThrow, Utils.SearchChildren, true);
				foreach (ShsAudioSource obj in components)
				{
					UnityEngine.Object.Destroy(obj);
				}
			}
			break;
		case State.Throwing:
			if (!throwing)
			{
				if (objectToThrow.rigidbody != null)
				{
					saveKinematicMode = objectToThrow.rigidbody.isKinematic;
					saveUsesGravity = objectToThrow.rigidbody.useGravity;
					objectToThrow.rigidbody.isKinematic = true;
					objectToThrow.rigidbody.useGravity = false;
					objectToThrow.active = false;
					objectToThrow.active = true;
				}
				if (animationComponent["pickup_throw"] != null)
				{
					animationComponent.Play("pickup_throw");
					throwing = true;
					throwStartTime = elapsedTime;
					PlayEffect();
				}
			}
			else if (animationComponent["pickup_throw"] == null || elapsedTime - throwStartTime > animationComponent["pickup_throw"].length)
			{
				currentState = State.Done;
			}
			break;
		case State.Done:
			if (DoneThrowing != null)
			{
				DoneThrowing(objectToThrow);
			}
			break;
		}
	}

	public override void behaviorEnd()
	{
		if (objectToThrow != null && currentState != State.Done)
		{
			UnityEngine.Object.Destroy(objectToThrow);
		}
		if (pickupSequence != null)
		{
			UnityEngine.Object.Destroy(pickupSequence.gameObject);
		}
		objectToThrow = null;
		attachNode = null;
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return currentState == State.Done;
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override void behaviorCancel()
	{
		detachObject();
	}

	protected void detachObject()
	{
		if (objectToThrow != null && objectToThrow.rigidbody != null)
		{
			objectToThrow.rigidbody.isKinematic = saveKinematicMode;
			objectToThrow.rigidbody.useGravity = saveUsesGravity;
		}
	}

	private void LetGo()
	{
		objectToThrow.transform.parent = null;
		if (objectToThrow.rigidbody != null)
		{
			objectToThrow.rigidbody.isKinematic = false;
			objectToThrow.rigidbody.useGravity = true;
			objectToThrow.active = false;
			objectToThrow.active = true;
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				Vector3 force = localPlayer.transform.TransformDirection(Vector3.forward).normalized * throwForce;
				Collider[] components = Utils.GetComponents<Collider>(objectToThrow, Utils.SearchChildren);
				Collider[] array = components;
				foreach (Collider collider in array)
				{
					Collider[] components2 = Utils.GetComponents<Collider>(localPlayer, Utils.SearchChildren);
					Collider[] array2 = components2;
					foreach (Collider collider2 in array2)
					{
						Physics.IgnoreCollision(collider, collider2);
					}
				}
				float num = throwForceY;
				Vector3 position = objectToThrow.transform.position;
				float y = num + (position.y - originalObjectElevation - 1.937362f) * throwForceY * 2f;
				objectToThrow.rigidbody.AddForce(force, ForceMode.Impulse);
				objectToThrow.rigidbody.AddForce(new Vector3(0f, y, 0f), ForceMode.Force);
			}
		}
		EnableWorldCollision();
	}

	private void EnableWorldCollision()
	{
		for (int i = 0; i < objectToThrow.transform.childCount; i++)
		{
			Transform child = objectToThrow.transform.GetChild(i);
			if (child.gameObject.name == "OnWorldCollision")
			{
				Collider component = Utils.GetComponent<Collider>(child);
				if (component != null)
				{
					component.isTrigger = false;
				}
			}
		}
	}

	private void PlayEffect()
	{
		EffectSequence emoteSequence = charGlobals.effectsList.GetLogicalEffectSequence("Throwable sequence");
		if (emoteSequence == null)
		{
			GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName("Throwable sequence") as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				emoteSequence = (gameObject2.GetComponent(typeof(EffectSequence)) as EffectSequence);
			}
		}
		if (!(emoteSequence == null))
		{
			emoteSequence.SetParent(charGlobals.gameObject);
			emoteSequence.AssignCreator(charGlobals);
			emoteSequence.Initialize(null, delegate
			{
				UnityEngine.Object.Destroy(emoteSequence.gameObject);
			}, null);
			emoteSequence.StartSequence();
		}
	}
}
