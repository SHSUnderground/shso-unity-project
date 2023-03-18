using System;
using UnityEngine;

[Serializable]
public class CameraOffsetData
{
	public string label = string.Empty;

	public float rotationAngle;

	public float verticalAngle = 25f;

	public float distance = 5f;

	public float rotationVelocity = 10f;

	public float movementVelocity = 3f;

	public Vector3 lookAtOffset = Vector3.up;

	public float duration;

	public float transitionTime;
}
