using System.Collections.Generic;
using UnityEngine;

public class NPCPathNode : PathNodeBase
{
	public NPCPathNode[] nextNodes;

	public List<NPCCommandHint> commands;

	public static bool drawWarningLines = true;

	public bool startNode;

	public float radius = 0.25f;

	public float linecastHeight = 1f;

	public override void Awake()
	{
		base.Awake();
		Type = NodeType.NPC;
	}

	public override void Start()
	{
		NPCPathNode[] array = nextNodes;
		foreach (NPCPathNode x in array)
		{
			if (x == null)
			{
			}
		}
	}

	public override void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(base.transform.position, 0.15f);
		Matrix4x4 matrix = Gizmos.matrix;
		float d = radius + 0.5f;
		NPCPathNode[] array = nextNodes;
		foreach (NPCPathNode nPCPathNode in array)
		{
			if (nPCPathNode != null)
			{
				GameObject gameObject = nPCPathNode.gameObject;
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, gameObject.transform.position);
				float d2 = nPCPathNode.radius + 0.5f;
				if (drawWarningLines)
				{
					Vector3 normalized = (gameObject.transform.position - base.transform.position).normalized;
					Vector3 normalized2 = Vector3.Cross(normalized, new Vector3(0f, 1f, 0f)).normalized;
					Vector3 vector = base.transform.position + normalized2 * d + new Vector3(0f, linecastHeight, 0f);
					Vector3 vector2 = gameObject.transform.position + normalized2 * d2 + new Vector3(0f, nPCPathNode.linecastHeight, 0f);
					Gizmos.color = ((!NPCMoveToNodeCommand.ClearLineTo(vector, vector2)) ? Color.red : new Color(255f, 255f, 0f, 0.2f));
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawLine(vector, vector + new Vector3(0f, 0f - linecastHeight, 0f));
					Gizmos.DrawLine(vector2, vector2 + new Vector3(0f, 0f - nPCPathNode.linecastHeight, 0f));
					Vector3 vector3 = base.transform.position - normalized2 * d + new Vector3(0f, linecastHeight, 0f);
					Vector3 vector4 = gameObject.transform.position - normalized2 * d2 + new Vector3(0f, nPCPathNode.linecastHeight, 0f);
					Gizmos.color = ((!NPCMoveToNodeCommand.ClearLineTo(vector3, linecastHeight, vector4, nPCPathNode.linecastHeight)) ? Color.red : new Color(255f, 255f, 0f, 0.2f));
					Gizmos.DrawLine(vector3, vector4);
					Gizmos.DrawLine(vector3, vector3 + new Vector3(0f, 0f - linecastHeight, 0f));
					Gizmos.DrawLine(vector4, vector4 + new Vector3(0f, 0f - nPCPathNode.linecastHeight, 0f));
				}
			}
		}
		Gizmos.matrix = matrix;
		Color color = Gizmos.color;
		Gizmos.color = new Color(0.5f, 0.3f, 0.2f, 0.3f);
		Matrix4x4 matrix2 = default(Matrix4x4);
		matrix2.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 0.2f, 1f));
		Gizmos.matrix = matrix2;
		Gizmos.DrawSphere(base.transform.position, radius);
		Gizmos.color = color;
		Gizmos.matrix = matrix;
		base.OnDrawGizmosSelected();
	}

	public override bool SnapToGround(float tolerance)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + new Vector3(0f, tolerance, 0f), Vector3.down, out hitInfo, 100f, 804756969))
		{
			base.transform.position = hitInfo.point;
			return true;
		}
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float y = position2.y;
		Vector3 position3 = base.transform.position;
		Vector3 origin = new Vector3(x, y, position3.z);
		origin += new Vector3(0f, 100f, 0f);
		if (Physics.Raycast(origin, Vector3.down, out hitInfo, 100f, 804756969))
		{
			base.transform.position = hitInfo.point;
			return true;
		}
		return false;
	}
}
