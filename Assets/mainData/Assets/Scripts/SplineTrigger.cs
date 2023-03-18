using UnityEngine;

public class SplineTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SplineController spline;

	public bool followRotations = true;

	public bool ignoreCollision = true;

	public bool freeFallOnFinish;

	private void OnTriggerEnter(Collider other)
	{
		if (Utils.IsPlayer(other.gameObject) && spline != null)
		{
			BehaviorManager component = other.gameObject.GetComponent<BehaviorManager>();
			BehaviorSpline behaviorSpline = component.requestChangeBehavior<BehaviorSpline>(false);
			if (behaviorSpline != null)
			{
				behaviorSpline.Initialize(spline, followRotations, null, ignoreCollision, freeFallOnFinish);
			}
		}
	}
}
