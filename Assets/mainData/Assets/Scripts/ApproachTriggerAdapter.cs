using UnityEngine;

public class ApproachTriggerAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject onApproached;

	public float approachDistance = 2f;

	public float approachTolerance = 1f;

	public void Triggered()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(localPlayer);
		if (localPlayer == null || component == null)
		{
			return;
		}
		Vector3 vector = base.gameObject.transform.position - localPlayer.transform.position;
		float num = approachDistance + approachTolerance;
		if (vector.sqrMagnitude <= num * num)
		{
			BehaviorTurnTo behaviorTurnTo = component.requestChangeBehavior<BehaviorTurnTo>(false);
			if (behaviorTurnTo == null)
			{
				return;
			}
			behaviorTurnTo.Initialize(base.gameObject.transform.position, OnApproachFinished);
		}
		else
		{
			BehaviorApproach behaviorApproach = component.requestChangeBehavior<BehaviorApproach>(false);
			if (behaviorApproach == null)
			{
				return;
			}
			Vector3 vector2 = base.gameObject.transform.position - vector.normalized * approachDistance;
			Quaternion rotation = Quaternion.LookRotation(base.gameObject.transform.position - vector2);
			behaviorApproach.Initialize(vector2, rotation, true, OnApproachFinished, null, 0.1f, 0.3f, true, true);
		}
		DoHackishThings(localPlayer);
	}

	private void OnApproachFinished(GameObject player)
	{
		if (onApproached != null)
		{
			onApproached.SendMessage("Triggered", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void DoHackishThings(GameObject player)
	{
		if (base.transform.parent.name.Contains("PrizeWheel"))
		{
			VOManager.Instance.PlayVO("prize_wheel", player);
		}
	}
}
