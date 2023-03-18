using UnityEngine;

public class WaterAudio : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool useVO;

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<CharacterGlobals>() != null)
		{
			WaterAudioTracker waterAudioTracker = WaterAudioTracker.AddTracker(other);
			if (waterAudioTracker != null)
			{
				waterAudioTracker.UseVO = useVO;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		WaterAudioTracker.RemoveTracker(other);
	}

	public void OnPowerEmote(GameObject player)
	{
		WaterAudioTracker.ManualPlaySplash(player);
	}
}
