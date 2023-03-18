using UnityEngine;

public class LifetimelessGeneral : MonoBehaviour, IGeneralEffect
{
	public float ChanceToPlay
	{
		get
		{
			return -1f;
		}
	}

	public bool IsFinished()
	{
		return false;
	}

	public bool IsLooping()
	{
		return false;
	}
}
