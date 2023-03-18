using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Splicer")]
public class CutSceneSplicer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string cutSceneTrigger;

	protected bool subscribed;

	public void Start()
	{
		if (string.IsNullOrEmpty(cutSceneTrigger))
		{
			CspUtils.DebugLog("CutSceneSplicer::Start() - cut scene trigger string is not set");
			return;
		}
		if (BrawlerController.Instance == null)
		{
			CspUtils.DebugLog("CutSceneSplicer::Start() - brawler controller does not exist");
			return;
		}
		if (BrawlerController.Instance.StartTransaction == null)
		{
			CspUtils.DebugLog("CutSceneSplicer::Start() - start transaction does not exist");
			return;
		}
		if (ScenarioEventManager.Instance == null)
		{
			CspUtils.DebugLog("CutSceneSplicer::Start() - scenario event manager does not exist");
			return;
		}
		ScenarioEventManager.Instance.SubscribeScenarioEvent(cutSceneTrigger, CutSceneTriggered);
		subscribed = true;
		BrawlerController.Instance.StartTransaction.AddStep("cut_scene_splicer");
	}

	public void OnDisable()
	{
		if (subscribed && ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(cutSceneTrigger, CutSceneTriggered);
		}
		subscribed = false;
	}

	public void CutSceneTriggered(string eventName)
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.StartTransaction != null)
		{
			BrawlerController.Instance.StartTransaction.CompleteStep("cut_scene_splicer");
		}
	}
}
