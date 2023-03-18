using UnityEngine;

[AddComponentMenu("ScenarioEvent/Absolute Translate")]
public class AbsoluteTranslateScenarioEvent : ScenarioEventHandlerEnableBase
{
	public Transform destination;

	public GameObject objectToMoveOverride;

	public bool storeOriginalLocationOnStart;

	protected Vector3 originalPosition;

	protected Quaternion originalRotation;

	protected bool storedTransform;

	protected GameObject ToMove
	{
		get
		{
			if (objectToMoveOverride != null)
			{
				return objectToMoveOverride;
			}
			return base.gameObject;
		}
	}

	protected override void Start()
	{
		if (storeOriginalLocationOnStart)
		{
			StoreLocation();
		}
		base.Start();
	}

	protected override void OnEnableEvent(string eventName)
	{
		StoreLocation();
		if (destination != null)
		{
			GameObject toMove = ToMove;
			toMove.transform.position = destination.position;
			toMove.transform.rotation = destination.rotation;
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		GameObject toMove = ToMove;
		toMove.transform.position = originalPosition;
		toMove.transform.rotation = originalRotation;
	}

	protected void StoreLocation()
	{
		GameObject toMove = ToMove;
		if (!storedTransform)
		{
			originalPosition = toMove.transform.position;
			originalRotation = toMove.transform.rotation;
			storedTransform = true;
		}
	}
}
