using System;
using UnityEngine;

[AddComponentMenu("LOD/Toggle2")]
public class LodToggle2 : LodBase
{
	public bool rendererOnly = true;

	public override void SetLod(int lod)
	{
		if (lod == 0)
		{
			ActivateChildren();
		}
		else
		{
			DeactivateChildren();
		}
	}

	public override bool IsVisible()
	{
		return GetComponentInChildren<Renderer>().isVisible;
	}

	public override Bounds GetBounds()
	{
		return GetComponentInChildren<Renderer>().bounds;
	}

	protected void ActivateChildren()
	{
		if (rendererOnly)
		{
			Utils.ForEachTree(base.gameObject, delegate(GameObject go)
			{
				if (go.renderer != null)
				{
					go.renderer.enabled = true;
				}
			});
		}
		else
		{
			ForAllChildren(delegate(GameObject go)
			{
				go.active = true;
			});
		}
	}

	protected void DeactivateChildren()
	{
		if (rendererOnly)
		{
			Utils.ForEachTree(base.gameObject, delegate(GameObject go)
			{
				if (go.renderer != null)
				{
					go.renderer.enabled = false;
				}
			});
		}
		else
		{
			ForAllChildren(delegate(GameObject go)
			{
				go.active = false;
			});
		}
	}

	protected void ForAllChildren(Action<GameObject> action)
	{
		foreach (Transform item in base.transform)
		{
			Utils.ForEachTree(item.gameObject, action);
		}
	}
}
