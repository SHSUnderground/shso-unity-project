using System;
using UnityEngine;

[Serializable]
public class FlingaHandTweak
{
	public float mouseToVelocity = 5f;

	public float maxVelocity = 40f;

	public float maxVelocityUp = 10f;

	public Vector2[] throwToVelocityUp;

	public Vector2[] gestureTimeToVelocityUp;
}
