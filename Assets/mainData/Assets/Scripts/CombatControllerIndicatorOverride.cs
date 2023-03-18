using UnityEngine;

public class CombatControllerIndicatorOverride : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject onIndicator;

	public GameObject offIndicator;

	public bool disableMouseOver = true;

	protected virtual void Start()
	{
		CombatController component = Utils.GetComponent<CombatController>(base.gameObject);
		if (disableMouseOver && component != null)
		{
			component.useMouseOver = false;
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		MouseRollover mouseRollover = data as MouseRollover;
		if (mouseRollover.allowUserInput)
		{
			if (onIndicator != null)
			{
				Utils.ActivateTree(onIndicator, true);
			}
			if (offIndicator != null)
			{
				Utils.ActivateTree(offIndicator, false);
			}
		}
	}

	public void OnMouseRolloverExit()
	{
		if (onIndicator != null)
		{
			Utils.ActivateTree(onIndicator, false);
		}
		if (offIndicator != null)
		{
			Utils.ActivateTree(offIndicator, true);
		}
	}
}
