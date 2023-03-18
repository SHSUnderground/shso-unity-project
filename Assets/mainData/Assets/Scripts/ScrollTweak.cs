using System;
using UnityEngine;

[Serializable]
public class ScrollTweak
{
	[Serializable]
	public class TimeValue
	{
		public float time;

		public float value;
	}

	public float scrollArea = 0.2f;

	public float scrollDelay = 0.25f;

	public float scrollSpeedMax = 20f;

	public float scrollSpeedAcc = 10f;

	public Vector2[] distToAcceleration;

	public bool scrollDrag;

	public bool scrollDragInvert;

	public float scrollDragSpeed = 75f;
}
