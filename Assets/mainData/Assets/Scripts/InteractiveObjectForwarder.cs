using System;
using UnityEngine;

[AddComponentMenu("Interactive Object/Forwarder")]
public class InteractiveObjectForwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Flags]
	public enum Options
	{
		Trigger = 0x1,
		Rollver = 0x2,
		Click = 0x4,
		TiggerRolloverClick = 0x7,
		RolloverClick = 0x6
	}

	public InteractiveObject owner;

	public Options options;

	public void SetOwner(InteractiveObject owner, Options options)
	{
		this.owner = owner;
		this.options = options;
	}

	public void OnTriggerEnter(Collider other)
	{
		if ((options & Options.Trigger) != 0)
		{
			owner.OnTriggerEnter(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if ((options & Options.Trigger) != 0)
		{
			owner.OnTriggerExit(other);
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		if ((options & Options.Rollver) != 0)
		{
			owner.OnMouseRolloverEnter(data);
		}
	}

	public void OnMouseRolloverExit()
	{
		if ((options & Options.Rollver) != 0)
		{
			owner.OnMouseRolloverExit();
		}
	}

	public bool OnMouseClick(GameObject player)
	{
		if ((options & Options.Click) != 0)
		{
			return owner.OnMouseClick(player);
		}
		return false;
	}
}
