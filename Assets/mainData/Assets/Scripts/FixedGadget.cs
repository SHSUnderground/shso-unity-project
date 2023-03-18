using UnityEngine;

public class FixedGadget : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void OnDoneUsing();

	public OnDoneUsing doneCallback;

	protected bool isInUse;

	public bool IsInUse
	{
		get
		{
			return isInUse;
		}
	}

	private void Start()
	{
		isInUse = false;
	}

	public virtual void TurnOn()
	{
	}

	public virtual void TurnOff()
	{
	}
}
