using UnityEngine;

public class DebugDrawBounds : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool DrawBounds = true;

	private void OnDrawGizmos()
	{
		if (DrawBounds)
		{
			foreach (Transform item in Utils.WalkTree(base.gameObject.transform))
			{
				GameObject gameObject = item.gameObject;
				if (gameObject.renderer != null)
				{
					Vector3 min = gameObject.renderer.bounds.min;
					Vector3 max = gameObject.renderer.bounds.max;
					Vector3 vector = new Vector3(min.x, min.y, max.z);
					Vector3 vector2 = new Vector3(max.x, min.y, max.z);
					Vector3 to = new Vector3(max.x, min.y, min.z);
					Vector3 vector3 = new Vector3(min.x, max.y, min.z);
					Vector3 vector4 = new Vector3(min.x, max.y, max.z);
					Vector3 to2 = new Vector3(max.x, max.y, min.z);
					Gizmos.color = Color.white;
					Gizmos.DrawLine(min, vector3);
					Gizmos.DrawLine(min, vector);
					Gizmos.DrawLine(min, to);
					Gizmos.DrawLine(vector3, vector4);
					Gizmos.DrawLine(vector3, to2);
					Gizmos.DrawLine(max, vector2);
					Gizmos.DrawLine(max, vector4);
					Gizmos.DrawLine(max, to2);
					Gizmos.DrawLine(vector2, vector);
					Gizmos.DrawLine(vector2, to);
					Gizmos.DrawLine(vector4, vector);
					Gizmos.DrawLine(min, max);
					Gizmos.DrawLine(vector4, to);
					Gizmos.DrawLine(vector3, vector2);
					Gizmos.DrawLine(vector, to2);
				}
			}
		}
	}
}
