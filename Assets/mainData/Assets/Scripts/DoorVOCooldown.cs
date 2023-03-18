using UnityEngine;

public class DoorVOCooldown : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float cooldownTime;

	private static readonly float COOLDOWN_DURATION = 10f;

	private static readonly float CHANCE_TO_PLAY = 0.5f;

	public static DoorVOCooldown GetCooldown(GameObject player)
	{
		DoorVOCooldown doorVOCooldown = player.GetComponent<DoorVOCooldown>();
		if (doorVOCooldown == null)
		{
			doorVOCooldown = player.AddComponent<DoorVOCooldown>();
		}
		return doorVOCooldown;
	}

	public static bool OkToPlay(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		return Random.value < CHANCE_TO_PLAY && Time.time > GetCooldown(player).cooldownTime;
	}

	public static void StartCooldown(GameObject player)
	{
		if (!(player == null))
		{
			GetCooldown(player).cooldownTime = Time.time + COOLDOWN_DURATION;
		}
	}
}
