using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Lab/Activator/Trigger")]
public class ActivatorTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject Target;

	public bool TriggerOnEnter = true;

	public bool TriggerOnExit;

	private List<Collider> enterTriggerSet = new List<Collider>();

	private List<Collider> exitTriggerSet = new List<Collider>();

	public void OnTriggerEnter(Collider other)
	{
		if (TriggerOnEnter && Target != null && !enterTriggerSet.Contains(other))
		{
			enterTriggerSet.Add(other);
			Log("triggered OnEnter");
			Target.active = true;
			StartCoroutine(Finish());
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (TriggerOnExit && Target != null && !exitTriggerSet.Contains(other))
		{
			exitTriggerSet.Add(other);
			Log("triggered OnExit");
			Target.active = true;
			StartCoroutine(Finish());
		}
	}

	private IEnumerator Finish()
	{
		yield return new WaitForEndOfFrame();
		base.gameObject.active = false;
		enterTriggerSet.Clear();
		exitTriggerSet.Clear();
	}

	private void Log(string message)
	{
	}
}
