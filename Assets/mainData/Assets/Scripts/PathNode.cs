using System.Collections.Generic;
using UnityEngine;

public class PathNode : PathNodeBase
{
	public long lastSearch;

	public List<PathNode> forcedLinks = new List<PathNode>();

	public List<PathNode> brokenLinks = new List<PathNode>();

	public List<PathNode> disabledLinks = new List<PathNode>();

	public List<PathNode> teleportLinks = new List<PathNode>();

	public static float MaxAngle = 45f;

	public override void Awake()
	{
		lastSearch = 0L;
	}

	public bool LinkNode(PathNode node)
	{
		if (this == node)
		{
			return false;
		}
		if (brokenLinks.Contains(node))
		{
			return false;
		}
		if ((base.transform.position - node.transform.position).sqrMagnitude > PathNodeBase.MaxDistanceSqrd)
		{
			return false;
		}
		GameObject x = ClearLineTo(node);
		if (x != null)
		{
			return false;
		}
		return CanWalkBetween(node);
	}

	public bool CanWalkBetween(PathNode node)
	{
		Vector3 from = node.transform.position - base.transform.position;
		float num = Vector3.Angle(from, Vector3.up);
		if (num < MaxAngle || num > 180f - MaxAngle)
		{
			return false;
		}
		if (GapInBetween(node))
		{
			return false;
		}
		return true;
	}

	public bool GapInBetween(Vector3 position)
	{
		Vector3 vector = base.transform.position + new Vector3(0f, PathNodeBase.StepHeight, 0f);
		Vector3 a = position + new Vector3(0f, PathNodeBase.StepHeight, 0f);
		Vector3 vector2 = a - vector;
		float magnitude = vector2.magnitude;
		vector2.Normalize();
		for (int i = 0; (float)i < magnitude; i++)
		{
			Vector3 end = new Vector3(vector.x, vector.y - PathNodeBase.StepHeight * 2f, vector.z);
			RaycastHit hitInfo;
			if (!Physics.Linecast(vector, end, out hitInfo))
			{
				return true;
			}
			vector += vector2;
		}
		return false;
	}

	protected bool GapInBetween(PathNode node)
	{
		return GapInBetween(node.transform.position);
	}
}
