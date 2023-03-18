using UnityEngine;

public class NPCPath : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public NPCPathNode GetStartNode()
	{
		NPCPathNode[] components = Utils.GetComponents<NPCPathNode>(this, Utils.SearchChildren);
		NPCPathNode[] array = components;
		foreach (NPCPathNode nPCPathNode in array)
		{
			if (nPCPathNode.startNode)
			{
				return nPCPathNode;
			}
		}
		return null;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawIcon(base.transform.position, "NPCPathOrigin.png");
	}
}
