using System.Collections;
using UnityEngine;

public class BehaviorSit : BehaviorBase
{
	public delegate void SitFinishedDelegate(BehaviorSit sit);

	public delegate void FinishedDelegate(BehaviorSit sit);

	protected bool sitting;

	protected bool standing;

	protected bool ended;

	protected bool localOnly = true;

	protected BoxCollider dummyCollider;

	protected EffectSequence idleSequence;

	protected bool playVO = true;

	public float SitAnimationLength
	{
		get
		{
			return AnimationLength("sit_down");
		}
	}

	public float StandAnimationLength
	{
		get
		{
			return AnimationLength("sit_stand");
		}
	}

	public event SitFinishedDelegate OnFinishedSitting;

	public event FinishedDelegate OnFinishedStanding;

	public void Initialize(bool localOnly)
	{
		Initialize(localOnly, true);
	}

	public void Initialize(bool localOnly, bool playVO)
	{
		this.localOnly = localOnly;
		this.playVO = playVO;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		sitting = false;
		standing = false;
		ended = false;
		if (charGlobals.motionController.SitOverride == null)
		{
			if (animationComponent["sit_down"] != null)
			{
				animationComponent["sit_down"].wrapMode = WrapMode.Once;
				animationComponent.CrossFade("sit_down");
			}
			else
			{
				charGlobals.behaviorManager.endBehavior();
			}
			idleSequence = charGlobals.effectsList.PlaySequence("emote_idle_sequence");
			charGlobals.effectsList.TryOneShot("sit_sequence");
			charGlobals.StartCoroutine(PlayVO());
			if (!localOnly && networkComponent != null && networkComponent.IsOwner())
			{
				NetActionSit action = new NetActionSit(owningObject);
				networkComponent.QueueNetAction(action);
			}
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (charGlobals.motionController.SitOverride != null)
		{
			charGlobals.behaviorManager.endBehavior();
		}
		else if (standing && !animationComponent.IsPlaying("sit_stand"))
		{
			animationComponent.Play("movement_idle", AnimationPlayMode.Stop);
			animationComponent.Sample();
			charGlobals.motionController.teleportTo(owningObject.transform.position, -1f * owningObject.transform.forward);
			charGlobals.motionController.updateLookDirection();
			charGlobals.motionController.setDestination(owningObject.transform.position);
			charGlobals.behaviorManager.endBehavior();
			if (this.OnFinishedStanding != null)
			{
				this.OnFinishedStanding(this);
				this.OnFinishedStanding = null;
			}
		}
		else if (!sitting && !animationComponent.IsPlaying("sit_down"))
		{
			sitting = true;
			charGlobals.effectsList.TryOneShot("sit_idle_sequence");
			if (animationComponent["sit_idle"] != null)
			{
				animationComponent["sit_idle"].wrapMode = WrapMode.Loop;
				animationComponent.Play("sit_idle");
			}
			if (networkComponent != null && !networkComponent.IsOwner())
			{
				AttachDummyCollider();
			}
			if (this.OnFinishedSitting != null)
			{
				this.OnFinishedSitting(this);
				this.OnFinishedSitting = null;
			}
		}
	}

	public override void behaviorLateUpdate()
	{
		if (!animationComponent.IsPlaying("sit_idle") && charGlobals.motionController.SitOverride == null)
		{
			charGlobals.motionController.performRootMotion();
		}
	}

	public override void behaviorEnd()
	{
		if (idleSequence != null)
		{
			Object.Destroy(idleSequence.gameObject);
		}
		base.behaviorEnd();
		if (charGlobals.motionController.SitOverride != null)
		{
			if (this.OnFinishedStanding != null)
			{
				this.OnFinishedStanding(this);
				this.OnFinishedStanding = null;
			}
			if (owningObject == GameController.GetController().LocalPlayer)
			{
				sbyte id = EmotesDefinition.Instance.GetEmoteByCommand(charGlobals.motionController.SitOverride).id;
				AppShell.Instance.EventMgr.Fire(owningObject, new EmoteMessage(owningObject, id));
			}
		}
	}

	public void stand()
	{
		if (sitting)
		{
			charGlobals.effectsList.TryOneShot("sit_stand_sequence");
			if (animationComponent["sit_stand"] != null)
			{
				animationComponent["sit_stand"].wrapMode = WrapMode.Once;
				animationComponent.CrossFade("sit_stand");
			}
			standing = true;
			if (dummyCollider != null)
			{
				Object.Destroy(dummyCollider);
				dummyCollider = null;
			}
		}
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public void dispatchStand()
	{
		if (networkComponent != null && networkComponent.IsOwner())
		{
			NetActionStand action = new NetActionStand();
			networkComponent.QueueNetAction(action);
		}
	}

	private void AttachDummyCollider()
	{
		Transform transform = Utils.FindNodeInChildren(owningObject.transform, "export_node");
		Bounds bounds = transform.GetComponentInChildren<Renderer>().bounds;
		BoxCollider boxCollider = owningObject.AddComponent<BoxCollider>();
		boxCollider.center = owningObject.transform.InverseTransformPoint(bounds.center);
		boxCollider.size = owningObject.transform.InverseTransformDirection(bounds.size);
		boxCollider.isTrigger = true;
		dummyCollider = boxCollider;
	}

	private float AnimationLength(string animationName)
	{
		if (animationComponent[animationName] != null)
		{
			return animationComponent[animationName].length;
		}
		return 0f;
	}

	private IEnumerator PlayVO()
	{
		yield return 0;
		if (playVO)
		{
			VOManager.Instance.PlayVO("sit_down", owningObject);
		}
	}
}
