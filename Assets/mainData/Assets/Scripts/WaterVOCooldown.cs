using UnityEngine;

public class WaterVOCooldown : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float cooldownTime;

	private static readonly float cooldownDuration = 30f;

	public static WaterVOCooldown GetCooldown(GameObject player)
	{
		WaterVOCooldown waterVOCooldown = player.GetComponent<WaterVOCooldown>();
		if (waterVOCooldown == null)
		{
			waterVOCooldown = player.AddComponent<WaterVOCooldown>();
		}
		return waterVOCooldown;
	}

	public static bool OkToPlay(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		return Time.time > GetCooldown(player).cooldownTime;
	}

	public static void StartCooldown(GameObject player)
	{
		if (!(player == null))
		{
			GetCooldown(player).cooldownTime = Time.time + cooldownDuration;
		}
	}
}
