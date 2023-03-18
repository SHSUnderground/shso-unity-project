using UnityEngine;

public class RandomChildSelector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Awake()
	{
		int num = Random.Range(0, base.transform.childCount);
		int num2 = 0;
		foreach (Transform item in base.transform)
		{
			if (num2++ != num)
			{
				Object.Destroy(item.gameObject);
			}
		}
	}
}
