using System.Collections;
using UnityEngine;

public class AIControllerRandom : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public FRange destinationRepickFrequency = new FRange(0.3f, 2f);

	public float destinationMaxRange = 20f;

	protected CharacterGlobals charGlobals;

	private void OnEnable()
	{
		charGlobals = GetComponent<CharacterGlobals>();
		StartCoroutine(AIMain());
	}

	protected IEnumerator AIMain()
	{
		while (true)
		{
			PickDestination();
			yield return new WaitForSeconds(destinationRepickFrequency.RandomValue);
		}
	}

	protected void PickDestination()
	{
		if (charGlobals != null)
		{
			Vector3 newDestination = BehaviorAttackTeleport.FindTeleportableLocation(charGlobals, destinationMaxRange);
			charGlobals.motionController.setDestination(newDestination, true);
		}
	}
}
