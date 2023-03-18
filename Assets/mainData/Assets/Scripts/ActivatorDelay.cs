using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/Activator/Delay")]
public class ActivatorDelay : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject Target;

	public float MinDelay;

	public float MaxDelay;

	private void OnEnable()
	{
		if (Target != null)
		{
			StartCoroutine(Go(Target, Random.Range(MinDelay, MaxDelay)));
		}
	}

	private IEnumerator Go(GameObject target, float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		target.active = true;
		yield return new WaitForEndOfFrame();
		base.gameObject.active = false;
	}
}
