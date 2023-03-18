using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RCBot : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void DespawnFinishedDelegate(RCBot bot);

	public string destructionEffectName = "bot_destroy_sequence";

	public float failsafeDestructTimeSeconds = 300f;

	public string selfDestructEmote = "rude";

	private CharacterGlobals _characterGlobals;

	private NetworkComponent _networkComponent;

	[CompilerGenerated]
	private int _003COriginalOwner_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CDespawning_003Ek__BackingField;

	public int OriginalOwner
	{
		[CompilerGenerated]
		get
		{
			return _003COriginalOwner_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003COriginalOwner_003Ek__BackingField = value;
		}
	}

	public bool Despawning
	{
		[CompilerGenerated]
		get
		{
			return _003CDespawning_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CDespawning_003Ek__BackingField = value;
		}
	}

	public CharacterGlobals CharacterGlobals
	{
		get
		{
			if (_characterGlobals == null)
			{
				_characterGlobals = base.gameObject.GetComponent<CharacterGlobals>();
			}
			return _characterGlobals;
		}
	}

	public NetworkComponent NetworkComponent
	{
		get
		{
			if (CharacterGlobals != null)
			{
				return CharacterGlobals.networkComponent;
			}
			if (_networkComponent == null)
			{
				_networkComponent = base.gameObject.GetComponent<NetworkComponent>();
			}
			return _networkComponent;
		}
	}

	public RCBot()
	{
		Despawning = false;
	}

	private void Start()
	{
		base.gameObject.GetComponent<HairTrafficController>().ToggleBillboard(false);
		Utils.SetLayerTreeFiltered(base.gameObject, 2, 1077760);
		AppShell.Instance.ServerConnection.Game.ForEachNetEntity(delegate(NetGameManager.NetEntity e)
		{
			Physics.IgnoreCollision(base.gameObject.collider, e.netComp.gameObject.collider);
		});
		base.gameObject.AddComponent<TimedSelfDestruct>().lifetime = failsafeDestructTimeSeconds;
		StartCoroutine(AcquireOriginalOwner());
	}

	public void TakeControlLocally(GameObject player, DespawnFinishedDelegate onSelfDestruct)
	{
		PlayerInputController playerInputController = base.gameObject.AddComponent<PlayerInputController>();
		playerInputController.registerAsLocalPlayerOnStart = false;
		playerInputController.allowObjectInteraction = false;
		playerInputController.sendRolloverEvents = false;
		RCBotEmoteResponder rCBotEmoteResponder = base.gameObject.AddComponent<RCBotEmoteResponder>();
		EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand(selfDestructEmote);
		if (emoteByCommand != null)
		{
			rCBotEmoteResponder.EmoteDelegates[emoteByCommand.id] = delegate
			{
				RelinquishLocalControl(player, onSelfDestruct);
			};
		}
		MoveCamera(player, base.gameObject);
	}

	public void RelinquishLocalControl(GameObject player, DespawnFinishedDelegate onFinished)
	{
		if (!Despawning)
		{
			Despawning = true;
			base.gameObject.GetComponent<PlayerInputController>().AllowInput = false;
			CharacterGlobals.motionController.stopGently();
			CharacterGlobals.behaviorManager.getBehavior().destinationChanged();
			CharacterGlobals.behaviorManager.requestChangeBehavior<BehaviorWait>(false);
			EffectSequence x = CharacterGlobals.effectsList.PlaySequence(destructionEffectName, delegate(EffectSequence seq)
			{
				OnDestructionEffectFinished(seq, player, onFinished);
			});
			if (x == null)
			{
				OnDestructionEffectFinished(null, player, onFinished);
			}
		}
	}

	public void Despawn()
	{
		SpawnData component = base.gameObject.GetComponent<SpawnData>();
		if (component != null)
		{
			component.Despawn(EntityDespawnMessage.despawnType.effect);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnBecomeOwner()
	{
		if (OriginalOwner >= 0 && NetworkComponent.NetOwnerId != OriginalOwner)
		{
			Despawn();
		}
	}

	private void MoveCamera(GameObject oldTarget, GameObject newTarget)
	{
		CameraTargetHelper componentInChildren = oldTarget.GetComponentInChildren<CameraTargetHelper>();
		if (componentInChildren != null)
		{
			componentInChildren.SetTarget(newTarget.transform);
		}
		PlayerOcclusionDetector.Instance.myPlayer = newTarget;
	}

	private void OnDestructionEffectFinished(EffectSequence seq, GameObject player, DespawnFinishedDelegate onFinished)
	{
		MoveCamera(base.gameObject, player);
		if (seq != null)
		{
			Object.Destroy(seq.gameObject);
		}
		CharacterGlobals.GetComponent<PlayerInputController>().FireDestructionMessage();
		CharacterGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.effect);
		if (onFinished != null)
		{
			onFinished(this);
		}
	}

	private IEnumerator AcquireOriginalOwner()
	{
		OriginalOwner = -1;
		float timeout = Time.time + 10f;
		while (Time.time < timeout && OriginalOwner < 0)
		{
			if (NetworkComponent.NetOwnerId >= 0)
			{
				OriginalOwner = NetworkComponent.NetOwnerId;
			}
			yield return 0;
		}
	}

	private void CollectNetworkState(Hashtable extraData)
	{
		extraData["remote_spawner"] = "RCBotSpawner";
	}
}
