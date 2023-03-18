using UnityEngine;

public class EnableProxy : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool invert;

	public GameObject[] objects;

	protected bool unloading;

	private void OnEnable()
	{
		ToggleObjects(true);
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		if (!unloading)
		{
			ToggleObjects(false);
		}
	}

	public void ToggleObjects(bool enabled)
	{
		enabled = ((!invert) ? enabled : (!enabled));
		GameObject[] array = objects;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				Utils.ActivateTree(gameObject, enabled);
			}
		}
	}
}
