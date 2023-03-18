using UnityEngine;

public class InteractiveObjectTriggerSelector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum SelectionMode
	{
		Random,
		Sequential
	}

	public enum ForwardMode
	{
		Broadcast,
		Send
	}

	public SelectionMode selectionMode;

	public ForwardMode forwardingMode;

	public GameObject[] objects;

	private int nextSequentialIndex;

	public void Triggered(object param)
	{
		if (objects.Length != 0)
		{
			int num;
			if (selectionMode == SelectionMode.Random)
			{
				num = Random.Range(0, objects.Length);
			}
			else
			{
				num = ((nextSequentialIndex < objects.Length) ? nextSequentialIndex : 0);
				nextSequentialIndex = num + 1;
			}
			if (forwardingMode == ForwardMode.Broadcast)
			{
				objects[num].BroadcastMessage("Triggered", param, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				objects[num].SendMessage("Triggered", param, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
