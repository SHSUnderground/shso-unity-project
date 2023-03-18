using System.Collections.Generic;
using UnityEngine;

public class PetKillTroublebotEmitter : MonoBehaviour
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
		Dictionary<TroubleBotActivityObject, bool> dictionary = new Dictionary<TroubleBotActivityObject, bool>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			TroubleBotActivityObject componentRecursive = Utils.GetComponentRecursive<TroubleBotActivityObject>(collider.gameObject);
			if (componentRecursive != null && !dictionary.ContainsKey(componentRecursive))
			{
				dictionary.Add(componentRecursive, true);
			}
		}
		foreach (TroubleBotActivityObject key in dictionary.Keys)
		{
			if (!key.triggered)
			{
				key.ActionTriggered(ActivityObjectActionNameEnum.PowerEmote);
			}
		}
	}
}
