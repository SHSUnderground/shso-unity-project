using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Transform start;

	public Transform end;

	public bool doIt;

	protected PathFinder finder;

	private List<PathNode> path;

	public void Start()
	{
		Object[] array = Object.FindSceneObjectsOfType(typeof(GameObject));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject g = (GameObject)array2[i];
			finder = Utils.GetComponent<PathFinder>(g);
			if (finder != null)
			{
				break;
			}
		}
	}

	public void Update()
	{
		if (doIt)
		{
			doIt = false;
			if (!(start == null) && !(end == null))
			{
				path = finder.FindPath(start.transform.position, end.transform.position, true, null);
			}
		}
	}

	public void OnDrawGizmos()
	{
		if (path != null)
		{
			for (int i = 1; i < path.Count; i++)
			{
				Debug.DrawLine(path[i - 1].transform.position, path[i].transform.position, Color.green);
			}
		}
	}
}
