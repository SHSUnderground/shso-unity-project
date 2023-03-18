using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Boss Description")]
internal class CutSceneBossDescriptionEvent : CutSceneTargetEvent
{
	public override void StartEvent()
	{
		base.StartEvent();
		if (base.Target != null)
		{
			BossAIControllerBrawler component = base.Target.GetComponent<BossAIControllerBrawler>();
			if (component != null)
			{
				component.DontWaitForBossName();
			}
			else
			{
				CspUtils.DebugLog("Boss Description target <" + base.Target.name + "> is not a boss");
			}
		}
	}
}
