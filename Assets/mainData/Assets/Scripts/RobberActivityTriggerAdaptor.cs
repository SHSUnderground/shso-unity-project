using UnityEngine;

public class RobberActivityTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool useEmote;

	private BehaviorManager playerBehaviorManager;

	public void Triggered()
	{
		if (useEmote)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			playerBehaviorManager = Utils.GetComponent<BehaviorManager>(localPlayer);
			if (playerBehaviorManager != null)
			{
				BehaviorApproach behaviorApproach = playerBehaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
				if (behaviorApproach != null)
				{
					Vector3 position = localPlayer.transform.position;
					Quaternion rotation = Quaternion.LookRotation(base.gameObject.transform.position - localPlayer.transform.position);
					behaviorApproach.Initialize(position, rotation, false, OnApproachArrived, OnApproachArrived, 0.1f, 0.1f, true, false);
				}
				else
				{
					OnApproachArrived(GameController.GetController().LocalPlayer);
				}
			}
		}
		else
		{
			ForceStart();
		}
	}

	private void OnApproachArrived(GameObject player)
	{
		BehaviorEmote behaviorEmote = playerBehaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
		if (behaviorEmote != null)
		{
			behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("taunt").id, false, OnFinishedEmote);
			return;
		}
		CspUtils.DebugLog("Could not start robber rally with emote; defaulting to a forced start");
		ForceStart();
	}

	private void OnFinishedEmote(GameObject unusedParameter)
	{
		ForceStart();
	}

	private void ForceStart()
	{
		string kActivityName = AIControllerRobber.kActivityName;
		SHSRobberRallyActivity sHSRobberRallyActivity = AppShell.Instance.ActivityManager.GetActivity(kActivityName) as SHSRobberRallyActivity;
		if (sHSRobberRallyActivity == null)
		{
			CspUtils.DebugLog("Activity: " + kActivityName + " does not exist.");
			return;
		}
		AIControllerRobber robber = GetRobber();
		if (robber == null)
		{
			CspUtils.DebugLog("RobberActivityTriggerAdapter component is not attached to an object containing an AIControllerRobber component.");
			return;
		}
		sHSRobberRallyActivity.BeginRally(robber);
		if (robber.consideredHuman)
		{
			VOManager.Instance.PlayVO("chase_robber", GameController.GetController().LocalPlayer);
		}
		else
		{
			VOManager.Instance.PlayVO("chase_non_robber", GameController.GetController().LocalPlayer);
		}
		RobberActivityTriggerAdaptor[] components = Utils.GetComponents<RobberActivityTriggerAdaptor>(robber, Utils.SearchChildren, true);
		foreach (RobberActivityTriggerAdaptor robberActivityTriggerAdaptor in components)
		{
			Object.Destroy(robberActivityTriggerAdaptor.gameObject);
		}
		InteractiveObject[] components2 = Utils.GetComponents<InteractiveObject>(robber, Utils.SearchChildren, true);
		foreach (InteractiveObject interactiveObject in components2)
		{
			interactiveObject.clickAcceptedForEnable = false;
			interactiveObject.highlightOnHover = false;
			interactiveObject.highlightOnProximity = false;
		}
	}

	private AIControllerRobber GetRobber()
	{
		GameObject gameObject = base.gameObject;
		while (gameObject != null)
		{
			AIControllerRobber component = Utils.GetComponent<AIControllerRobber>(gameObject);
			if (component != null)
			{
				return component;
			}
			gameObject = ((!(gameObject.transform.parent != null)) ? null : gameObject.transform.parent.gameObject);
		}
		return null;
	}
}
