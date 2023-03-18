using UnityEngine;

[AddComponentMenu("Hq/Switch/Switch")]
public class HqSwitchObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool playerOnly = true;

	public bool playSFX = true;

	protected bool isOn = true;

	protected bool isHovering;

	protected GameObject onObject;

	protected GameObject offObject;

	protected GameObject hoverObject;

	protected HqSwitchController switchController;

	public void Start()
	{
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.name == "OnHover")
			{
				hoverObject = item.gameObject;
			}
			else if (item.gameObject.name == "On")
			{
				onObject = item.gameObject;
			}
			else if (item.gameObject.name == "Off")
			{
				offObject = item.gameObject;
			}
			else if (item.gameObject.name == "Controller")
			{
				switchController = Utils.GetComponent<HqSwitchController>(item);
			}
		}
		isOn = true;
		DeactivateTree(hoverObject);
		DeactivateTree(offObject);
	}

	public void Update()
	{
		if (isHovering)
		{
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
		}
		else if (isOn)
		{
			if (onObject != null && !onObject.active)
			{
				ActivateTree(onObject);
			}
			if (offObject != null && offObject.active)
			{
				DeactivateTree(offObject);
			}
		}
		else
		{
			if (onObject != null && onObject.active)
			{
				DeactivateTree(onObject);
			}
			if (offObject != null && !offObject.active)
			{
				ActivateTree(offObject);
			}
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		if (!(switchController == null) && !switchController.CanUse())
		{
			return;
		}
		if (hoverObject != null)
		{
			ActivateTree(hoverObject);
			isHovering = true;
		}
		if (isOn)
		{
			if (onObject != null)
			{
				DeactivateTree(onObject);
			}
		}
		else if (offObject != null)
		{
			DeactivateTree(offObject);
		}
	}

	public void OnMouseRolloverExit()
	{
		if (!(switchController == null) && !switchController.CanUse())
		{
			return;
		}
		if (hoverObject != null)
		{
			DeactivateTree(hoverObject);
			isHovering = false;
		}
		if (isOn)
		{
			if (onObject != null)
			{
				ActivateTree(onObject);
			}
		}
		else if (offObject != null)
		{
			ActivateTree(offObject);
		}
	}

	public bool OnMouseClick(GameObject player)
	{
		if (switchController == null || switchController.CanUse())
		{
			return Toggle();
		}
		return false;
	}

	private bool Toggle()
	{
		isOn = !isOn;
		if (isOn)
		{
			if (onObject != null)
			{
				ActivateTree(onObject);
			}
			if (offObject != null)
			{
				DeactivateTree(offObject);
			}
		}
		else
		{
			if (offObject != null)
			{
				ActivateTree(offObject);
			}
			if (onObject != null)
			{
				DeactivateTree(onObject);
			}
		}
		if (switchController != null)
		{
			switchController.Flip();
		}
		if (playSFX)
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
		}
		return true;
	}

	protected void ActivateTree(GameObject obj)
	{
		if (!(obj == null))
		{
			Utils.ForEachTree(obj, delegate(GameObject o)
			{
				o.active = true;
				if (o.particleEmitter != null)
				{
					o.particleEmitter.emit = true;
				}
				o.SendMessage("OnActivated", SendMessageOptions.DontRequireReceiver);
			});
		}
	}

	protected void DeactivateTree(GameObject obj)
	{
		if (!(obj == null))
		{
			Utils.ForEachTree(obj, delegate(GameObject o)
			{
				if (o.particleEmitter != null)
				{
					o.particleEmitter.emit = false;
				}
				else
				{
					o.active = false;
				}
				o.SendMessage("OnDeactivated", SendMessageOptions.DontRequireReceiver);
			});
		}
	}
}
