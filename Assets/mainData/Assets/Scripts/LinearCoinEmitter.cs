using System;
using UnityEngine;

public class LinearCoinEmitter : CoinEmitterBase
{
	private Vector3 coinDir;

	private Vector3 originPos;

	protected override void OnEmitterInitialized()
	{
		originPos = dispenserObject.transform.position;
		Vector3 eulerAngles = dispenserObject.transform.rotation.eulerAngles;
		float f = eulerAngles.y * (float)Math.PI / 180f;
		coinDir = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
		coinDir.Normalize();
		if (adapter.usePhysics)
		{
			coinDir += new Vector3(0f, adapter.height, 0f);
		}
	}

	protected override void EmitCoin(int coinIndex)
	{
		int num = coinIndex + 1;
		Vector3 vector = new Vector3(adapter.range * (float)num * coinDir.x, adapter.height, adapter.range * (float)num * coinDir.z);
		if (!adapter.usePhysics)
		{
			UnityEngine.Object.Instantiate(prefab, originPos + vector, Quaternion.identity);
			return;
		}
		vector.Normalize();
		UnityEngine.Object @object = UnityEngine.Object.Instantiate(prefab, originPos, Quaternion.identity);
		if (@object != null)
		{
			GameObject gameObject = (GameObject)@object;
			gameObject.rigidbody.AddForce(vector * adapter.force * num);
		}
	}
}
