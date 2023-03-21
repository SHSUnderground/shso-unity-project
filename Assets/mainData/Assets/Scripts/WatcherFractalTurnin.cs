using System.Collections;
using UnityEngine;

public class WatcherFractalTurnin : GlowableInteractiveController
{
	public class Use : BaseUse
	{
		private WatcherFractalTurnin owner;

		public Use(GameObject player, GlowableInteractiveController owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = (owner as WatcherFractalTurnin);
		}

		public override bool Start()
		{
			if (base.Start())
			{
				Approach(owner.transform, owner.approachDistance, true);
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject player)
		{
			if (base.IsLocal && !owner.InUse)
			{
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("fractal_type_id", (int)owner.fractalType);
				AppShell.Instance.WebService.StartRequest("resources$users/get_fractal_bank.py", delegate(ShsWebResponse response)
				{
					if (response.Status == 200)
					{
						DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
						dataWarehouse.Parse();
						SHSSocialWatcherFractalTurnInConfirmation dialogWindow = new SHSSocialWatcherFractalTurnInConfirmation(owner, dataWarehouse.GetInt("bank/balance"), dataWarehouse.TryGetInt("bank/current_balance", 0) > 0);
						GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
					}
				}, wWWForm.data);
				owner.InUse = true;
			}
			base.OnApproachArrived(player);
		}
	}

	public float approachDistance = 3f;

	public float fidgitDelay = 30f;

	public FractalActivitySpawnPoint.FractalType fractalType = FractalActivitySpawnPoint.FractalType.Fractal;

	public EffectSequence turnInSequence;

	public bool InUse;

	public GameObject modelGameObject;

	private Animation animationComponent;

	private float lastFidgit;

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
		animationComponent = (base.gameObject.GetComponentInChildren(typeof(Animation)) as Animation);
		lastFidgit = Time.time;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		return new Use(player, this, onDone).Start();
	}

	public void PlayTurnInSequence()
	{
		if (turnInSequence != null)
		{
			GameObject gameObject = Object.Instantiate(turnInSequence.gameObject) as GameObject;
			gameObject.transform.position = owner.transform.position;
			gameObject.transform.rotation = owner.transform.rotation;
			gameObject.GetComponent<EffectSequence>().Initialize(modelGameObject, DestroyTurnInSequence, null);
			if (!SHSMySquadChallengeGadget.InUse)
			{
				SHSMySquadChallengeGadget dialogWindow = new SHSMySquadChallengeGadget(SHSMySquadChallengeTabStrip.TabType.Fractals);
				GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Default);
			}
		}
	}

	private void DestroyTurnInSequence(EffectSequence instance)
	{
		animationComponent.CrossFade("movement_idle_01", 0.2f);
		Object.Destroy(instance.gameObject);
	}

	private void Update()
	{
		if (Time.time - lastFidgit > fidgitDelay && !animationComponent.IsPlaying("movement_idle_03"))
		{
			StartCoroutine(PlayFidgit());
			lastFidgit = Time.time;
		}
	}

	private IEnumerator PlayFidgit()
	{
		animationComponent.CrossFade((new string[2]
		{
			"movement_idle_02",
			"interact"
		})[Random.Range(0, 2)], 0.2f);
		yield return new WaitForSeconds(3f);
		animationComponent.CrossFade("movement_idle_01", 0.5f);
		animationComponent.wrapMode = WrapMode.Loop;
	}
}
