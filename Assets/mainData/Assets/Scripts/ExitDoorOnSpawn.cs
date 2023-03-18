using System.Collections;
using UnityEngine;

public class ExitDoorOnSpawn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public DoorManager door;

	public void OnPlayerSpawned(GameObject player)
	{
		if (Utils.IsLocalPlayer(player) && !player.name.Contains("placeholder"))
		{
			StartCoroutine(ExitDoor(player));
		}
	}

	private IEnumerator ExitDoor(GameObject player)
	{
		BehaviorManager behaviorMan = player.GetComponent<BehaviorManager>();
		while (behaviorMan == null)
		{
			yield return 0;
			behaviorMan = player.GetComponent<BehaviorManager>();
		}
		while (behaviorMan.getBehavior() == null)
		{
			yield return 0;
		}
		behaviorMan.ChangeDefaultBehavior("BehaviorWait");
		NetworkComponent net = Utils.GetComponent<NetworkComponent>(player);
		while (!net.IsOwner())
		{
			yield return 0;
		}
		yield return new WaitForSeconds(2f);
		door.ExitWithPlayer(player, OnDoorExited, false);
		if (net != null)
		{
			NetActionExitDoor action = new NetActionExitDoor(player, door);
			net.QueueNetAction(action);
		}
	}

	private void OnDoorExited(GameObject player, InteractiveObjectController.CompletionStateEnum completionState)
	{
		if (Utils.IsLocalPlayer(player))
		{
			PlayerOcclusionDetector.OcclusionDetectionEnabled = true;
		}
	}
}
