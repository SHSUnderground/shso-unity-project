using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hq/Switch/Holding Cell")]
public class HqHoldingCell : HqTriggerArea
{
	public List<PathNode> enclosedPathNodes = new List<PathNode>();

	protected Dictionary<PathNode, List<PathNode>> outsideLinks = new Dictionary<PathNode, List<PathNode>>();

	protected override void AddNewEntry(out Entry data, GameObject obj)
	{
		if (!isOn)
		{
			data = null;
			return;
		}
		AIControllerHQ aIControllerHQ = Utils.GetComponent<AIControllerHQ>(obj, Utils.SearchChildren);
		if (aIControllerHQ == null)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(obj, Utils.SearchChildren);
			if (component != null)
			{
				aIControllerHQ = component.AIController;
			}
		}
		if (aIControllerHQ == null)
		{
			data = null;
			return;
		}
		data = new Entry();
		data.lastUpdate = Time.time;
		data.hqComp = aIControllerHQ;
		objects[obj] = data;
		aIControllerHQ.InJail = true;
	}

	protected override void RemoveEntry(GameObject go)
	{
		if (!(go != null))
		{
			return;
		}
		Entry value;
		objects.TryGetValue(go, out value);
		if (value != null && value.hqComp != null)
		{
			AIControllerHQ aIControllerHQ = value.hqComp as AIControllerHQ;
			if (aIControllerHQ != null)
			{
				aIControllerHQ.InJail = false;
			}
		}
		base.RemoveEntry(go);
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || Utils.GetComponent<AIControllerHQ>(go) == null)
		{
			return false;
		}
		return true;
	}

	protected override GameObject GetRelevantGameObject(GameObject go)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null)
		{
			return go;
		}
		return null;
	}

	public override void Start()
	{
		base.Start();
		TurnOn();
	}

	public override void TurnOn()
	{
		base.TurnOn();
		if (enclosedPathNodes != null)
		{
			foreach (PathNode enclosedPathNode in enclosedPathNodes)
			{
				SetPathNodeLinks(enclosedPathNode, true);
			}
		}
	}

	public override void TurnOff()
	{
		base.TurnOff();
		if (enclosedPathNodes != null)
		{
			foreach (PathNode enclosedPathNode in enclosedPathNodes)
			{
				SetPathNodeLinks(enclosedPathNode, false);
			}
		}
	}

	protected void SetPathNodeLinks(PathNode node, bool unlink)
	{
		if (!outsideLinks.ContainsKey(node))
		{
			outsideLinks[node] = new List<PathNode>();
			PathNode[] links = node.links;
			foreach (PathNode item in links)
			{
				if (!enclosedPathNodes.Contains(item) && !node.disabledLinks.Contains(item))
				{
					outsideLinks[node].Add(item);
				}
			}
		}
		if (outsideLinks[node] != null && outsideLinks[node].Count > 0)
		{
			foreach (PathNode item2 in outsideLinks[node])
			{
				if (unlink)
				{
					if (!node.disabledLinks.Contains(item2))
					{
						node.disabledLinks.Add(item2);
						item2.disabledLinks.Add(node);
					}
				}
				else if (node.disabledLinks.Contains(item2))
				{
					node.disabledLinks.Remove(item2);
					if (item2.disabledLinks.Contains(node))
					{
						item2.disabledLinks.Remove(node);
					}
				}
			}
		}
	}
}
