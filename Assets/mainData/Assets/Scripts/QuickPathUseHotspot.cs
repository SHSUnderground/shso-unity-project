using UnityEngine;

[AddComponentMenu("Lab/QuickPath/Use Hotspot")]
[RequireComponent(typeof(CharacterTargetSource))]
public class QuickPathUseHotspot : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public HotSpot HotSpot;

	private void Update()
	{
		GameObject character = Utils.GetComponent<CharacterTargetSource>(this).Character;
		if ((bool)character)
		{
			HotSpot.UseHotSpot(character);
			base.gameObject.active = false;
		}
	}

	public void Activate()
	{
		base.gameObject.active = true;
	}

	public void Deactivate()
	{
		base.gameObject.active = false;
	}
}
