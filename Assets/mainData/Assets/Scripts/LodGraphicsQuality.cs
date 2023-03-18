using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("LOD/Graphics Quality")]
public class LodGraphicsQuality : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Rule
	{
		KillAtOrBelow,
		KillAtOrAbove,
		KillBelow,
		KillAbove
	}

	public GraphicsOptions.GraphicsQuality cutOff;

	public Rule rule;

	public void Start()
	{
		StartCoroutine(WaitForAppShell());
	}

	public void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
	}

	public void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
	}

	protected IEnumerator WaitForAppShell()
	{
		yield return 0;
		while (!AppShell.Instance.IsReady)
		{
			yield return 0;
		}
		OnGraphicsOptionsChange(null);
	}

	protected void OnGraphicsOptionsChange(GraphicsOptionsChange msg)
	{
		bool flag = false;
		switch (rule)
		{
		case Rule.KillAtOrBelow:
			if (GraphicsOptions.ModelQuality <= cutOff)
			{
				flag = true;
			}
			break;
		case Rule.KillAtOrAbove:
			if (GraphicsOptions.ModelQuality >= cutOff)
			{
				flag = true;
			}
			break;
		case Rule.KillBelow:
			if (GraphicsOptions.ModelQuality < cutOff)
			{
				flag = true;
			}
			break;
		case Rule.KillAbove:
			if (GraphicsOptions.ModelQuality > cutOff)
			{
				flag = true;
			}
			break;
		}
		if (flag)
		{
			List<GameObject> deadObjects = new List<GameObject>();
			foreach (Transform item in base.transform)
			{
				deadObjects.Add(item.gameObject);
				Utils.ForEachTree(item.gameObject, delegate(GameObject go)
				{
					deadObjects.Add(go);
				});
			}
			foreach (GameObject item2 in deadObjects)
			{
				Object.Destroy(item2);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
