using UnityEngine;

public class PointCoinEmitter : CoinEmitterBase
{
	public Transform[] coinLocations;

	protected override void OnEmitterInitialized()
	{
		base.OnEmitterInitialized();
	}

	protected override void EmitCoin(int coinIndex)
	{
		base.EmitCoin(coinIndex);
		if (coinIndex < coinLocations.Length)
		{
			GameObject gameObject = Object.Instantiate(prefab, coinLocations[coinIndex].position, coinLocations[coinIndex].rotation) as GameObject;
			gameObject.rigidbody.isKinematic = true;
		}
	}
}
