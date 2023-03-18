using UnityEngine;

public class RobberPathNode : NPCPathNode
{
	public bool robberWaitsForPlayer;

	public string overrideAnimationName = string.Empty;

	public string effectSequenceName = string.Empty;

	public float overrideAlertRadius = -1f;

	public static float kDefaultAlertRadius
	{
		get
		{
			return 3.5f;
		}
	}

	public override void OnDrawGizmosSelected()
	{
		if (robberWaitsForPlayer)
		{
			Gizmos.DrawIcon(base.transform.position, "RobberWaitPoint.png");
			if (overrideAlertRadius > 0f)
			{
				Gizmos.DrawWireSphere(base.transform.position, overrideAlertRadius + radius);
			}
			else
			{
				Gizmos.DrawWireSphere(base.transform.position, kDefaultAlertRadius + radius);
			}
		}
		base.OnDrawGizmosSelected();
	}
}
