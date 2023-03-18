using UnityEngine;

public class SurvivalModeObject : GlowableInteractiveController
{
	public class Use : BaseUse
	{
		private SurvivalModeObject owner;

		public Use(GameObject player, GlowableInteractiveController owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = (owner as SurvivalModeObject);
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
				SHSSocialSurvivalModeLeaderboardSelector dialogWindow = new SHSSocialSurvivalModeLeaderboardSelector();
				GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
			}
			base.OnApproachArrived(player);
		}
	}

	public float approachDistance = 3f;

	public float fidgitDelay = 30f;

	public EffectSequence turnInSequence;

	public bool InUse;

	public GameObject modelGameObject;

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
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
		}
		SHSBrawlerSurvivalLeaderboards dialogWindow = new SHSBrawlerSurvivalLeaderboards();
		GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Default);
	}

	private void DestroyTurnInSequence(EffectSequence instance)
	{
		Object.Destroy(instance.gameObject);
	}
}
