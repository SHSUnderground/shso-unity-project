using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public FRange periodicEmoteDelay = new FRange(10f, 45f);

	protected StatusBubble bubble;

	protected PlayerStatusTriggerProxy triggerProxy;

	private HairTrafficController htc;

	protected bool rollover;

	protected bool inProximity;

	[CompilerGenerated]
	private PlayerStatusDefinition.Status _003CStatus_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsDefaultStatus_003Ek__BackingField;

	public PlayerStatusDefinition.Status Status
	{
		[CompilerGenerated]
		get
		{
			return _003CStatus_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CStatus_003Ek__BackingField = value;
		}
	}

	public bool IsDefaultStatus
	{
		[CompilerGenerated]
		get
		{
			return _003CIsDefaultStatus_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CIsDefaultStatus_003Ek__BackingField = value;
		}
	}

	public HairTrafficController Htc
	{
		set
		{
			htc = value;
		}
	}

	public bool Rollover
	{
		get
		{
			return rollover;
		}
		set
		{
			rollover = value;
			CheckShowStatus();
		}
	}

	public bool InProximity
	{
		get
		{
			return inProximity;
		}
		set
		{
			inProximity = value;
			CheckShowStatus();
		}
	}

	public void SetStatus(PlayerStatusDefinition.Status status, bool localOnly)
	{
		Status = status;
		bool isDefaultStatus = IsDefaultStatus;
		IsDefaultStatus = (Status.name == PlayerStatusDefinition.DefaultStatusName);
		CheckShowStatus();
		CheckTriggerProxy();
		if (isDefaultStatus && !IsDefaultStatus)
		{
			StartCoroutine(EmotePeriodically());
		}
		else if (!isDefaultStatus && IsDefaultStatus)
		{
			StopAllCoroutines();
		}
		if (!localOnly)
		{
			NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.gameObject);
			if (component != null && component.IsOwner())
			{
				component.QueueNetAction(new NetActionPlayerStatus(Status));
			}
		}
	}

	public void SetStatus(PlayerStatusDefinition.Status status)
	{
		SetStatus(status, false);
	}

	public void ShowStatus()
	{
		if (bubble == null)
		{
			Object @object = Resources.Load("GUI/3D/StatusBubblePrefab");
			if (@object != null)
			{
				GameObject gameObject = Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = new Vector3(0f, 3f, 0f);
				bubble = Utils.GetComponent<StatusBubble>(gameObject);
			}
		}
		if (bubble != null)
		{
			bubble.SetIconTextureByName(Status.icon);
			bubble.FadeIn();
		}
	}

	public void HideStatus()
	{
		if (bubble != null)
		{
			bubble.FadeOut();
		}
		htc.ToggleBillboard(true);
	}

	private void CheckShowStatus()
	{
		if (!IsDefaultStatus && (InProximity || Rollover || base.gameObject == GameController.GetController().LocalPlayer))
		{
			ShowStatus();
		}
		else
		{
			HideStatus();
		}
	}

	private void CheckTriggerProxy()
	{
		if (base.gameObject != GameController.GetController().LocalPlayer)
		{
			if (triggerProxy == null)
			{
				triggerProxy = PlayerStatusTriggerProxy.AttachProxyCollider(this);
			}
			Utils.ActivateTree(triggerProxy.gameObject, !IsDefaultStatus);
		}
	}

	private IEnumerator EmotePeriodically()
	{
		BehaviorManager behaviorMan = Utils.GetComponent<BehaviorManager>(base.gameObject);
		sbyte emoteID = EmotesDefinition.Instance.GetEmoteByCommand(Status.emote).id;
		while (behaviorMan != null)
		{
			yield return new WaitForSeconds(periodicEmoteDelay.RandomValue);
			if (behaviorMan != null)
			{
				BehaviorEmote emote = behaviorMan.requestChangeBehavior<BehaviorEmote>(false);
				if (emote != null)
				{
					emote.Initialize(emoteID, true);
				}
			}
		}
	}

	private void Awake()
	{
		Status = PlayerStatusDefinition.Instance.GetStatus(PlayerStatusDefinition.DefaultStatusName);
		IsDefaultStatus = true;
	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnUserSpawned);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnUserSpawned);
	}

	private void OnUserSpawned(EntitySpawnMessage e)
	{
		if ((e.spawnType & (CharacterSpawn.Type.Remote | CharacterSpawn.Type.Player)) != 0)
		{
			SetStatus(Status);
		}
	}

	private void OnMouseRolloverEnter()
	{
		Rollover = true;
	}

	private void OnMouseRolloverExit()
	{
		Rollover = false;
	}

	public static PlayerStatus SetStatus(GameObject player, PlayerStatusDefinition.Status status, bool localOnly)
	{
		PlayerStatus playerStatus = Utils.GetComponent<PlayerStatus>(player);
		if (playerStatus == null)
		{
			playerStatus = Utils.AddComponent<PlayerStatus>(player);
		}
		HairTrafficController component = player.GetComponent<HairTrafficController>();
		if (component != null)
		{
			playerStatus.Htc = component;
			component.BeginningStatus(playerStatus);
		}
		playerStatus.SetStatus(status, localOnly);
		return playerStatus;
	}

	public static PlayerStatus SetStatus(GameObject player, PlayerStatusDefinition.Status status)
	{
		return SetStatus(player, status, false);
	}

	public static PlayerStatus SetStatus(CharacterGlobals player, PlayerStatusDefinition.Status status, bool localOnly)
	{
		return SetStatus(player.gameObject, status, localOnly);
	}

	public static PlayerStatus SetStatus(CharacterGlobals player, PlayerStatusDefinition.Status status)
	{
		return SetStatus(player, status, false);
	}

	public static PlayerStatus SetLocalStatus(PlayerStatusDefinition.Status status)
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			return SetStatus(localPlayer, status);
		}
		return null;
	}

	public static PlayerStatus ClearLocalStatus()
	{
		return SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus(PlayerStatusDefinition.DefaultStatusName));
	}

	public static PlayerStatusDefinition.Status GetStatus(GameObject player)
	{
		PlayerStatus component = Utils.GetComponent<PlayerStatus>(player);
		if (component == null)
		{
			return PlayerStatusDefinition.Instance.GetStatus(PlayerStatusDefinition.DefaultStatusName);
		}
		return component.Status;
	}

	public static PlayerStatusDefinition.Status GetLocalStatus()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			return GetStatus(localPlayer);
		}
		return null;
	}
}
