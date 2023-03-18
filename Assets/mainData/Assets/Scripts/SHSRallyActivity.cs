using System.Collections.Generic;
using UnityEngine;

public class SHSRallyActivity : SHSActivityBase
{
	private List<RallyPath> candidatePaths;

	private RallyPath activePath;

	private RallyPathNode activePathNode;

	private float activityStartTime;

	private float activityDuration;

	private List<GameObject> activityObjectsList;

	private Queue<RallyPathNode> activePathNodeQueue;

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		candidatePaths = new List<RallyPath>();
		activePath = null;
	}

	public override void Start(GameObject initObject)
	{
		activityObjectsList = new List<GameObject>();
		activePathNodeQueue = new Queue<RallyPathNode>();
		RallyPath component = Utils.GetComponent<RallyPath>(initObject);
		if (component == null)
		{
			CspUtils.DebugLog(initObject.name + " does not have a RallyPath component. Can't initiate activity.");
			return;
		}
		activePath = component;
		if (State != 0)
		{
			CspUtils.DebugLog("Activity already in progress for path: " + component);
			return;
		}
		if (!candidatePaths.Contains(component))
		{
			CspUtils.DebugLog("No path: " + component + " in list of candidate paths.");
			return;
		}
		base.Start();
		activePath = component;
		bool flag = false;
		activityDuration = component.duration;
		if (activityDuration < 0f)
		{
			flag = true;
		}
		List<RallyPathNode> list = new List<RallyPathNode>();
		RallyPathNode rallyPathNode = activePath.GetStartNode();
		if (rallyPathNode == null)
		{
			CspUtils.DebugLog("No Start Node set for path: " + activePath);
			Reset();
		}
		while (rallyPathNode != null && !list.Contains(rallyPathNode))
		{
			list.Add(rallyPathNode);
			if (flag)
			{
				activityDuration += rallyPathNode.duration;
			}
			ActivitySpawnPoint component2 = Utils.GetComponent<ActivitySpawnPoint>(rallyPathNode);
			if (rallyPathNode.startNode)
			{
				activePathNode = rallyPathNode;
			}
			else
			{
				activePathNodeQueue.Enqueue(rallyPathNode);
			}
			if (component2 != null)
			{
				component2.activityReference = this;
				component2.SpawnActivityObject();
			}
			rallyPathNode = rallyPathNode.nextNodes[0];
		}
		activityStartTime = Time.time;
	}

	private void setNextPathNode()
	{
		if (activePathNodeQueue.Count > 0)
		{
			activePathNode = activePathNodeQueue.Dequeue();
			toggleObjectActivity(activePathNode.gameObject, true);
		}
		else
		{
			result = ActivityResultEnum.Success;
			Complete();
		}
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
		if (socialSpaceController != null && socialSpaceController.isTestScene)
		{
			zoneName = "Daily_Bugle";
		}
		RallyPath[] array = (RallyPath[])Object.FindObjectsOfType(typeof(RallyPath));
		RallyPath[] array2 = array;
		foreach (RallyPath item in array2)
		{
			candidatePaths.Add(item);
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		if (State == ActivityStateEnum.Started)
		{
			result = ActivityResultEnum.Abort;
			Complete();
		}
	}

	public override void Update()
	{
		base.Update();
		if (State == ActivityStateEnum.Started && Time.time - activityStartTime > activityDuration)
		{
			result = ActivityResultEnum.Failure;
			Complete();
		}
	}

	public override void Complete()
	{
		base.Complete();
		ActivityResultEnum result = base.result;
		CspUtils.DebugLog("COMPLETE!!!!! Result: " + base.result.ToString());
		Reset();
		string empty = string.Empty;
		float num = Time.time - activityStartTime;
		float @float = PlayerPrefs.GetFloat(Name, 0f);
		switch (result)
		{
		default:
			return;
		case ActivityResultEnum.Success:
			empty = "Congratulations! You've completed the rally game in " + num + " seconds!";
			if (num < @float)
			{
				string text = empty;
				empty = text + "\n\n New best time (beating your old time by " + (@float - num) + " seconds.)";
				PlayerPrefs.SetFloat(Name, num);
			}
			break;
		case ActivityResultEnum.Failure:
			empty = "Sorry, but you didn't finish the rally in time. Try again!";
			break;
		}
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, empty, new GUIDialogNotificationSink(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}), GUIControl.ModalLevelEnum.None);
	}

	public override void Reset()
	{
		base.Reset();
		state = ActivityStateEnum.Idle;
		result = ActivityResultEnum.Dormant;
		clearActivityObjects();
		activePath = null;
		activePathNode = null;
		activePathNodeQueue = new Queue<RallyPathNode>();
	}

	private void clearActivityObjects()
	{
		if (activePathNode != null)
		{
			activePathNodeQueue.Enqueue(activePathNode);
		}
		int count = activePathNodeQueue.Count;
		while (activePathNodeQueue.Count > 0)
		{
			ActivityObject component = Utils.GetComponent<ActivityObject>(activePathNodeQueue.Dequeue(), Utils.SearchChildren);
			if (component == null)
			{
				CspUtils.DebugLog("Activity object null when clearing activity objects.");
			}
			else
			{
				component.Despawn();
			}
		}
		CspUtils.DebugLog("Cleared: " + count + " activity objects.");
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		activityObjectsList.Add(activityObject.gameObject);
		AppShell.Instance.EventMgr.AddListener<ActivityObjectDespawnMessage>(activityObject.gameObject, OnActivityObjectDespawned);
		RallyPathNode component = Utils.GetComponent<RallyPathNode>(activityObject.spawner);
		if (component != null)
		{
			toggleObjectActivity(component.gameObject, component == activePathNode);
			if (component.nextNodes.Length > 0 && component.nextNodes[0] != null)
			{
				activityObject.gameObject.transform.LookAt(component.nextNodes[0].transform);
			}
		}
	}

	private void toggleObjectActivity(GameObject obj, bool active)
	{
		ActivityObject component = Utils.GetComponent<ActivityObject>(obj, Utils.SearchChildren);
		if (component == null)
		{
			CspUtils.DebugLog("Activity object for: " + obj + " can't be found.");
		}
		else
		{
			component.ToggleActiveState(active);
		}
	}

	public override void UnRegisterActivityObject(ActivityObject activityObject)
	{
		activityObjectsList.Remove(activityObject.gameObject);
		AppShell.Instance.EventMgr.RemoveListener<ActivityObjectDespawnMessage>(activityObject.gameObject, OnActivityObjectDespawned);
	}

	private void OnActivityObjectDespawned(ActivityObjectDespawnMessage msg)
	{
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		if (state == ActivityStateEnum.Started && result == ActivityResultEnum.Dormant)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (!(localPlayer == null))
			{
				toggleObjectActivity(activityObject.gameObject, false);
				setNextPathNode();
				activityObject.Despawn();
				AppShell.Instance.CounterManager.AddCounter("RallyCounters");
			}
		}
	}
}
