using System;
using UnityEngine;

public class HedgeDropActivityObject : ActivityObject
{
	public enum DropType
	{
		Coin,
		Ticket,
		Star
	}

	public DropType reward;

	public int rewardCount = 1;

	public GameObject soundEffectPrefab;

	protected SpinAndBounce bouncer;

	private float collisionRadiusSqr = -1f;

	private GameObject player;

	protected override void Start()
	{
		base.Start();
		bouncer = Utils.GetComponent<SpinAndBounce>(base.gameObject, Utils.SearchChildren);
		InteractiveObject component = Utils.GetComponent<InteractiveObject>(base.gameObject);
		collisionRadiusSqr = component.maxInteractRange * component.maxInteractRange;
		player = GameController.GetController().LocalPlayer;
	}

	protected override void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		base.OnDisable();
	}

	private void OnLocalPlayerChanged(LocalPlayerChangedMessage e)
	{
		player = e.localPlayer;
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (bouncer != null && bouncer.Bounces == 0)
		{
			SpinAndBounce spinAndBounce = bouncer;
			spinAndBounce.OnBounced = (SpinAndBounce.BouncedCallback)Delegate.Combine(spinAndBounce.OnBounced, new SpinAndBounce.BouncedCallback(Collect));
		}
		else
		{
			Collect();
		}
	}

	public void Update()
	{
		if (collisionRadiusSqr > 0f && player != null && (player.transform.position - base.gameObject.transform.position).sqrMagnitude <= collisionRadiusSqr && bouncer != null && bouncer.Bounces > 0)
		{
			collisionRadiusSqr = -1f;
			MagneticActivityObjectTriggerAdapter magneticActivityObjectTriggerAdapter = Utils.AddComponent<MagneticActivityObjectTriggerAdapter>(base.gameObject);
			magneticActivityObjectTriggerAdapter.Triggered();
		}
	}

	protected void Collect()
	{
		PlaySFX();
		if (reward == DropType.Coin)
		{
			GameObject g = new GameObject("Coin Awarder");
			TokenAwarder tokenAwarder = Utils.AddComponent<TokenAwarder>(g);
			tokenAwarder.coinsToAward = rewardCount;
		}
		else if (reward == DropType.Ticket)
		{
			AppShell.Instance.EventReporter.ReportAwardTokens(rewardCount);
			NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
		}
		else if (reward == DropType.Star && AppShell.Instance.Profile != null)
		{
			AppShell.Instance.Profile.AddStars(rewardCount);
		}
		spawner.activityReference.UnRegisterActivityObject(this);
		Despawn();
	}

	protected void PlaySFX()
	{
		if (soundEffectPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(soundEffectPrefab, base.transform.position, Quaternion.identity) as GameObject;
			gameObject.AddComponent<SuicideOnStop>();
		}
	}
}
