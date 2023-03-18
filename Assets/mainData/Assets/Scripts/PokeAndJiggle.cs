using System.Collections;
using UnityEngine;

public class PokeAndJiggle : InteractiveObjectController
{
	protected class PlayerBlob
	{
		protected PokeAndJiggle owner;

		protected GameObject player;

		protected OnDone onDone;

		protected OnDone onCancel;

		protected bool highlightOnProximity;

		protected bool highlightOnHover;

		public PlayerBlob(PokeAndJiggle owner, GameObject player, OnDone onDone, OnDone onCancel)
		{
			this.owner = owner;
			this.player = player;
			this.onDone = onDone;
			this.onCancel = onCancel;
			if (owner.owner != null)
			{
				highlightOnProximity = owner.owner.highlightOnProximity;
				highlightOnHover = owner.owner.highlightOnHover;
			}
		}

		public bool Start()
		{
			if (!owner.readyToPoke)
			{
				return false;
			}
			if (owner.animationComponent == null)
			{
				owner.animationComponent = owner.GetComponentInChildren<Animation>();
			}
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (component == null)
			{
				CspUtils.DebugLog("Player attempted to poke an interactive object, but the player does not have an attached BehaviorManager");
				return false;
			}
			BehaviorApproach behaviorApproach = component.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				CspUtils.DebugLog("Could not change player behavior to BehaviorApproach");
				return false;
			}
			Vector3 approachPoint = GetApproachPoint(player);
			Quaternion rotation = Quaternion.LookRotation(owner.transform.position - approachPoint);
			behaviorApproach.Initialize(approachPoint, rotation, true, ApproachArrived, ApproachCancelled, 0f, 2f, true, false);
			owner.readyToPoke = false;
			return true;
		}

		private Vector3 GetApproachPoint(GameObject player)
		{
			DockPoint[] components = Utils.GetComponents<DockPoint>(owner.owner, Utils.SearchChildren);
			if (components.Length > 0)
			{
				DockPoint dockPoint = null;
				float num = float.MaxValue;
				DockPoint[] array = components;
				foreach (DockPoint dockPoint2 in array)
				{
					float sqrMagnitude = (dockPoint2.transform.position - player.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						dockPoint = dockPoint2;
					}
				}
				return dockPoint.transform.position;
			}
			Vector3 vector = owner.transform.position - player.transform.position;
			return owner.transform.position - vector.normalized * owner.pokeDistance;
		}

		private void ApproachArrived(GameObject player)
		{
			Poke(player);
			PlayVO(player);
		}

		private void ApproachCancelled(GameObject player)
		{
			owner.readyToPoke = true;
			if (onCancel != null)
			{
				onCancel(player, CompletionStateEnum.Canceled);
			}
		}

		private void Poke(GameObject player)
		{
			if (player == GameController.GetController().LocalPlayer)
			{
				AppShell.Instance.EventMgr.Fire(this, new InteractiveObjectUsedMessage(owner.owner));
			}
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (owner.doPoke)
			{
				BehaviorEmote behaviorEmote = component.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
				if (behaviorEmote != null)
				{
					behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("poke").id, false, OnPokeFinished);
					owner.StartCoroutine(DelayedPokeResponse(player.animation.GetClip("emote_poke").length / 2f));
					return;
				}
				CspUtils.DebugLog("Could not switch to emote (poke) behavior");
				owner.readyToPoke = true;
				if (onDone != null)
				{
					onDone(player, CompletionStateEnum.Success);
				}
			}
			else
			{
				owner.StartCoroutine(DelayedPokeResponse(0f));
			}
		}

		private IEnumerator DelayedPokeResponse(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			if (owner.owner != null)
			{
				owner.owner.highlightOnProximity = false;
				owner.owner.highlightOnHover = false;
				owner.owner.GotoBestState();
			}
			if (owner.animationComponent != null && !string.IsNullOrEmpty(owner.animationToPlay))
			{
				owner.animationComponent.Rewind();
				owner.animationComponent.Sample();
				owner.animationComponent[owner.animationToPlay].wrapMode = WrapMode.Once;
				owner.animationComponent.Play(owner.animationToPlay);
			}
			if (owner.sound != null)
			{
				ShsAudioSource.PlayAutoSound(owner.sound, owner.transform);
			}
			if (owner.effect != null)
			{
				if (owner.fxInstance != null)
				{
					Utils.GetComponent<EffectSequence>(owner.fxInstance).Cancel();
				}
				owner.fxInstance = (Object.Instantiate(owner.effect.gameObject, Vector3.zero, Quaternion.identity) as GameObject);
				EffectSequence fxSequence = Utils.GetComponent<EffectSequence>(owner.fxInstance);
				if (!fxSequence.Initialized)
				{
					fxSequence.Initialize(owner.gameObject, null, null);
				}
				fxSequence.StartSequence();
			}
			if (player == GameController.GetController().LocalPlayer)
			{
				owner.DropLoot(false);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
			if (owner.animationComponent != null)
			{
				if (!string.IsNullOrEmpty(owner.animationToPlay))
				{
					yield return new WaitForSeconds(owner.animationComponent[owner.animationToPlay].length);
					owner.animationComponent.Stop();
					owner.animationComponent.Rewind();
					owner.animationComponent.Sample();
				}
				if (owner.owner != null)
				{
					owner.owner.highlightOnProximity = highlightOnProximity;
					owner.owner.highlightOnHover = highlightOnHover;
					owner.owner.GotoBestState();
				}
			}
			owner.readyToPoke = true;
		}

		protected void OnPokeFinished(GameObject player)
		{
			if (player != null)
			{
				owner.StartCoroutine(PlayEmote(player.GetComponent<CharacterGlobals>()));
			}
		}

		protected IEnumerator PlayEmote(CharacterGlobals player)
		{
			yield return new WaitForSeconds(1f);
			if (!string.IsNullOrEmpty(owner.emoteOnFinish) && player != null)
			{
				BehaviorEmote emote = player.behaviorManager.requestChangeBehavior<BehaviorEmote>(false);
				emote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(owner.emoteOnFinish).id, true, null);
			}
		}

		protected void PlayVO(GameObject player)
		{
			VOManager.Instance.PlayVO("use_object", player);
		}
	}

	private const float kCooldownPeriod = 60f;

	public string animationToPlay = "Take 001";

	public GameObject sound;

	public EffectSequence effect;

	public bool doPoke = true;

	public float pokeDistance = 2f;

	public float starProbability = 0.5f;

	public Animation animationComponent;

	public CoinsTriggerAdaptor coinTrigger;

	public string emoteOnFinish = string.Empty;

	private GameObject fxInstance;

	private bool readyToPoke = true;

	private float nextLootTime;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, onDone, onDone);
		return playerBlob.Start();
	}

	public void DropLoot(bool viaPowerEmote)
	{
		if ((viaPowerEmote && owner.GetLastPowerEmotePlayer() != GameController.GetController().LocalPlayer) || Time.time < nextLootTime || starProbability <= 0f)
		{
			return;
		}
		nextLootTime = Time.time + 60f;
		if (coinTrigger != null)
		{
			coinTrigger.Triggered();
			return;
		}
		ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(this, Utils.SearchChildren);
		if (component == null)
		{
			CspUtils.DebugLog("No activity spawner or CoinsTriggerAdaptor found on hedge object -- No loot can be dropped");
		}
		else if (Random.value <= starProbability)
		{
			component.activityPrefab = "HedgeStarPrefab";
			component.SpawnActivityObject();
		}
	}
}
