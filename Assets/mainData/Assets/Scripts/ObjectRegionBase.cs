using System.Collections.Generic;
using UnityEngine;

public class ObjectRegionBase : ScenarioEventHandlerEnableBase
{
	public GameObject constraintObject;

	protected List<ObjectRegionConstraint> inclusionConstraints;

	protected List<ObjectRegionConstraint> exclusionConstraints;

	protected NetworkComponent netComp;

	protected override void Start()
	{
		base.Start();
		netComp = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
		inclusionConstraints = new List<ObjectRegionConstraint>();
		exclusionConstraints = new List<ObjectRegionConstraint>();
		Component[] array = (!(constraintObject != null)) ? GetComponentsInChildren(typeof(ObjectRegionConstraint)) : constraintObject.GetComponentsInChildren(typeof(ObjectRegionConstraint));
		Component[] array2 = array;
		foreach (Component component in array2)
		{
			ObjectRegionConstraint objectRegionConstraint = component as ObjectRegionConstraint;
			if (objectRegionConstraint.constraintType == ObjectRegionConstraint.ConstraintType.Inclusion)
			{
				inclusionConstraints.Add(objectRegionConstraint);
			}
			else
			{
				exclusionConstraints.Add(objectRegionConstraint);
			}
		}
	}

	public bool checkConstraints(Vector3 point, bool replication)
	{
		if (inclusionConstraints.Count > 0)
		{
			bool flag = false;
			foreach (ObjectRegionConstraint inclusionConstraint in inclusionConstraints)
			{
				if ((!inclusionConstraint.replicationOnly || replication) && inclusionConstraint.checkPoint(point))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (exclusionConstraints.Count > 0)
		{
			foreach (ObjectRegionConstraint exclusionConstraint in exclusionConstraints)
			{
				if ((!exclusionConstraint.replicationOnly || replication) && exclusionConstraint.checkPoint(point))
				{
					return false;
				}
			}
		}
		return true;
	}
}
