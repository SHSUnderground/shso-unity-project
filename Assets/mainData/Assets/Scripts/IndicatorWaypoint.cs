using UnityEngine;

public class IndicatorWaypoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string indicatorIcon = string.Empty;

	public GameObject indicatorPrefab;

	public virtual void OnEnable()
	{
		if (BrawlerController.Instance != null)
		{
			BrawlerController.Instance.AddWaypoint(base.gameObject, indicatorPrefab);
		}
	}

	public virtual void OnDisable()
	{
		if (BrawlerController.Instance != null)
		{
			BrawlerController.Instance.RemoveWaypoint(base.gameObject);
		}
	}
}
