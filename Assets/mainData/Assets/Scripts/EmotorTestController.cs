using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/Emotor/Test Controller")]
public class EmotorTestController : GameController
{
	public Transform Spawn;

	public GameObject SpawnerPrefab;

	public override void Awake()
	{
		base.Awake();
		bCallControllerReadyFromStart = false;
		AppShell.Instance.EventMgr.AddListener<ApplicationInitializedMessage>(OnAppShellReady);
		StartCoroutine(KillLoadScreen());
	}

	protected void OnAppShellReady(ApplicationInitializedMessage msg)
	{
		AppShell.Instance.EventMgr.RemoveListener<ApplicationInitializedMessage>(OnAppShellReady);
		ControllerReady();
		if (SpawnerPrefab != null)
		{
			if (Spawn != null)
			{
				Object.Instantiate(SpawnerPrefab, Spawn.position, Spawn.rotation);
			}
			else
			{
				Object.Instantiate(SpawnerPrefab);
			}
		}
	}

	private IEnumerator KillLoadScreen()
	{
		GUIManager.Instance.SetScreenBackColor(GUIManager.UILayer.System, false);
		AppShell.Instance.EventMgr.Fire(this, new WaitWatcherEvent.KillWaitWatcher());
		yield return null;
	}
}
