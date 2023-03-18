using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class Use
{
	public enum Posture
	{
		stand,
		sit,
		jump
	}

	protected const float MAX_USE_ANGLE_DEFAULT = 1f;

	protected List<ItemAction> itemActions;

	protected string dockPointName;

	protected Posture postureType;

	protected Vector3 useVector;

	protected float maxUseAngleDelta;

	public Vector3 UseVector
	{
		get
		{
			return useVector;
		}
	}

	public float MaxUseAngleDelta
	{
		get
		{
			return maxUseAngleDelta;
		}
	}

	public List<ItemAction> ItemActions
	{
		get
		{
			return itemActions;
		}
	}

	public Posture PostureType
	{
		get
		{
			return postureType;
		}
	}

	public string DockPointName
	{
		get
		{
			return dockPointName;
		}
	}

	public Use(DataWarehouse data)
	{
		InitializeFromData(data);
	}

	public void InitializeFromData(DataWarehouse data)
	{
		dockPointName = data.TryGetString("dockpoint_name", null);
		string value = data.TryGetString("use_posture", "stand");
		postureType = (Posture)(int)Enum.Parse(typeof(Posture), value, true);
		useVector = data.TryGetVector("use_vector", Vector3.up);
		maxUseAngleDelta = data.TryGetFloat("max_use_angle_delta", 1f);
		ParseActions(data);
	}

	private void ParseActions(DataWarehouse data)
	{
		string text = data.TryGetXml("actions", null);
		if (text == null)
		{
			return;
		}
		itemActions = new List<ItemAction>();
		XPathNavigator value = data.GetValue("actions");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("action", string.Empty);
		if (xPathNodeIterator == null)
		{
			return;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ItemAction));
		while (xPathNodeIterator.MoveNext())
		{
			XPathNavigator current = xPathNodeIterator.Current;
			if (current != null)
			{
				string outerXml = current.OuterXml;
				ItemAction itemAction = xmlSerializer.Deserialize(new StringReader(outerXml)) as ItemAction;
				if (itemAction != null)
				{
					itemActions.Add(itemAction);
				}
			}
		}
	}
}
