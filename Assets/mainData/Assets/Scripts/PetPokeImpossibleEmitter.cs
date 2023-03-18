using System.Collections.Generic;
using UnityEngine;

public class PetPokeImpossibleEmitter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private int _frameCount;

	public int radius = 7;

	public static GameObject getChildGameObject(GameObject fromGameObject, string withName)
	{
		WheresImpossibleMan[] componentsInChildren = fromGameObject.transform.GetComponentsInChildren<WheresImpossibleMan>();
		WheresImpossibleMan[] array = componentsInChildren;
		foreach (WheresImpossibleMan wheresImpossibleMan in array)
		{
			if (wheresImpossibleMan.gameObject.name == withName)
			{
				return wheresImpossibleMan.gameObject;
			}
		}
		return null;
	}

	public void Update()
	{
		if (SocialSpaceControllerImpl.playerIsIdle)
		{
			return;
		}
		_frameCount++;
		if (_frameCount < 3)
		{
			return;
		}
		_frameCount = 0;
		Collider[] array = Physics.OverlapSphere(base.gameObject.transform.position, radius);
		if (array.Length <= 0)
		{
			return;
		}
		Dictionary<InteractiveObject, bool> dictionary = new Dictionary<InteractiveObject, bool>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			WheresImpossibleMan componentRecursive = Utils.GetComponentRecursive<WheresImpossibleMan>(collider.gameObject);
			if (componentRecursive != null && componentRecursive.enabled)
			{
				InteractiveObject componentRecursive2 = Utils.GetComponentRecursive<InteractiveObject>(collider.gameObject);
				if (componentRecursive2 != null)
				{
					AppShell.Instance.EventMgr.Fire(this, new InteractiveObjectUsedMessage(componentRecursive2));
				}
			}
		}
	}
}
