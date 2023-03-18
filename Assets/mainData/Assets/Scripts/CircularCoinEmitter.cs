using System;
using UnityEngine;

public class CircularCoinEmitter : CoinEmitterBase
{
	private float angle;

	private Vector3 position;

	protected override void OnEmitterInitialized()
	{
		angle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		position = dispenserObject.transform.position;
	}

	protected override void EmitCoin(int coinIndex)
	{
		if (!adapter.usePhysics)
		{
			Vector3 b = new Vector3(adapter.range * (float)Math.Cos(angle), adapter.height, (float)Math.Sin(angle) * adapter.range);
			UnityEngine.Object.Instantiate(prefab, position + b, Quaternion.identity);
		}
		else
		{
			Vector3 a = new Vector3(adapter.range * (float)Math.Cos(angle), 0f, (float)Math.Sin(angle) * adapter.range);
			a.Normalize();
			a += new Vector3(0f, adapter.height, 0f);
			UnityEngine.Object @object = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
			GameObject gameObject = (GameObject)@object;
			gameObject.rigidbody.AddForce(a * adapter.force);
		}
		angle += (float)Math.PI * 2f / (float)adapter.coinAmount;
	}
}
