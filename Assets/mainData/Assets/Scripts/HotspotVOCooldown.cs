using System;
using UnityEngine;

public class HotspotVOCooldown : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float[] cooldownTimes = new float[Enum.GetNames(typeof(HotSpotType.Style)).Length];

	private static readonly float COOLDOWN_DURATION = 10f;

	private static readonly float CHANCE_TO_PLAY = 0.5f;

	public static HotspotVOCooldown GetCooldown(GameObject player)
	{
		return Utils.GetComponentAlways<HotspotVOCooldown>(player);
	}

	public static bool OkToPlay(GameObject player, HotSpotType.Style hotSpotType)
	{
		if (player == null)
		{
			return false;
		}
		int styleIndex = HotSpotType.GetStyleIndex(hotSpotType);
		HotspotVOCooldown cooldown = GetCooldown(player);
		if (cooldown.cooldownTimes[styleIndex] == 0f)
		{
			return true;
		}
		return UnityEngine.Random.value < CHANCE_TO_PLAY && Time.time > cooldown.cooldownTimes[styleIndex];
	}

	public static void StartCooldown(GameObject player, HotSpotType.Style hotSpotType)
	{
		if (!(player == null))
		{
			GetCooldown(player).cooldownTimes[HotSpotType.GetStyleIndex(hotSpotType)] = Time.time + COOLDOWN_DURATION;
		}
	}
}
