using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MiniRewardHandler : MonoBehaviour, IMissionEndHandler
{
	public string finishSequence = "asgard_maze_reward_received";

	[CompilerGenerated]
	private EventResultMissionEvent _003CResults_003Ek__BackingField;

	public EventResultMissionEvent Results
	{
		[CompilerGenerated]
		get
		{
			return _003CResults_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CResults_003Ek__BackingField = value;
		}
	}

	public bool OnMissionEndUI()
	{
		StartCoroutine(WaitForMissionResults());
		return false;
	}

	public bool OnMissionEndDiorama()
	{
		return false;
	}

	public void OnEnable()
	{
		StartCoroutine(RegisterForEvents());
	}

	private IEnumerator RegisterForEvents()
	{
		while (AppShell.Instance == null || AppShell.Instance.EventMgr == null)
		{
			yield return 0;
		}
		AppShell.Instance.EventMgr.AddListener<BrawlerStageBegin>(OnBrawlerStageBegin);
		AppShell.Instance.EventMgr.AddListener<BrawlerResultsMessage>(OnBrawlerResultsMessage);
	}

	public void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<BrawlerResultsMessage>(OnBrawlerResultsMessage);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerStageBegin>(OnBrawlerStageBegin);
	}

	protected void OnBrawlerStageBegin(BrawlerStageBegin e)
	{
		BrawlerController.Instance.MissionEndHandler = this;
		Results = null;
	}

	protected void OnBrawlerResultsMessage(BrawlerResultsMessage e)
	{
		Results = e.Results;
	}

	private IEnumerator WaitForMissionResults()
	{
		if (AppShell.Instance.Profile != null)
		{
			float timeout = Time.time + 8f;
			while (Time.time < timeout && Results == null)
			{
				yield return 0;
			}
			if (Results != null)
			{
				long gazillionID = AppShell.Instance.PlayerDictionary.GetPlayerId(AppShell.Instance.ServerConnection.GetGameUserId());
				if (Results.PlayerResults.ContainsKey(gazillionID))
				{
					ShowReward(Results.PlayerResults[gazillionID].ownable);
					yield break;
				}
			}
		}
		CspUtils.DebugLog("Could not retrieve player reward; exiting stage");
		BrawlerController.Instance.ClearObjects();
		AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
	}

	private void ShowReward(string ownableTypeId)
	{
		BrawlerController.Instance.ClearObjects(false);
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		BehaviorManager component = localPlayer.GetComponent<BehaviorManager>();
		BehaviorReceiveItem behaviorReceiveItem = component.requestChangeBehavior<BehaviorReceiveItem>(false);
		if (behaviorReceiveItem != null)
		{
			behaviorReceiveItem.Initialize(finishSequence);
		}
		GUIManager.Instance.ShowDynamicWindow(new SHSBrawlerMiniRewardWindow(ownableTypeId), GUIControl.ModalLevelEnum.Full);
	}
}
