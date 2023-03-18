using UnityEngine;

public abstract class HotSpotController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum MovementTypes
	{
		FlyFeetDown = 1,
		FlyFeetUp = 2,
		SuperJump = 4,
		Agile = 8,
		Generic = 0x10
	}

	public abstract MovementTypes MovementType
	{
		get;
	}

	public abstract bool CanPlayerUse(GameObject player);

	public abstract bool StartWithPlayer(GameObject player);
}
