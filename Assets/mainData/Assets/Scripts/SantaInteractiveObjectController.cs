using UnityEngine;

public class SantaInteractiveObjectController : EffectSequenceController
{
	public class Use : BaseUse
	{
		private SantaInteractiveObjectController owner;

		private GameObject foodInstance;

		public Use(GameObject player, GlowableInteractiveController owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = (owner as SantaInteractiveObjectController);
		}

		public override bool Start()
		{
			if (base.Start())
			{
				Approach(owner.approachTarget ?? owner.transform);
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject player)
		{
			ChangeBehavior<BehaviorWait>(false);
			PlayGiveSequence();
		}

		protected void PlayGiveSequence()
		{
			owner.SequencesEnabled = false;
			EffectSequence effectSequence = InstantiateGiveSequence();
			effectSequence.Initialize(owner.SequenceOwner, OnGiveSequenceFinished, OnGiveSequenceEvent);
			if (!effectSequence.AutoStart)
			{
				effectSequence.StartSequence();
			}
		}

		protected EffectSequence InstantiateGiveSequence()
		{
			EffectSequence sequence = owner.giveSequence.GetSequence(owner.SequenceOwner);
			GameObject gameObject = Object.Instantiate(sequence.gameObject) as GameObject;
			return gameObject.GetComponent<EffectSequence>();
		}

		protected void InstantiateFood()
		{
			Transform transform = Utils.FindNodeInChildren(owner.SequenceOwner.transform, owner.giveNodeName);
			foodInstance = (Object.Instantiate(owner.objectToGive) as GameObject);
			Utils.AttachGameObject(transform.gameObject, foodInstance);
			foodInstance.transform.localPosition = Vector3.zero;
			foodInstance.transform.localRotation = Quaternion.identity;
		}

		protected void OnGiveSequenceEvent(EffectSequence seq, EventEffect effect)
		{
			if (effect.EventName == owner.objectSpawnEventName)
			{
				InstantiateFood();
			}
			else if (effect.EventName == owner.objectGiveEventName)
			{
				ChangeBehavior<BehaviorEat>(false).Initialize(foodInstance, true, OnEaten);
			}
			else
			{
				CspUtils.DebugLog("Received event during <" + seq.name + "> that was not recognized.  Santa responds to the \"" + owner.objectSpawnEventName + "\" and \"" + owner.objectGiveEventName + "\" events.");
			}
		}

		protected void OnEaten(GameObject objInteractedWith)
		{
			if (foodInstance != null)
			{
				Object.Destroy(foodInstance);
			}
		}

		protected void OnGiveSequenceFinished(EffectSequence seq)
		{
			Object.Destroy(seq.gameObject);
			owner.SequencesEnabled = true;
			Done();
		}
	}

	public Transform approachTarget;

	public EffectSequenceReference giveSequence;

	public GameObject objectToGive;

	public string giveNodeName = "fx_Rpalm";

	public string objectSpawnEventName = "spawn";

	public string objectGiveEventName = "give";

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		Use use = new Use(player, this, onDone);
		return use.Start();
	}
}
