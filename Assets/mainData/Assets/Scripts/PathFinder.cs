using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	internal class NodeData : IComparable<NodeData>
	{
		internal PathNode node;

		internal PathNode parent;

		internal bool closed;

		internal float distance;

		internal float estimate;

		internal NodeData(PathNode node, Vector3 dest)
		{
			this.node = node;
			parent = null;
			closed = false;
			distance = 0f;
			estimate = (node.transform.position - dest).sqrMagnitude;
		}

		public int CompareTo(NodeData other)
		{
			float num = distance + estimate;
			float value = other.distance + other.estimate;
			return num.CompareTo(value);
		}
	}

	public delegate bool DoesObstacleBlock(GameObject obj);

	public static bool drawQuadTree;

	protected static long id = 1L;

	protected QuadTree tree;

	public bool DebugLogging;

	protected QuadTree Tree
	{
		get
		{
			if (tree == null)
			{
				tree = new QuadTree(new Vector2(5f, 5f), 10);
				PathNode[] components = Utils.GetComponents<PathNode>(base.gameObject, Utils.SearchChildren, true);
				int num = 0;
				PathNode[] array = components;
				foreach (PathNode pathNode in array)
				{
					if (pathNode.name == "PathNode")
					{
						pathNode.name = "PathNode_" + num;
						num++;
					}
					tree.AddPoint(pathNode.gameObject);
				}
			}
			return tree;
		}
	}

	public bool HasADoor
	{
		get
		{
			PathNode[] components = Utils.GetComponents<PathNode>(base.gameObject, Utils.SearchChildren);
			PathNode[] array = components;
			foreach (PathNode pathNode in array)
			{
				if (pathNode.Type == PathNodeBase.NodeType.Door)
				{
					return true;
				}
			}
			return false;
		}
	}

	public void Start()
	{
	}

	public void OnDrawGizmos()
	{
		if (drawQuadTree && tree != null)
		{
			tree.DebugDraw();
		}
	}

	public PathNode RandomPathNode()
	{
		PathNode[] components = Utils.GetComponents<PathNode>(base.gameObject, Utils.SearchChildren, true);
		int num = components.Length;
		if (num > 0)
		{
			if (num > 1)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				return components[num2];
			}
			return components[0];
		}
		return null;
	}

	public PathNode ClosestPathNode(Vector3 point)
	{
		List<GameObject> list = Tree.Query(point, 15f, true);
		if (list.Count <= 0)
		{
			PathNode[] components = Utils.GetComponents<PathNode>(base.gameObject, Utils.SearchChildren, true);
			if (components == null)
			{
				return null;
			}
			PathNode[] array = components;
			foreach (PathNode pathNode in array)
			{
				list.Add(pathNode.gameObject);
			}
		}
		foreach (GameObject item in list)
		{
			Vector3 start = point;
			start.y += PathNodeBase.StepHeight;
			Vector3 position = item.transform.position;
			position.y += PathNodeBase.StepHeight;
			RaycastHit hitInfo;
			if (!Physics.Linecast(start, position, out hitInfo, 4694016) || !(hitInfo.collider.gameObject != item))
			{
				PathNode component = Utils.GetComponent<PathNode>(item);
				DebugLog("Checking to see if there is a gap between " + item.name + " start: " + point + " end:" + item.transform.position);
				if (!(component != null) || !component.GapInBetween(point))
				{
					return Utils.GetComponent<PathNode>(item);
				}
				DebugLog("Line to node " + item.name + " has a gap.");
			}
		}
		DebugLog("There isn't a clear line or non-gap from any node to point!");
		return null;
	}

	public List<PathNode> FindPath(Vector3 start, Vector3 end, bool checkForObstacles, DoesObstacleBlock obstCallback)
	{
		PathNode pathNode = ClosestPathNode(start);
		if (pathNode == null)
		{
			DebugLog("No start");
			return null;
		}
		PathNode pathNode2 = ClosestPathNode(end);
		if (pathNode2 == null)
		{
			DebugLog("No end");
			return null;
		}
		if (!Physics.CheckCapsule(pathNode.transform.position, pathNode2.transform.position, PathNodeBase.StepHeight, 4694016))
		{
			List<PathNode> list = new List<PathNode>();
			list.Add(pathNode);
			list.Add(pathNode2);
			return list;
		}
		DebugLog("StartNode is " + pathNode.name);
		DebugLog("EndNode is " + pathNode2.name);
		Dictionary<PathNode, NodeData> dictionary = new Dictionary<PathNode, NodeData>();
		BinaryHeap<NodeData> binaryHeap = new BinaryHeap<NodeData>();
		NodeData nodeData = new NodeData(pathNode, pathNode2.transform.position);
		dictionary.Add(nodeData.node, nodeData);
		binaryHeap.Push(nodeData);
		while (binaryHeap.Count > 0)
		{
			nodeData = binaryHeap.Pop();
			DebugLog(" Visiting node " + nodeData.node.name + " links");
			PathNode[] links = nodeData.node.links;
			foreach (PathNode pathNode3 in links)
			{
				DebugLog(" Visiting node link: " + pathNode3.name);
				if (pathNode3 == nodeData.node || nodeData.node.disabledLinks.Contains(pathNode3))
				{
					continue;
				}
				if (checkForObstacles)
				{
					DebugLog(" Checking for obstacles");
					GameObject gameObject = nodeData.node.ClearLineTo(pathNode3);
					if (gameObject != null)
					{
						DebugLog(" Found obstacles between nodes calling blocking callback.");
						bool flag = true;
						if (obstCallback != null)
						{
							DebugLog("Checking to see if line line between " + pathNode3.name + " and " + nodeData.node.name + " is blocking");
							flag = obstCallback(gameObject);
							DebugLog("Result is " + flag);
						}
						if (flag)
						{
							continue;
						}
					}
					DebugLog(" Found no obstacles between nodes.");
				}
				DebugLog(" Adding node to open list: " + pathNode3.name);
				NodeData value = null;
				if (dictionary.TryGetValue(pathNode3, out value))
				{
					if (!value.closed)
					{
						float num = nodeData.distance + (pathNode3.transform.position - nodeData.node.transform.position).sqrMagnitude;
						if (num < value.distance)
						{
							binaryHeap.Remove(value);
							value.distance = num;
							value.parent = nodeData.node;
							binaryHeap.Push(value);
						}
					}
				}
				else
				{
					value = new NodeData(pathNode3, pathNode2.transform.position);
					value.distance = nodeData.distance + (pathNode3.transform.position - nodeData.node.transform.position).sqrMagnitude;
					value.parent = nodeData.node;
					dictionary.Add(value.node, value);
					binaryHeap.Push(value);
				}
				foreach (PathNode teleportLink in nodeData.node.teleportLinks)
				{
					if (dictionary.TryGetValue(teleportLink, out value))
					{
						if (!value.closed)
						{
							float distance = nodeData.distance;
							if (distance < value.distance)
							{
								binaryHeap.Remove(value);
								value.distance = distance;
								value.parent = nodeData.parent;
								binaryHeap.Push(value);
							}
						}
					}
					else
					{
						value = new NodeData(teleportLink, pathNode2.transform.position);
						value.distance = nodeData.distance;
						value.parent = nodeData.node;
						dictionary.Add(value.node, value);
						binaryHeap.Push(value);
					}
				}
			}
			if (nodeData.node == pathNode2)
			{
				List<PathNode> list2 = new List<PathNode>();
				list2.Add(nodeData.node);
				while (nodeData.parent != null)
				{
					NodeData nodeData2 = dictionary[nodeData.parent];
					nodeData = nodeData2;
					list2.Add(nodeData.node);
				}
				list2.Reverse();
				return list2;
			}
			nodeData.closed = true;
		}
		return null;
	}

	private void DebugCurrentViablePath(Dictionary<PathNode, NodeData> nodeMap, NodeData n)
	{
		string text = string.Empty;
		NodeData value = n;
		while (value != null && value.parent != null)
		{
			text = text + " " + value.parent.name;
			if (!nodeMap.TryGetValue(value.parent, out value))
			{
				break;
			}
		}
		DebugLog("Current path back to start is " + text);
	}

	public bool PickRandomDestination(Vector3 startLocation, out Vector3 destination)
	{
		PathNode[] components = Utils.GetComponents<PathNode>(base.gameObject, Utils.SearchChildren, true);
		if (components.Length > 0)
		{
			int num = UnityEngine.Random.Range(0, components.Length - 1);
			destination = components[num].transform.position;
			return true;
		}
		destination = Vector3.zero;
		return false;
	}

	private void DebugLog(string message)
	{
		if (DebugLogging)
		{
			CspUtils.DebugLog(message);
		}
	}
}
