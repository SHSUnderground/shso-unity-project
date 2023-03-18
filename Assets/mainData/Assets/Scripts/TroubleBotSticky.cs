using UnityEngine;

public class TroubleBotSticky : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static void AttachTo(GameObject obj)
	{
		if (obj != null)
		{
			obj.AddComponent<TroubleBotSticky>();
		}
	}

	public static void RemoveFrom(GameObject obj)
	{
		if (obj != null)
		{
			TroubleBotSticky component = Utils.GetComponent<TroubleBotSticky>(obj);
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}
}
