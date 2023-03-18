using UnityEngine;

public class RallyPath : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string activityName = "RallyActivity";

	public float duration = 30f;

	public RallyPathNode GetStartNode()
	{
		RallyPathNode[] components = Utils.GetComponents<RallyPathNode>(this, Utils.SearchChildren);
		RallyPathNode[] array = components;
		foreach (RallyPathNode rallyPathNode in array)
		{
			if (rallyPathNode.startNode)
			{
				return rallyPathNode;
			}
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "RallyPathOrigin.png");
	}
}
