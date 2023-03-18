using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorCelebrate : BehaviorBase
{
	private bool isFinished;

	private List<GameObject> effectsCreated;

	private float kTimeoutDurationSeconds = 60f;

	private float timeout = -1f;

	protected bool sendNetMessages;

	protected GameObject cameraObject;

	protected bool PlayerBillboardVisible
	{
		set
		{
			HairTrafficController component = owningObject.GetComponent<HairTrafficController>();
			if (component != null)
			{
				component.ToggleBillboard(value);
			}
		}
	}

	public virtual void Initialize(bool sendNetMessages)
	{
		this.sendNetMessages = sendNetMessages;
	}

	public override void behaviorBegin()
	{
		if (networkComponent != null && networkComponent.IsOwnedBySomeoneElse())
		{
			timeout = Time.time + kTimeoutDurationSeconds;
		}
		PlayerBillboardVisible = false;
		effectsCreated = new List<GameObject>();
		cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
		base.behaviorBegin();
	}

	protected void TryToFaceCamera()
	{
		if (cameraObject != null)
		{
			Vector3 vector = cameraObject.transform.position - owningObject.transform.position;
			vector.y = 0f;
			if (Vector3.Angle(owningObject.transform.forward, vector) > 5f)
			{
				Vector3 forward = Vector3.RotateTowards(owningObject.transform.forward, vector, charGlobals.motionController.rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
				owningObject.transform.rotation = Quaternion.LookRotation(forward);
			}
		}
	}

	protected void PlayEffect(string effectName)
	{
		GameObject gameObject = charGlobals.effectsList.TryGetEffectSequencePrefabByName(effectName) as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		if (gameObject2 != null)
		{
			gameObject2.name = effectName;
			EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject2);
			if (component != null)
			{
				component.SetParent(owningObject);
				component.AssignCreator(charGlobals);
				component.Initialize(null, OnSequenceDone, null);
				component.StartSequence();
				effectsCreated.Add(gameObject2);
			}
		}
	}

	public override void behaviorUpdate()
	{
		TryToFaceCamera();
		if (timeout > 0f && Time.time > timeout)
		{
			charGlobals.behaviorManager.endBehavior();
		}
		base.behaviorUpdate();
	}

	public override bool allowUserInput()
	{
		return isFinished;
	}

	public override void behaviorEnd()
	{
		for (int num = effectsCreated.Count - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(effectsCreated[num]);
		}
		PlayerBillboardVisible = true;
		base.behaviorEnd();
	}

	public override bool useMotionController()
	{
		return false;
	}

	protected abstract void OnSequenceDone(EffectSequence seq);

	protected void DestroyEffect(EffectSequence seq)
	{
		if (effectsCreated.Contains(seq.gameObject))
		{
			effectsCreated.Remove(seq.gameObject);
		}
		UnityEngine.Object.Destroy(seq.gameObject);
	}
}
