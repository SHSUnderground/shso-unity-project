using UnityEngine;

[AddComponentMenu("ScenarioEvent/CutScene")]
public class CutScene : ScenarioEventHandlerEnableBase
{
	public string completeEvent = string.Empty;

	public CutSceneClip[] clips;

	private bool mIsPlaying;

	private float mStartTime;

	public bool IsScenePlaying
	{
		get
		{
			return mIsPlaying;
		}
	}

	public bool PlayScene()
	{
		if (IsScenePlaying)
		{
			return false;
		}
		if (clips == null || clips.Length == 0)
		{
			return false;
		}
		GameController controller = GameController.GetController();
		if (!controller)
		{
			return false;
		}
		controller.OnCutSceneStart();
		AIControllerBrawler[] array = Utils.FindObjectsOfType<AIControllerBrawler>();
		foreach (AIControllerBrawler aIControllerBrawler in array)
		{
			aIControllerBrawler.gameObject.SendMessage("OnCutSceneStart");
		}
		CutSceneClip[] array2 = clips;
		foreach (CutSceneClip cutSceneClip in array2)
		{
			cutSceneClip.InitializeClip(OnClipEnd);
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if (gameObject != null)
		{
			AOSceneCamera aOSceneCamera = gameObject.GetComponentInChildren(typeof(AOSceneCamera)) as AOSceneCamera;
			aOSceneCamera.DepthOfField = false;
		}
		mStartTime = Time.time;
		mIsPlaying = true;
		return mIsPlaying;
	}

	public void StopScene(bool sceneComplete)
	{
		mIsPlaying = false;
		GameController controller = GameController.GetController();
		if ((bool)controller)
		{
			controller.OnCutSceneEnd();
		}
		AIControllerBrawler[] array = Utils.FindObjectsOfType<AIControllerBrawler>();
		foreach (AIControllerBrawler aIControllerBrawler in array)
		{
			aIControllerBrawler.gameObject.SendMessage("OnCutSceneEnd");
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if (gameObject != null)
		{
			AOSceneCamera aOSceneCamera = gameObject.GetComponentInChildren(typeof(AOSceneCamera)) as AOSceneCamera;
			aOSceneCamera.DepthOfField = GraphicsOptions.DOF;
		}
		if (sceneComplete && completeEvent != string.Empty)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(completeEvent, false);
		}
	}

	public void OnClipEnd()
	{
		CutSceneClip[] array = clips;
		foreach (CutSceneClip cutSceneClip in array)
		{
			if (!cutSceneClip.Played)
			{
				return;
			}
		}
		StopScene(true);
	}

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		PlayScene();
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		StopScene(false);
	}

	protected override void Start()
	{
		base.Start();
		mIsPlaying = false;
	}

	public void Update()
	{
		if (!mIsPlaying)
		{
			return;
		}
		float num = Time.time - mStartTime;
		CutSceneClip[] array = clips;
		foreach (CutSceneClip cutSceneClip in array)
		{
			if (!cutSceneClip.Played && !cutSceneClip.IsPlaying && cutSceneClip.timeOffset <= num)
			{
				cutSceneClip.StartClip();
			}
		}
	}
}
