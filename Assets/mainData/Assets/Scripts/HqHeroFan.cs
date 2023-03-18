using System;
using UnityEngine;

[AddComponentMenu("Hq/Toys/Hero Fan")]
public class HqHeroFan : HqFan
{
	public override bool IsOn
	{
		get
		{
			Type state = HqController2.Instance.State;
			if (state != typeof(HqController2.HqControllerFlinga))
			{
				return false;
			}
			return true;
		}
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || go == base.gameObject)
		{
			return false;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			if (go.transform.GetChild(i).gameObject == base.gameObject)
			{
				return false;
			}
		}
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null)
		{
			return true;
		}
		return false;
	}
}
