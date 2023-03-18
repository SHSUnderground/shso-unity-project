using System.Collections.Generic;
using UnityEngine;

public class PetStunPigeonEmitter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private int _frameCount;

	public int radius = 7;

	public void Update()
	{
		if (SocialSpaceControllerImpl.playerIsIdle)
		{
			return;
		}
		_frameCount++;
		if (_frameCount < 3)
		{
			return;
		}
		_frameCount = 0;
		Collider[] array = Physics.OverlapSphere(base.gameObject.transform.position, radius);
		if (array.Length <= 0)
		{
			return;
		}
		Dictionary<InteractiveObject, bool> dictionary = new Dictionary<InteractiveObject, bool>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			AIControllerPigeon componentRecursive = Utils.GetComponentRecursive<AIControllerPigeon>(collider.gameObject);
			if (componentRecursive != null && componentRecursive.Idling)
			{
				InteractiveObject componentRecursive2 = Utils.GetComponentRecursive<InteractiveObject>(collider.gameObject);
				if (componentRecursive2 != null && !dictionary.ContainsKey(componentRecursive2))
				{
					dictionary.Add(componentRecursive2, true);
				}
			}
		}
		int num = 0;
		foreach (InteractiveObject key in dictionary.Keys)
		{
			num++;
			key.OnPowerEmote(SocialSpaceController.Instance.LocalPlayer);
		}
	}
}
