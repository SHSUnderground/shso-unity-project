using UnityEngine;

public class TargetDummyLootAwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float kCooldownPeriod = 10f;

	public float coinProbability = 0.5f;

	public float ticketProbability = 0.01f;

	private float nextLootTime;

	private void OnAttacked(CharacterGlobals attackingPlayer)
	{
		if (attackingPlayer.gameObject == GameController.GetController().LocalPlayer)
		{
			DropLoot();
		}
	}

	private void DropLoot()
	{
		if (Time.time < nextLootTime)
		{
			return;
		}
		nextLootTime = Time.time + 10f;
		ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(this, Utils.SearchChildren);
		if (component == null)
		{
			CspUtils.DebugLog("No activity spawner found on target dummy -- No loot can be dropped");
			return;
		}
		float value = Random.value;
		if (value <= ticketProbability)
		{
			component.activityPrefab = "HedgeTicketPrefab";
			component.SpawnActivityObject();
		}
		else if (value <= coinProbability)
		{
			component.activityPrefab = "HedgeCoinPrefab";
			component.SpawnActivityObject();
		}
	}
}
