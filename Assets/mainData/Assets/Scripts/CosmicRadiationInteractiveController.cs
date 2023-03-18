using UnityEngine;

public class CosmicRadiationInteractiveController : OperateDeviceInteractiveObjectController
{
	public EffectSequence effectSequence;

	public GameObject parentObject;

	public float duration = 30f;

	public ShsAudioSource soundEffect;

	public override bool CanPlayerUse(GameObject player)
	{
		InvisibleWomanFadeController component = Utils.GetComponent<InvisibleWomanFadeController>(player);
		return component == null || !component.IsInvisible();
	}

	public override bool ShouldIgnoreMouseClick(GameObject player)
	{
		InvisibleWomanFadeController component = Utils.GetComponent<InvisibleWomanFadeController>(player);
		if (component != null && component.IsInvisible())
		{
			return true;
		}
		return base.ShouldIgnoreMouseClick(player);
	}

	protected override void ApproachArrived(GameObject obj)
	{
		base.ApproachArrived(obj);
		InvisibleWomanFadeController invisibleWomanFadeController = Utils.GetComponent<InvisibleWomanFadeController>(obj);
		if (invisibleWomanFadeController == null)
		{
			invisibleWomanFadeController = obj.AddComponent<InvisibleWomanFadeController>();
		}
		if (!invisibleWomanFadeController.IsInvisible())
		{
			if (this.effectSequence != null && parentObject != null)
			{
				EffectSequence effectSequence = Object.Instantiate(this.effectSequence) as EffectSequence;
				effectSequence.Initialize(parentObject, null, null);
				effectSequence.StartSequence();
			}
			if (soundEffect != null)
			{
				ShsAudioSource.PlayAutoSound(soundEffect.gameObject, obj.transform);
			}
			BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("Player should always have a BehaviorManager");
				return;
			}
			behaviorManager.requestChangeBehavior<BehaviorWait>(false);
			AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "baxter_invisible", string.Empty, 2f);
		}
	}

	protected override void AnimationComplete(GameObject obj)
	{
		base.AnimationComplete(obj);
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager != null)
		{
			behaviorManager.endBehavior();
		}
		InvisibleWomanFadeController component = Utils.GetComponent<InvisibleWomanFadeController>(obj);
		if (component != null && !component.IsInvisible())
		{
			component.autoFadeInDelay = duration;
			component.Fade(duration);
		}
	}
}
