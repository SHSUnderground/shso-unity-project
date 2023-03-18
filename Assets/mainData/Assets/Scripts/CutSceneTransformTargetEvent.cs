using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Transform Target")]
internal class CutSceneTransformTargetEvent : CutSceneTargetEvent
{
	public GameObject targetTransform;

	public bool positionTarget = true;

	public bool rotateTarget = true;

	public override void StartEvent()
	{
		base.StartEvent();
		if (targetTransform == null)
		{
			LogEventError("Transform object not set for transform target event");
		}
		else
		{
			if (!base.Target)
			{
				return;
			}
			if (positionTarget)
			{
				CharacterMotionController component = base.Target.GetComponent<CharacterMotionController>();
				if (component != null)
				{
					component.setDestination(targetTransform);
					component.teleportTo(targetTransform.transform.position);
				}
				else
				{
					base.Target.transform.position = targetTransform.transform.position;
				}
			}
			if (rotateTarget)
			{
				base.Target.transform.rotation = targetTransform.transform.rotation;
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (targetTransform != null)
		{
			DrawGizmoToTargetTransform(targetTransform.transform);
		}
	}
}
