using System;
using UnityEngine;

public class CurveTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Serializable]
	public class TimeValue
	{
		public float time;

		public float value;
	}

	public TimeValue[] data;

	public Keyframe[] data2;

	public float t;

	public bool fire;

	protected AnimationCurve curve;

	private void Start()
	{
		curve = new AnimationCurve();
		curve.AddKey(0f, 1f);
		curve.AddKey(1f, 0f);
	}

	private void Update()
	{
		if (fire)
		{
			fire = false;
			if (curve != null)
			{
				CspUtils.DebugLog("Curve = " + curve.Evaluate(t));
			}
		}
	}
}
