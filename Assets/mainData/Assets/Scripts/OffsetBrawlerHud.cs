using System.Collections;
using UnityEngine;

public class OffsetBrawlerHud : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Vector3 offset;

	private void Start()
	{
		StartCoroutine(CoOffsetHud());
	}

	private IEnumerator CoOffsetHud()
	{
		BrawlerOrthographicHud hud = Object.FindObjectOfType(typeof(BrawlerOrthographicHud)) as BrawlerOrthographicHud;
		while (hud == null)
		{
			yield return new WaitForSeconds(2f);
			hud = (Object.FindObjectOfType(typeof(BrawlerOrthographicHud)) as BrawlerOrthographicHud);
		}
		hud.transform.position += offset;
	}
}
