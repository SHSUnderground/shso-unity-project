using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
	internal enum Direction
	{
		NE,
		NW,
		SE,
		SW
	}

	internal class QuadNode
	{
		internal bool canSubdivide;

		internal Bounds2 bounds;

		private QuadNode[] nodes;

		private List<GameObject> objects;

		internal QuadNode this[Direction idx]
		{
			get
			{
				return nodes[(int)idx];
			}
			set
			{
				nodes[(int)idx] = value;
			}
		}

		internal QuadNode[] Nodes
		{
			get
			{
				return nodes;
			}
		}

		internal List<GameObject> Objects
		{
			get
			{
				return objects;
			}
		}

		internal QuadNode(Bounds2 bounds)
		{
			canSubdivide = true;
			this.bounds = bounds;
			nodes = new QuadNode[4];
			objects = new List<GameObject>();
		}

		internal bool Contains(Vector3 point)
		{
			return bounds.Contains(new Vector2(point.x, point.z));
		}

		internal bool Contains(Vector2 point)
		{
			return bounds.Contains(point);
		}

		internal void Encapsulate(Vector2 point)
		{
			bounds.Encapsulate(point);
		}
	}

	internal QuadNode root;

	internal int maxObjectsPerLeaf;

	internal Vector2 minLeafSize;

	internal Dictionary<GameObject, QuadNode> objectToNode;

	public QuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf)
	{
		this.minLeafSize = minLeafSize;
		this.maxObjectsPerLeaf = maxObjectsPerLeaf;
		objectToNode = new Dictionary<GameObject, QuadNode>();
	}

	public void AddPoint(GameObject go)
	{
		if (root == null)
		{
			Vector3 position = go.transform.position;
			float x = position.x;
			Vector3 position2 = go.transform.position;
			Vector2 center = new Vector2(x, position2.z);
			Bounds2 bounds = new Bounds2(center, minLeafSize);
			root = new QuadNode(bounds);
		}
		while (!root.Contains(go.transform.position))
		{
			ExpandRoot(go.transform.position);
		}
		InsertNode(root, go);
	}

	public void Remove(GameObject go)
	{
		QuadNode value = null;
		if (objectToNode.TryGetValue(go, out value))
		{
			value.Objects.Remove(go);
			objectToNode.Remove(go);
		}
	}

	public List<GameObject> Query(Vector3 pt, float radius, bool sort)
	{
		List<GameObject> list = new List<GameObject>();
		Bounds2 bounds = new Bounds2(new Vector2(pt.x, pt.z), new Vector2(radius, radius));
		QueryNode(root, bounds, list);
		if (sort)
		{
			list.Sort(delegate(GameObject o1, GameObject o2)
			{
				float sqrMagnitude = (o1.transform.position - pt).sqrMagnitude;
				float sqrMagnitude2 = (o2.transform.position - pt).sqrMagnitude;
				return sqrMagnitude.CompareTo(sqrMagnitude2);
			});
		}
		return list;
	}

	public void DebugDraw()
	{
		DebugDraw(root, 0);
	}

	internal void DebugDraw(QuadNode node, int level)
	{
		if (node != null)
		{
			QuadNode[] nodes = node.Nodes;
			foreach (QuadNode node2 in nodes)
			{
				DebugDraw(node2, level + 1);
			}
			Color color = Color.red;
			if (level % 2 == 0)
			{
				color = Color.magenta;
			}
			Vector2 center = node.bounds.center;
			float x = center.x;
			Vector2 extents = node.bounds.extents;
			float x2 = x - extents.x;
			Vector2 center2 = node.bounds.center;
			float y = center2.y;
			Vector2 extents2 = node.bounds.extents;
			Vector3 start = new Vector3(x2, 0f, y - extents2.y);
			Vector2 center3 = node.bounds.center;
			float x3 = center3.x;
			Vector2 extents3 = node.bounds.extents;
			float x4 = x3 - extents3.x;
			Vector2 center4 = node.bounds.center;
			float y2 = center4.y;
			Vector2 extents4 = node.bounds.extents;
			Vector3 end = new Vector3(x4, 0f, y2 + extents4.y);
			Vector2 center5 = node.bounds.center;
			float x5 = center5.x;
			Vector2 extents5 = node.bounds.extents;
			float x6 = x5 + extents5.x;
			Vector2 center6 = node.bounds.center;
			float y3 = center6.y;
			Vector2 extents6 = node.bounds.extents;
			Vector3 end2 = new Vector3(x6, 0f, y3 - extents6.y);
			Vector2 center7 = node.bounds.center;
			float x7 = center7.x;
			Vector2 extents7 = node.bounds.extents;
			float x8 = x7 + extents7.x;
			Vector2 center8 = node.bounds.center;
			float y4 = center8.y;
			Vector2 extents8 = node.bounds.extents;
			Vector3 start2 = new Vector3(x8, 0f, y4 + extents8.y);
			Debug.DrawLine(start, end, color);
			Debug.DrawLine(start, end2, color);
			Debug.DrawLine(start2, end, color);
			Debug.DrawLine(start2, end2, color);
		}
	}

	internal void InsertNode(QuadNode node, GameObject go)
	{
		if (!node.Contains(go.transform.position))
		{
			throw new Exception("Point " + go.transform.position + " does not fit " + node.bounds.ToString());
		}
		if (node.canSubdivide && node.Objects.Count + 1 > maxObjectsPerLeaf && SetupChildNodes(node))
		{
			List<GameObject> list = new List<GameObject>(node.Objects);
			foreach (GameObject item in list)
			{
				Remove(item);
				InsertNode(node, item);
			}
		}
		QuadNode[] nodes = node.Nodes;
		foreach (QuadNode quadNode in nodes)
		{
			if (quadNode != null && quadNode.Contains(go.transform.position))
			{
				InsertNode(quadNode, go);
				return;
			}
		}
		node.Objects.Add(go);
		objectToNode.Add(go, node);
	}

	internal void QueryNode(QuadNode node, Bounds2 bounds, List<GameObject> results)
	{
		if (node != null && node.bounds.Intersects(bounds))
		{
			foreach (GameObject @object in node.Objects)
			{
				if (bounds.Contains(@object.transform.position))
				{
					results.Add(@object);
				}
			}
			QuadNode[] nodes = node.Nodes;
			foreach (QuadNode node2 in nodes)
			{
				QueryNode(node2, bounds, results);
			}
		}
	}

	internal bool SetupChildNodes(QuadNode node)
	{
		Vector2 size = node.bounds.size * 0.5f;
		if (minLeafSize.x <= size.x && minLeafSize.y <= size.y)
		{
			Vector2 center = default(Vector2);
			Vector2 extents = node.bounds.extents;
			float num = extents.x * 0.5f;
			Vector2 extents2 = node.bounds.extents;
			float num2 = extents2.y * 0.5f;
			Vector2 center2 = node.bounds.center;
			center.x = center2.x + num;
			Vector2 center3 = node.bounds.center;
			center.y = center3.y + num2;
			node[Direction.NE] = new QuadNode(new Bounds2(center, size));
			Vector2 center4 = node.bounds.center;
			center.x = center4.x - num;
			Vector2 center5 = node.bounds.center;
			center.y = center5.y + num2;
			node[Direction.NW] = new QuadNode(new Bounds2(center, size));
			Vector2 center6 = node.bounds.center;
			center.x = center6.x + num;
			Vector2 center7 = node.bounds.center;
			center.y = center7.y - num2;
			node[Direction.SE] = new QuadNode(new Bounds2(center, size));
			Vector2 center8 = node.bounds.center;
			center.x = center8.x - num;
			Vector2 center9 = node.bounds.center;
			center.y = center9.y - num2;
			node[Direction.SW] = new QuadNode(new Bounds2(center, size));
			node.canSubdivide = false;
			return true;
		}
		node.canSubdivide = false;
		return false;
	}

	internal void ExpandRoot(Vector3 point)
	{
		float z = point.z;
		Vector2 center = root.bounds.center;
		bool flag = z >= center.y;
		float x = point.x;
		Vector2 center2 = root.bounds.center;
		bool flag2 = x >= center2.x;
		Vector2 size = root.bounds.size * 2f;
		Vector2 center3 = root.bounds.center;
		Direction idx;
		if (flag)
		{
			if (flag2)
			{
				float x2 = center3.x;
				Vector2 extents = root.bounds.extents;
				center3.x = x2 + extents.x;
				float y = center3.y;
				Vector2 extents2 = root.bounds.extents;
				center3.y = y + extents2.y;
				idx = Direction.SW;
			}
			else
			{
				float x3 = center3.x;
				Vector2 extents3 = root.bounds.extents;
				center3.x = x3 - extents3.x;
				float y2 = center3.y;
				Vector2 extents4 = root.bounds.extents;
				center3.y = y2 + extents4.y;
				idx = Direction.SE;
			}
		}
		else if (flag2)
		{
			float x4 = center3.x;
			Vector2 extents5 = root.bounds.extents;
			center3.x = x4 + extents5.x;
			float y3 = center3.y;
			Vector2 extents6 = root.bounds.extents;
			center3.y = y3 - extents6.y;
			idx = Direction.NW;
		}
		else
		{
			float x5 = center3.x;
			Vector2 extents7 = root.bounds.extents;
			center3.x = x5 - extents7.x;
			float y4 = center3.y;
			Vector2 extents8 = root.bounds.extents;
			center3.y = y4 - extents8.y;
			idx = Direction.NE;
		}
		QuadNode quadNode = new QuadNode(new Bounds2(center3, size));
		SetupChildNodes(quadNode);
		quadNode[idx] = root;
		root = quadNode;
	}
}
