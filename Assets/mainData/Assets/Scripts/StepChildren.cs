using System.Collections.Generic;
using UnityEngine;

internal class StepChildren : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public List<GameObject> stepKids;

	public void Awake()
	{
		stepKids = new List<GameObject>();
	}
}
